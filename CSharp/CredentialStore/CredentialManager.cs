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

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// <inheritdoc cref="ICredentialManager"/>
    /// </summary>
    public sealed class CredentialManager : ICredentialManager
    {
        private readonly INativeInterop _nativeInterop;
        private readonly IErrorCodeToStringService _errorCodeToStringService;

        public CredentialManager()
            : this(new NativeApi.ErrorCodeToStringService(TraceLoggerAdapter.DiagnosticsLogger), TraceLoggerAdapter.DiagnosticsLogger)
        {
        }

        private CredentialManager(IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
            : this(new NativeApi.NativeInterop(new NativeApi.CriticalCredentialHandleFactory(errorCodeToStringService, logger)), errorCodeToStringService)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the 
        /// <see cref="CredentialManager"/> object
        /// </summary>
        /// <param name="nativeInterop">Win32 api wrappers</param>
        /// <param name="errorCodeToStringService">translates Win32 error codes to message</param>
        /// <exception cref="ArgumentNullException">
        /// if any of the provided arguments are null
        /// </exception>
        public CredentialManager(INativeInterop nativeInterop, IErrorCodeToStringService errorCodeToStringService)
        {
            _nativeInterop = nativeInterop ?? throw new ArgumentNullException(nameof(nativeInterop));
            _errorCodeToStringService = errorCodeToStringService ?? throw new ArgumentNullException(nameof(errorCodeToStringService));

        }

        /// <summary>
        /// Returns all Credentials from the Users credential set
        /// </summary>
        public IEnumerable<Credential> Credentials => 
            GetCredentials(null, NativeApi.EnumerateFlag.EnumerateAllCredentials);

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
        public Credential? Find(string id, CredentialType type = CredentialType.Generic)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id cannot be empty", nameof(id));

            int result = _nativeInterop.CredRead(id, type, 0, out var nativeCredential);
            return result switch
            {
                NativeApi.NativeMethods.Success => nativeCredential != null ? new Credential(nativeCredential) : null,
                (int)NativeApi.ExpectedError.NotFound => null,
                _ => throw new Win32Exception(result, _errorCodeToStringService.GetMessageFor(result)),
            };
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
        public void Add(Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            using var intermediate = new NativeApi.IntermediateCredential(credential);

            var nativeCredential = intermediate.NativeCredential;
            int result = _nativeInterop.CredWrite(nativeCredential, 0);
            if (result != NativeApi.NativeMethods.Success)
                throw new Win32Exception(result, _errorCodeToStringService.GetMessageFor(result));
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

            int result = _nativeInterop.CredDelete(id, (int)type, 0);
            if (result != NativeApi.NativeMethods.Success && result != (int)NativeApi.ExpectedError.NotFound)
                throw new Win32Exception(result, _errorCodeToStringService.GetMessageFor(result));
        }

        private IEnumerable<Credential> GetCredentials(string? filter, NativeApi.EnumerateFlag flag) =>
            _nativeInterop.CredEnumerate(filter, (int)flag)
                .Select(credential => new Credential(credential))
                .ToArray();

    }
}
