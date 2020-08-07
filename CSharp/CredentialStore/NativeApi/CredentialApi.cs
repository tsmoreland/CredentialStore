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
        /// The CredRead function reads a credential from the user's 
        /// credential set. The credential set used is the one associated with 
        /// the logon session of the current token. The token must not have the 
        /// user's SID disabled.
        /// </summary>
        /// <param name="target">
        /// Pointer to a null-terminated string that contains the name of the
        /// credential to read.
        /// </param>
        /// <param name="type">
        /// Type of the credential to read. Type must be one of the
        /// <see cref="CredentialType"/> defined types.
        /// </param>
        /// <param name="reservedFlag">Currently reserved and must be zero.</param>
        /// <param name="credentialPtr">
        /// Pointer to a single allocated block buffer to return the credential.
        /// Any pointers contained within the buffer are pointers to locations
        /// within this single allocated block. The single returned buffer must
        /// be freed by calling <see cref="CredFree"/>
        /// </param>
        /// <returns>The function returns true on success and false on failure</returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32/CredRead.html
        /// </remarks>
        /// <returns>0 on success Last Win32 error on failure</returns>
        public int CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr) =>
            NativeMethods.CredRead(target, type, reservedFlag, out credentialPtr)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// The CredWrite function creates a new credential or modifies an 
        /// existing credential in the user's credential set. The new credential 
        /// is associated with the logon session of the current token. The token 
        /// must not have the user's security identifier (SID) disabled.
        /// </summary>
        /// <param name="userCredential">
        /// A pointer to the <see cref="Credential"/> structure to be written.
        /// </param>
        /// <param name="flags">
        /// Flags that control the function's operation.
        /// <see cref="PreserveFlag"/> for valid values.
        /// </param>
        /// <returns>If the function succeeds, the function returns TRUE.</returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredWrite
        /// </remarks>
        /// <returns>0 on success Last Win32 error on failure</returns>
        public int CredWrite(IntPtr userCredential, int flags) =>
            NativeMethods.CredWrite(userCredential, flags)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// The CredFree function frees a buffer returned by any of the credentials management functions.
        /// </summary>
        /// <param name="cred">Pointer to the buffer to be freed.</param>
        /// <returns>If the function succeeds, the function returns true</returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredFree
        /// </remarks>
        /// <returns>0 on success Last Win32 error on failure</returns>
        public int CredFree([In] IntPtr cred) =>
            NativeMethods.CredFree(cred)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// The CredDelete function deletes a credential from the user's 
        /// credential set. The credential set used is the one associated with 
        /// the logon session of the current token. The token must not have the 
        /// user's SID disabled.
        /// </summary>
        /// <param name="target">
        /// Pointer to a null-terminated string that contains the name of the
        /// credential to delete.
        /// </param>
        /// <param name="type">
        /// Type of the credential to delete. Must be one of the
        /// <see cref="CredentialType"/> defined types. For a list of the
        /// defined types, see the Type member of the <see cref="Credential"/>
        /// structure.
        /// </param>
        /// <param name="flags">Reserved and must be zero.</param>
        /// <returns>The function returns true on success and false on failure</returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredDelete
        /// </remarks>
        /// <returns>0 on success Last Win32 error on failure</returns>
        public int CredDelete([MarshalAs(UnmanagedType.LPWStr)] string target, int type, int flags) =>
            NativeMethods.CredDelete(target, type, flags)
                ? 0
                : _marshalService.GetLastWin32Error();

        /// <summary>
        /// The CredEnumerate function enumerates the credentials from the
        /// user's credential set. The credential set used is the one
        /// associated with the logon session of the current token. The token
        /// must not have the user's SID disabled.
        /// </summary>
        /// <param name="filter">
        /// Pointer to a null-terminated string that contains the filter for
        /// the returned credentials. Only credentials with a TargetName
        /// matching the filter will be returned. The filter specifies a name
        /// prefix followed by an asterisk. For instance, the filter "FRED*"
        /// will return all credentials with a TargetName beginning with the
        /// string "FRED".
        /// If null is specified, all credentials will be returned.
        /// </param>
        /// <param name="flag">
        /// The value of this parameter can be zero or more of
        /// <see cref="EnumerateFlag"/> values combined with a bitwise-OR
        /// operation.
        /// </param>
        /// <param name="count"></param>
        /// <param name="credentialsPtr"></param>
        /// <returns>0 on success Last Win32 error on failure</returns>
        public int CredEnumerate([MarshalAs(UnmanagedType.LPWStr)] string? filter, int flag, out int count,
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
