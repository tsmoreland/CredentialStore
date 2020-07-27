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

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Wrapper around static extern methods to allow Mocking and testing
    /// </summary>
    public class NativeCredentialApi : INativeCredentialApi
    {
        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredRead(string, CredentialType, int, out IntPtr)"/>
        /// </summary>
        public bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr) =>
            NativeApi.CredentialApi.CredRead(target, type, reservedFlag, out credentialPtr);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredWrite(IntPtr, int)"/>
        /// </summary>
        public bool CredWrite(IntPtr userCredential, int flags) =>
            NativeApi.CredentialApi.CredWrite(userCredential, flags);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredFree(IntPtr)"/>
        /// </summary>
        public bool CredFree([In] IntPtr cred) =>
            NativeApi.CredentialApi.CredFree(cred);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredDelete("/>
        /// </summary>
        public bool CredDelete(string target, int type, int flags) =>
            NativeApi.CredentialApi.CredDelete(target, type, flags);

        /// <summary>
        /// <see cref="NativeApi.CredentialApi.CredEnumerate(string?, int, out int, out IntPtr)"/>
        /// </summary>
        public bool CredEnumerate(string? filter, int flag, out int count, out IntPtr credentialsPtr) =>
            NativeApi.CredentialApi.CredEnumerate(filter, flag, out count, out credentialsPtr);
    }
}
