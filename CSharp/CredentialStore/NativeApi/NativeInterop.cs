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
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <inheritdoc cref="INativeInterop"/>
    /// </summary>
    internal sealed partial class NativeInterop : INativeInterop
    {
        private readonly ICriticalCredentialHandleFactory _criticalCredentialHandleFactory;

        /// <summary>
        /// Instantiates a new instance of the <see cref="NativeInterop"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if any of the provided arguments are null
        /// </exception>
        public NativeInterop(ICriticalCredentialHandleFactory criticalCredentialHandleFactory)
        {
            _criticalCredentialHandleFactory = criticalCredentialHandleFactory ??
                                               throw new ArgumentNullException(nameof(criticalCredentialHandleFactory));
        }

        /// <summary>
        /// <inheritdoc cref="INativeInterop.CredRead"/>
        /// </summary>
        public int CredRead(string target, CredentialType type, int reservedFlag, out Credential? credential)
        {
            if (NativeMethods.CredRead(target, type, reservedFlag, out var credentialPtr))
            {
                using var handle = _criticalCredentialHandleFactory.Build(credentialPtr);
                if (handle.IsValid && handle.NativeCredential != null)
                    credential = handle.NativeCredential;
                else
                    credential = null;

                return 0;
            }
            else
            {
                credential = null;
                return Marshal.GetLastWin32Error();
            }
        }

        /// <summary>
        /// <inheritdoc cref="INativeInterop.CredWrite"/>
        /// </summary>
        public int CredWrite(Credential credential, int flags)
        {
            var nativeCredentialPtr = IntPtr.Zero;
            try
            {
                nativeCredentialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(credential));
                Marshal.StructureToPtr(credential, nativeCredentialPtr, false);

                return NativeMethods.CredWrite(nativeCredentialPtr, flags)
                    ? 0
                    : Marshal.GetLastWin32Error();
            }
            finally
            {
                if (nativeCredentialPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(nativeCredentialPtr);
            }
        }

        /// <summary>
        /// <inheritdoc cref="INativeInterop.CredDelete"/>
        /// </summary>
        public int CredDelete(string target, int type, int flags) =>
            NativeMethods.CredDelete(target, type, flags)
                ? 0
                : Marshal.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="INativeInterop.CredEnumerate"/>
        /// </summary>
        public IEnumerable<Credential> CredEnumerate(string? filter, int flag)
        {
            if (!NativeMethods.CredEnumerate(filter, flag, out int count, out IntPtr credentialsPtr))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var nextPtr = IntPtr.Add(credentialsPtr, IntPtr.Size * i);
                    var currentPtr = Marshal.ReadIntPtr(nextPtr);
                    var nativeCredential = Marshal.PtrToStructure<Credential>(currentPtr);
                    if (nativeCredential == null)
                        continue;

                    Credential copy = nativeCredential;
                    yield return copy;
                }
            }
            finally
            {
                NativeMethods.CredFree(credentialsPtr);
            }
        }

        /// <summary>
        /// <inheritdoc cref="INativeInterop.CredFree"/>
        /// </summary>
        public int CredFree(IntPtr handle) =>
            NativeMethods.CredFree(handle)
                ? 0
                : Marshal.GetLastWin32Error();
    }
}
