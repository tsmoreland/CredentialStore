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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <see cref="IDisposable"/> handle to pinned structure which must be disposed or a memory leak
    /// will occur
    /// </summary>
    internal sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalCredentialHandle"/> class.
        /// </summary>
        /// <param name="handle">handle to <see cref="Credential"/></param>
        public CriticalCredentialHandle(IntPtr handle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Returns true if the underlying handle is valid
        /// </summary>
        public bool IsValid => !IsInvalid;

        /// <summary>
        /// returns <see cref="Credential"/> referenced by the handle if <see cref="IsValid"/>
        /// otherwise null
        /// </summary>
        public Credential? NativeCredential => IsValid
            ? (Credential?)Marshal.PtrToStructure(handle, typeof(Credential))
            : null;

        /// <summary>
        /// <see cref="CriticalHandleZeroOrMinusOneIsInvalid.ReleaseHandle()"/>
        /// </summary>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return true; // nothing to do

            if (!Credential.CredFree(handle))
                return false;
            SetHandleAsInvalid();
            return true;
        }
    }
}
