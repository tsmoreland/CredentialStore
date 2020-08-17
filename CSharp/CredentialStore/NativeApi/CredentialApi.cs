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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <inheritdoc cref="ICredentialApi"/>
    /// </summary>
    internal sealed class CredentialApi : ICredentialApi
    {
        private readonly IMarshalService _marshalService;

        /// <summary>
        /// Instantiates a new instance of the marshalService class.
        /// </summary>
        /// <param name="marshalService">
        /// used to retrieve last win32 error
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="marshalService"/> is null
        /// </exception>
        public CredentialApi(IMarshalService marshalService)
        {
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
        }

        /// <summary>
        /// <inheritdoc cref="ICredentialApi.CredRead"/>
        /// </summary>
        public int CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr) =>
            NativeMethods.CredRead(target, type, reservedFlag, out credentialPtr)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="ICredentialApi.CredWrite"/>
        /// </summary>
        public int CredWrite(IntPtr userCredential, int flags) =>
            NativeMethods.CredWrite(userCredential, flags)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="ICredentialApi.CredFree"/>
        /// </summary>
        public int CredFree(IntPtr cred) =>
            NativeMethods.CredFree(cred)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="ICredentialApi.CredDelete"/>
        /// </summary>
        public int CredDelete(string target, int type, int flags) =>
            NativeMethods.CredDelete(target, type, flags)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="ICredentialApi.CredEnumerate"/>
        /// </summary>
        public int CredEnumerate(string? filter, int flag, out int count,
            out IntPtr credentialsPtr) =>
            NativeMethods.CredEnumerate(filter, flag, out count, out credentialsPtr)
                ? 0
                : _marshalService.GetLastWin32Error();

        private static class NativeMethods
        {
            [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredRead([MarshalAs(UnmanagedType.LPWStr)] string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

            [DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "CredWriteW", CharSet = CharSet.Unicode)]
            public static extern bool CredWrite(IntPtr userCredential, int flags);

            [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
            public static extern bool CredFree([In] IntPtr cred);

            [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredDelete([MarshalAs(UnmanagedType.LPWStr)] string target, int type, int flags);

            [DllImport("advapi32", EntryPoint = "CredEnumerateW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredEnumerate([MarshalAs(UnmanagedType.LPWStr)] string? filter, int flag, out int count, out IntPtr credentialsPtr);
        }
    }
}
