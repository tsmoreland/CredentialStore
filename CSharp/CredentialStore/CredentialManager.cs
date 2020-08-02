//
// Copyright © 2020 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Moreland.Security.Win32.CredentialStore.NativeApi.ErrorCode;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// <inheritdoc cref="ICredentialManager"/>
    /// </summary>
    public sealed class CredentialManager : ICredentialManager
    {
        private readonly INativeCredentialApi _nativeCredentialApi;
        private readonly ILoggerAdapter _logger;

        /// <summary>
        /// Instantiates a new instance of the 
        /// <see cref="CredentialManager"/> object
        /// </summary>
        /// <param name="logger">logger used to aid in debugging</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="logger"/> is null
        /// </exception>
        public CredentialManager(ILoggerAdapter logger)
            : this(new NativeCredentialApi(logger), logger)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the 
        /// <see cref="CredentialManager"/> object
        /// </summary>
        /// <param name="nativeCredentialApi">Native Win32 Credential API access</param>
        /// <param name="logger">logger used to aid in debugging</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="nativeCredentialApi"/> or
        /// <paramref name="logger"/> are null
        /// </exception>
        public CredentialManager(INativeCredentialApi nativeCredentialApi, ILoggerAdapter logger)
        {
            _nativeCredentialApi = nativeCredentialApi ?? throw new ArgumentNullException(nameof(nativeCredentialApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns all Credentials from the Users credential set
        /// </summary>
        public IEnumerable<Credential> Credentials => GetCredentials(null, NativeApi.EnumerateFlag.EnumerateAllCredentials);

        /// <summary>
        /// Finds a credential with the given id value and optionally <see cref="CredentialType"/>
        /// </summary>
        /// <param name="id">id of the credential to be found</param>
        /// <param name="type">
        /// <see cref="CredentialType"/> of <see cref="Credential"/> to be found, 
        /// by default <see cref="CredentialType.DomainPassword"/>
        /// </param>
        /// <returns><see cref="Credential"/> found or null</returns>
        /// <exception cref="ArgumentException">
        /// if <paramref name="id"/> is null or empty or if returned credential structure is malformed
        /// </exception>
        public Credential? Find(string id, CredentialType type = CredentialType.DomainPassword)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id cannot be empty", nameof(id));

            var nativeCredential = _nativeCredentialApi.CredRead(id, type, 0);
            if (nativeCredential != null)
                return new Credential(nativeCredential);

            LogLastWin32Error(_logger, new[] { NotFound });
            return null;
        }

        /// <summary>
        /// Returns all credentials matching wildcard based <paramref name="filter"/>
        /// </summary>
        /// <param name="filter">filter using wildcards</param>
        /// <param name="searchAll">if true all credentials are searched</param>
        /// <returns><see cref="IEnumerable{Credential}"/> of credentials matching filter</returns>
        public IEnumerable<Credential> Find(string filter, bool searchAll) =>
            GetCredentials(filter, searchAll
                ? NativeApi.EnumerateFlag.EnumerateAllCredentials
                : NativeApi.EnumerateFlag.None);

        /// <summary>
        /// Adds <paramref name="credential"/> to Win32 
        /// Credential Manager
        /// </summary>
        /// <param name="credential">credential to be saved</param>
        /// <returns>true on success; otherwise, false</returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credential"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <see cref="Credential.Id"/> or 
        /// <see cref="Credential.UserName"/> are null or empty
        /// </exception>
        public bool Add(Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            using var intermediate = new NativeApi.IntermediateCredential(credential);

            var nativeCredential = intermediate.NativeCredential;
            if (!_nativeCredentialApi.CredWrite(nativeCredential, 0))
                return false;

            _logger.Verbose($"{credential} successfully saved");
            return true;
        }

        /// <summary>
        /// deletes a credential from the user's credential set
        /// </summary>
        /// <param name="credential"></param>
        /// <returns>The function returns true on success and false</returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="credential"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="credential.Id"/> is null or empty
        /// </exception>
        /// <exception cref="Win32Exception">
        /// if error occurs calling native api
        /// </exception>
        public void Delete(Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            Delete(credential.Id, credential.Type);
        }

        /// <summary>
        /// deletes a credential from the user's credential set
        /// </summary>
        /// <param name="id">id of item to be deleted</param>
        /// <param name="type">credential type of item to be deleted</param>
        /// <returns>true if item not found successfully deleted, otherwise false</returns>
        /// <exception cref="ArgumentException">
        /// if <paramref name="id"/> is null or empty
        /// </exception>
        /// <exception cref="Win32Exception">
        /// if error occurs calling native api
        /// </exception>
        public void Delete(string id, CredentialType type)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id cannot be empty", nameof(id));

            if (!_nativeCredentialApi.CredDelete(id, (int)type, 0) && LogLastWin32Error(out int lastError, _logger, new[] {NotFound}))
                throw new Win32Exception(lastError, "Failed to delete {id}");

            _logger.Info($"{id} successfully deleted");
        }

        private IEnumerable<Credential> GetCredentials(string? filter, NativeApi.EnumerateFlag flag) =>
            _nativeCredentialApi.CredEnumerate(filter, (int)flag)
                .Select(credential => new Credential(credential))
                .ToArray();

    }
}
