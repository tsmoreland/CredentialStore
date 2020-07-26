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
    /// https://docs.microsoft.com/en-us/windows/win32/api/wincred/ns-wincred-credentiala
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    internal class Credential
    {
        /// <summary>
        /// A bit member that identifies characteristics of the credential. 
        /// Undefined bits should be initialized as zero and not otherwise 
        /// altered to permit future enhancement.
        /// <see cref="CredentialFlag"/> for possible values
        /// </summary>
        public uint Flags;

        /// <summary>
        /// The type of the credential. This member cannot be changed after 
        /// the credential is created. The following values are valid.
        /// <see cref="CredentialType"/> for possible values
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public uint Type;

        /// <summary>
        /// The name of the credential. The <see cref="TargetName"/> and 
        /// <see cref="Type"/> members uniquely identify the credential. 
        /// This member cannot be changed after the credential is created. 
        /// Instead, the credential with the old name should be deleted 
        /// and the credential with the new name created.
        /// </summary>
        public IntPtr TargetName;

        /// <summary>
        /// A string comment from the user that describes this credential. This 
        /// member cannot be longer than 
        /// <see cref="Limits.MaximumCredentialStringLength"/> characters.
        /// </summary>
        public IntPtr Comment;

        /// <summary>
        /// The time, in Coordinated Universal Time (Greenwich Mean Time), of 
        /// the last modification of the credential. For write operations, the 
        /// value of this member is ignored.
        /// </summary>
        public long LastWritten;

        /// <summary>
        /// The size, in bytes, of the CredentialBlob member. This member 
        /// cannot be larger than <see cref="Limits.MaxCredentialBlobSize"/> bytes
        /// .</summary>
        public uint CredentialBlobSize;

        /// <summary>
        /// Secret data for the credential. The CredentialBlob member can be 
        /// both read and written.
        /// </summary>
        public IntPtr CredentialBlob = IntPtr.Zero;

        /// <summary>
        /// Defines the peristence of this credential. This member can be read and written
        /// <see cref="CredentialPeristence"/> for possible values
        /// .</summary>
        public uint Persist;

        /// <summary>
        /// The number of application-defined attributes to be associated with 
        /// the credential. This member can be read and written. Its value 
        /// cannot be greater than CRED_MAX_ATTRIBUTES (64).
        /// </summary>
        public uint AttributeCount;

        /// <summary>
        /// Application-defined attributes that are associated with the 
        /// credential. This member can be read and written.
        /// </summary>
        public IntPtr Attributes;

        /// <summary>
        /// Alias for the <see cref="TargetName"/>  member. This member can be 
        /// read and written. It cannot be longer than 
        /// <see cref="Limits.MaximumCredentialStringLength"/> characters.
        /// </summary>
        public IntPtr TargetAlias;

        /// <summary>
        /// The user name of the account used to connect to <see cref="TargetName"/>.
        /// </summary>
        public IntPtr UserName;

        /// <summary>
        /// The CredRead function reads a credential from the user's 
        /// credential set. The credential set used is the one associated with 
        /// the logon session of the current token. The token must not have the 
        /// user's SID disabled.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="reservedFlag"></param>
        /// <param name="CredentialPtr"></param>
        /// <returns></returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32/CredRead.html
        /// </remarks>
        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr CredentialPtr);

        /// <summary>
        /// The CredWrite function creates a new credential or modifies an 
        /// existing credential in the user's credential set. The new credential 
        /// is associated with the logon session of the current token. The token 
        /// must not have the user's security identifier (SID) disabled.
        /// </summary>
        /// <param name="userCredential"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredWrite
        /// </remarks>
        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredWrite([In] ref Credential userCredential, [In] UInt32 flags);

        /// <summary>
        /// The CredFree function frees a buffer returned by any of the credentials management functions.
        /// </summary>
        /// <param name="cred"></param>
        /// <returns></returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredFree
        /// </remarks>
        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        public static extern bool CredFree([In] IntPtr cred);

        /// <summary>
        /// The CredDelete function deletes a credential from the user's 
        /// credential set. The credential set used is the one associated with 
        /// the logon session of the current token. The token must not have the 
        /// user's SID disabled.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <remarks>
        /// reference: https://www.pinvoke.net/default.aspx/advapi32.CredDelete
        /// </remarks>
        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
        private static extern bool CredDelete(string target, CredentialType type, int flags);
    }
}
