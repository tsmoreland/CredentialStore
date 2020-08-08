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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    internal sealed class NativeInterop : INativeInterop
    {
        private readonly ICredentialApi _credentialApi;
        private readonly IMarshalService _marshalService;
        private readonly IErrorCodeToStringService _errorCodeToStringService;
        private readonly ILoggerAdapter _logger;
        private const int NativeSuccess = 0;

        public NativeInterop(ICredentialApi credentialApi, IMarshalService marshalService, IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
        {
            _credentialApi = credentialApi;
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            _errorCodeToStringService = errorCodeToStringService ?? throw new ArgumentNullException(nameof(errorCodeToStringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// <see cref="ICredentialApi.CredRead(string, CredentialType, int, out IntPtr)"/>
        /// </summary>
        /// <returns>Credential if found; otherwise, null</returns>
        public Credential? CredRead(string target, CredentialType type, int reservedFlag)
        {
            int result = _credentialApi.CredRead(target, type, reservedFlag, out var credentialPtr);
            return result switch
            {
                NativeSuccess => GetCredentialFromAndFreePtr(credentialPtr),
                (int)ExpectedError.NotFound => null,
                _ => LogAndThrowWin32Exception<Credential?>(result),
            };
        }

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredWrite(IntPtr, int)"/>
        /// </summary>
        public void CredWrite(Credential credential, int flags)
        {
            var nativeCredentialPtr = IntPtr.Zero;
            try
            {
                nativeCredentialPtr = _marshalService.AllocHGlobal(_marshalService.SizeOf(credential));
                _marshalService.StructureToPtr(credential, nativeCredentialPtr, false);

                int result = _credentialApi.CredWrite(nativeCredentialPtr, flags);
                if (result != NativeSuccess)
                    LogAndThrowWin32Exception(result);
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
        public void CredDelete(string target, int type, int flags)
        {
            var successResultCodes = new[] { NativeSuccess, (int)ExpectedError.NotFound };
            int result = _credentialApi.CredDelete(target, type, flags);
            if (!successResultCodes.Contains(result))
                LogAndThrowWin32Exception(result);
        }

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredEnumerate(string?, int, out int, out IntPtr)"/>
        /// </summary>
        public IEnumerable<Credential> CredEnumerate(string? filter, int flag)
        {
            int result = _credentialApi.CredEnumerate(filter, flag, out int count, out IntPtr credentialsPtr);
            if (result != NativeSuccess)
            {
                _logger.Warning(_errorCodeToStringService.GetMessageFor(result));
                yield break;
            }

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var nextPtr = IntPtr.Add(credentialsPtr, IntPtr.Size * i);
                    var currentPtr = _marshalService.ReadIntPtr(nextPtr);
                    var nativeCredential = _marshalService.PtrToStructure<Credential>(currentPtr);
                    if (nativeCredential == null)
                    {
                        _logger.Error($"pointer failed to pin to structure at index {i}");
                        yield break;
                    }

                    Credential copy = nativeCredential;
                    yield return copy;
                }
            }
            finally
            {
                result = _credentialApi.CredFree(credentialsPtr);
                if (result != NativeSuccess)
                    _logger.Warning(_errorCodeToStringService.GetMessageFor(result));
            }
        }

        public void CredFree(IntPtr handle)
        {
            int result = _credentialApi.CredFree(handle);
            if (result != NativeSuccess)
                throw new Win32Exception(_marshalService.GetLastWin32Error());
        }

        private Credential? GetCredentialFromAndFreePtr(IntPtr credentialPtr, [CallerMemberName] string callerMemberName = "")
        {
            if (credentialPtr == IntPtr.Zero)
            {
                _logger.Warning("null credential pointer, unable to convert to Credential object", callerMemberName);
                return null;
            }

            using var handle = new CriticalCredentialHandle(credentialPtr, _credentialApi, _errorCodeToStringService, _logger);
            if (handle.IsValid && handle.NativeCredential != null)
            {
                // make a copy so we're not referencing the pinned struct
                Credential nativeCredential = handle.NativeCredential;
                return nativeCredential;
            }

            _logger.Warning("Unable to get structure from credential pointer");
            return null;
        }
        private T LogAndThrowWin32Exception<T>(int error)
        {
            LogAndThrowWin32Exception(error);
            return default!; //unreachable
        }
        private void LogAndThrowWin32Exception(int error)
        {
            _logger.Error(_errorCodeToStringService.GetMessageFor(error));
            throw new Win32Exception(error);
        }

    }
}
