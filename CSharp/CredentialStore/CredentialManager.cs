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
using static Moreland.Security.Win32.CredentialStore.NativeApi.ErrorCode;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Win32 Credential Manager (Credential Repository) providing CRUD 
    /// operations for Windows Credential Manager 
    /// </summary>
    public sealed class CredentialManager
    {
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
        {
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
                throw new ArgumentException("id cannot be empty");

            var credentialPtr = IntPtr.Zero;
            try
            {
                if (NativeApi.Credential.CredRead(id, type, 0, out credentialPtr))
                    return GetCredentialFromPtr(credentialPtr);

                LogLastWin32Error(_logger, new[] {NotFound});
                return null;
            }
            finally
            {
                if (credentialPtr != IntPtr.Zero)
                {
                    _logger.Verbose("Releasing credential");
                    NativeApi.Credential.CredFree(credentialPtr);
                }

            }
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
        /// Attemtps to Save <paramref name="credential"/> to Win32 
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
        public bool Save(Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            using var intermediate = new NativeApi.IntermediateCredential(credential);

            var nativeCredential = intermediate.NativeCredential;
            if (!NativeApi.Credential.CredWrite(ref nativeCredential, 0))
            {
                LogLastWin32Error(_logger, Enumerable.Empty<int>());
                return false;
            }

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
        public bool Delete(Credential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            throw new NotImplementedException();
        }

        private IEnumerable<Credential> GetCredentials(string? filter, NativeApi.EnumerateFlag flag, [CallerMemberName] string callerMemberName = "")
        {
            if (!NativeApi.Credential.CredEnumerate(filter, (int)flag, out int count, out IntPtr credentialsPtr))
            {
                _logger.Warning(ErrorOrUnknownMessage(Marshal.GetLastWin32Error()), callerMemberName);
                yield break;
            }

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var nextPtr = (Environment.Is64BitProcess)
                        ? new IntPtr(credentialsPtr.ToInt64() + i)
                        : new IntPtr(credentialsPtr.ToInt32() + i);
                    var credential = GetCredentialFromPtr(nextPtr);
                    if (credential == null)
                        yield break;
                    yield return credential;
                }
            }
            finally
            {
                if (!NativeApi.Credential.CredFree(credentialsPtr))
                    _logger.Warning(ErrorOrUnknownMessage(Marshal.GetLastWin32Error()), callerMemberName);
            }
        }
        private Credential? GetCredentialFromPtr(IntPtr credentialPtr, [CallerMemberName] string callerMemberName = "")
        {
            if (credentialPtr == IntPtr.Zero)
            {
                _logger.Warning("null credential pointer, unable to convert to Credential object", callerMemberName);
                return null;
            }

            using var handle = new NativeApi.CriticalCredentialHandle(credentialPtr);
            if (handle.NativeCredential != null)
                return new Credential(handle.NativeCredential);

            _logger.Warning("Unable to get structure from credential pointer");
            return null;
        }

    }
}
