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
using System.Runtime.InteropServices.ComTypes;

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
        public int Flags = 0;

        /// <summary>
        /// The type of the credential. This member cannot be changed after 
        /// the credential is created. The following values are valid.
        /// <see cref="CredentialType"/> for possible values
        /// </summary>
        public int Type;

        /// <summary>
        /// The name of the credential. The <see cref="TargetName"/> and 
        /// <see cref="Type"/> members uniquely identify the credential. 
        /// This member cannot be changed after the credential is created. 
        /// Instead, the credential with the old name should be deleted 
        /// and the credential with the new name created.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? TargetName;

        /// <summary>
        /// A string comment from the user that describes this credential. This 
        /// member cannot be longer than 
        /// <see cref="Limits.MaximumCredentialStringLength"/> characters.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? Comment;

        /// <summary>
        /// The time, in Coordinated Universal Time (Greenwich Mean Time), of 
        /// the last modification of the credential. For write operations, the 
        /// value of this member is ignored.
        /// </summary>
        public FILETIME LastWritten = new FILETIME {dwHighDateTime = 0, dwLowDateTime = 0};

        /// <summary>
        /// The size, in bytes, of the CredentialBlob member. This member 
        /// cannot be larger than <see cref="Limits.MaxCredentialBlobSize"/> bytes
        /// .</summary>
        public int CredentialBlobSize;

        /// <summary>
        /// Secret data for the credential. The CredentialBlob member can be 
        /// both read and written.
        /// </summary>
        public IntPtr CredentialBlob = IntPtr.Zero;

        /// <summary>
        /// Defines the peristence of this credential. This member can be read and written
        /// <see cref="CredentialPeristence"/> for possible values
        /// .</summary>
        public int Persist;

        /// <summary>
        /// The number of application-defined attributes to be associated with 
        /// the credential. This member can be read and written. Its value 
        /// cannot be greater than CRED_MAX_ATTRIBUTES (64).
        /// </summary>
        public int AttributeCount = 0;

        /// <summary>
        /// Application-defined attributes that are associated with the 
        /// credential. This member can be read and written.
        /// </summary>
        public IntPtr Attributes = IntPtr.Zero;

        /// <summary>
        /// Alias for the <see cref="TargetName"/>  member. This member can be 
        /// read and written. It cannot be longer than 
        /// <see cref="Limits.MaximumCredentialStringLength"/> characters.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? TargetAlias = null;

        /// <summary>
        /// The user name of the account used to connect to <see cref="TargetName"/>.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? UserName;
    }
}
