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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Credential Entity as populated by Win32 api
    /// </summary>
    /// <remarks>
    /// this entity does not store any attributes from the native credential, 
    /// any persist of this object will result in the loss of that data
    /// </remarks>
    [DebuggerDisplay("{Id}: {UserName} {Type}")]
    public sealed class Credential 
    {
        internal Credential(NativeApi.Credential credential)
        {
            Id = credential.TargetName ?? string.Empty;
            UserName = credential.UserName ?? string.Empty;
            Secret = string.Empty;
            if (credential.CredentialBlob != IntPtr.Zero && credential.CredentialBlobSize > 0)
            {
                if (credential.CredentialBlobSize > NativeApi.Limits.MaxCredentialBlobSize)
                {
                    // .. log this .. it should be at most 512 but i've had ones come back higher
                }

                if (credential.CredentialBlobSize > (uint)int.MaxValue)
                    throw new ArgumentException($"secret is length greater than maximum supported value {int.MaxValue / sizeof(char)}");

                Secret = Marshal.PtrToStringUni(credential.CredentialBlob, (credential.CredentialBlobSize / sizeof(char))) ?? string.Empty;
            }

            Characteristics = Enum.IsDefined(typeof(CredentialFlag), credential.Flags)
                ? (CredentialFlag)credential.Flags
                : CredentialFlag.None;
            Type = Enum.IsDefined(typeof(CredentialType), credential.Type)
                ? (CredentialType)credential.Type
                : CredentialType.Unknown;
            PeristenceType = Enum.IsDefined(typeof(CredentialPeristence), credential.Persist)
                ? (CredentialPeristence)credential.Persist
                : CredentialPeristence.Unknown;

            long highBits = credential.LastWritten.dwHighDateTime;
            highBits <<= 32;
            LastUpdated = DateTime.FromFileTime(highBits + (long)(uint)credential.LastWritten.dwLowDateTime);

            if (!string.IsNullOrEmpty(GetInvalidArgumentNameOrEmpty()))
                throw new ArgumentException("invalid settings", nameof(credential));
        }

        public Credential(string id, string username, string secret, CredentialFlag characteristics, CredentialType type, CredentialPeristence persistanceType, DateTime lastUpdated)
        {
            Id = id;
            UserName = username ?? string.Empty;
            Secret = secret ?? string.Empty;
            Characteristics = characteristics;
            Type = type;
            PeristenceType = persistanceType;
            LastUpdated = lastUpdated;

            var invalidPropertyName = GetInvalidArgumentNameOrEmpty();
            if (!string.IsNullOrEmpty(invalidPropertyName))
                throw new ArgumentException("one or more parameters have invalid values", invalidPropertyName);
        }

        /// <summary>
        /// Unqiue Identifier, this represents the target name of the credential
        /// </summary>
        public string Id { get; } 

        /// <summary>
        /// The user name of the account used to connect to <see cref="Id"/> 
        /// </summary>
        /// <remarks>
        /// If the credential Type is <see cref="CredentialType.DomainPassword"/>, 
        /// this member can be either a DomainName/UserName or a UPN.
        /// </remarks>
        public string UserName { get; }
        /// <summary>
        /// Secret (password)
        /// </summary>
        public string Secret { get; } 

        /// <summary>
        /// <see cref="CredentialFlag"/>
        /// </summary>
        public CredentialFlag Characteristics { get; } 
        /// <summary>
        /// <see cref="CredentialType"/>
        /// </summary>
        public CredentialType Type { get; } 
        /// <summary>
        /// <see cref="CredentialType"/>
        /// </summary>
        public CredentialPeristence PeristenceType { get; }
        /// <summary>
        /// Last Updated time, only valid for reads
        /// </summary>
        public DateTime LastUpdated { get; }

        public void Deconstruct(out string id, out string username, out string secret)
        {
            id = Id;
            username = UserName;
            secret = Secret;
        }
        public Credential With(string? id = null, string? username = null, 
            string? secret = null, CredentialFlag? characteristics = null, 
            CredentialType? type = null, 
            CredentialPeristence? persistanceType = null, 
            DateTime? lastUpdated = null) =>
            new Credential(id ?? Id, username ?? UserName, secret ?? Secret,
                characteristics ?? Characteristics, type ?? Type,
                persistanceType ?? PeristenceType, lastUpdated ?? LastUpdated);

        public override string ToString() =>
            $"{Id}: {UserName} {Type}";

        private string GetInvalidArgumentNameOrEmpty()
        {
            if (string.IsNullOrEmpty(Id))
                return nameof(Id);
            if (Type == CredentialType.Unknown)
                return nameof(Type);
            return PeristenceType == CredentialPeristence.Unknown
                ? nameof(PeristenceType)
                : string.Empty;
        }
    }
}
