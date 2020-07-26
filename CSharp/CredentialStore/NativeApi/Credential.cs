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
        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

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
        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredWrite([In] ref Credential userCredential, [In] UInt32 flags);

        /// <summary>
        /// The CredFree function frees a buffer returned by any of the credentials management functions.
        /// </summary>
        /// <param name="cred">Pointer to the buffer to be freed.</param>
        /// <returns>If the function succeeds, the function returns true</returns>
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
        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
        public static extern bool CredDelete(string target, CredentialType type, int flags);

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
        /// <returns></returns>
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredEnumerate(string? filter, int flag, out int count, out IntPtr credentialsPtr);
    }
}
