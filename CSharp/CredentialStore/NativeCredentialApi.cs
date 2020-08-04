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
using System.Runtime.CompilerServices;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Wrapper around static extern methods to allow Mocking and testing
    /// </summary>
    public class NativeCredentialApi : INativeCredentialApi
    {
        private readonly IMarshalService _marshalService;
        private readonly IErrorCodeToStringService _errorCodeToStringService;
        private readonly ILoggerAdapter _logger; 

        public NativeCredentialApi(IMarshalService marshalService, IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
        {
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            _errorCodeToStringService = errorCodeToStringService ?? throw new ArgumentNullException(nameof(errorCodeToStringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredRead(string, CredentialType, int, out IntPtr)"/>
        /// </summary>
        public NativeApi.Credential? CredRead(string target, CredentialType type, int reservedFlag) =>
            !NativeApi.CredentialApi.CredRead(target, type, reservedFlag, out var credentialPtr)
                ? null
                : GetCredentialFromAndFreePtr(credentialPtr);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredWrite(IntPtr, int)"/>
        /// </summary>
        public bool CredWrite(NativeApi.Credential credential, int flags)
        {
            var nativeCredentialPtr = IntPtr.Zero;
            try
            {
                nativeCredentialPtr = _marshalService.AllocHGlobal(_marshalService.SizeOf(credential));
                _marshalService.StructureToPtr(credential, nativeCredentialPtr, false);

                if (!NativeApi.CredentialApi.CredWrite(nativeCredentialPtr, flags))
                    _errorCodeToStringService.LogLastWin32Error(_logger, Enumerable.Empty<int>());

                return true;
            }
            finally
            {
                if (nativeCredentialPtr != IntPtr.Zero)
                    _marshalService.FreeHGlobal(nativeCredentialPtr);
            }
        }

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredDelete(string, int, int)"/>
        /// </summary>
        public bool CredDelete(string target, int type, int flags) =>
            NativeApi.CredentialApi.CredDelete(target, type, flags);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredEnumerate(string?, int, out int, out IntPtr)"/>
        /// </summary>
        public IEnumerable<NativeApi.Credential> CredEnumerate(string? filter, int flag)
        {
            if (!NativeApi.CredentialApi.CredEnumerate(filter, flag, out int count, out IntPtr credentialsPtr))
            {
                int lastError = _marshalService.GetLastWin32Error();
                if (lastError != 0)
                    _logger.Warning(_errorCodeToStringService.GetMessageFor(lastError));
                yield break;
            }

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var nextPtr = IntPtr.Add(credentialsPtr, IntPtr.Size * i);
                    var currentPtr = _marshalService.ReadIntPtr(nextPtr);
                    var nativeCredential = _marshalService.PtrToStructure<NativeApi.Credential>(currentPtr);
                    if (nativeCredential == null)
                    {
                        _logger.Error($"pointer failed to pin to structure at index {i}");
                        yield break;
                    }

                    NativeApi.Credential copy = nativeCredential;
                    yield return copy;
                }
            }
            finally
            {
                if (!NativeApi.CredentialApi.CredFree(credentialsPtr))
                {
                    int lastError = _marshalService.GetLastWin32Error();
                    if (lastError != 0)
                        _logger.Warning(_errorCodeToStringService.GetMessageFor(lastError));
                }
            }
        }

        public void CredFree(IntPtr handle)
        {
            if (!NativeApi.CredentialApi.CredFree(handle))
                throw new Win32Exception(_marshalService.GetLastWin32Error());
        }

        private NativeApi.Credential? GetCredentialFromAndFreePtr(IntPtr credentialPtr, [CallerMemberName] string callerMemberName = "")
        {
            if (credentialPtr == IntPtr.Zero)
            {
                _logger.Warning("null credential pointer, unable to convert to Credential object", callerMemberName);
                return null;
            }

            using var handle = new NativeApi.CriticalCredentialHandle(credentialPtr, this, _errorCodeToStringService, _logger);
            if (handle.IsValid && handle.NativeCredential != null)
            {
                // make a copy so we're not referencing the pinned struct
                NativeApi.Credential nativeCredential = handle.NativeCredential;
                return nativeCredential;
            }

            _logger.Warning("Unable to get structure from credential pointer");
            return null;
        }
    }
}
