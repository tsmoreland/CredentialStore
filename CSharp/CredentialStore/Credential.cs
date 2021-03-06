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
    public sealed class Credential : IEquatable<Credential>
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

                Secret = Marshal.PtrToStringUni(credential.CredentialBlob, (credential.CredentialBlobSize / sizeof(char))) ?? string.Empty;
            }

            Characteristics = Enum.IsDefined(typeof(CredentialFlag), credential.Flags)
                ? (CredentialFlag)credential.Flags
                : CredentialFlag.None;
            Type = Enum.IsDefined(typeof(CredentialType), credential.Type)
                ? (CredentialType)credential.Type
                : CredentialType.Unknown;
            PersistenceType = Enum.IsDefined(typeof(CredentialPersistence), credential.Persist)
                ? (CredentialPersistence)credential.Persist
                : CredentialPersistence.Unknown;

            long highBits = credential.LastWritten.dwHighDateTime;
            highBits <<= 32;
            LastUpdated = DateTime.FromFileTime(highBits + (uint)credential.LastWritten.dwLowDateTime);

            if (GetInvalidArgumentNameOrEmpty() is not { Length: 0})
                throw new ArgumentException("invalid settings", nameof(credential));
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="Credential"/> class populated with the provided values
        /// </summary>
        /// <param name="id">unique id, maps to Target Name within Native Api</param>
        /// <param name="username">username</param>
        /// <param name="secret">secret to be stored securely by Native API</param>
        /// <param name="characteristics">typically <see cref="CredentialFlag.None"/>, see Win32 CREDENTIAL type for more detail</param>
        /// <param name="type"><see cref="CredentialType"/></param>
        /// <param name="persistenceType"><see cref="CredentialPersistence"/></param>
        /// <param name="lastUpdated">last updated value, not used for newly saved values</param>
        /// <exception cref="ArgumentException">
        /// if <paramref name="id"/> is empty; or
        /// if <paramref name="type"/> is <see cref="CredentialType.Unknown"/>; or
        /// if <paramref name="persistenceType"/> is <see cref="CredentialPersistence.Unknown"/>
        /// </exception>
        public Credential(string id, string username, string secret, CredentialFlag characteristics, CredentialType type, CredentialPersistence persistenceType, DateTime lastUpdated)
        {
            Id = id;
            UserName = username ?? string.Empty;
            Secret = secret ?? string.Empty;
            Characteristics = characteristics;
            Type = type;
            PersistenceType = persistenceType;
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
        /// <see cref="CredentialPersistence"/>
        /// </summary>
        public CredentialPersistence PersistenceType { get; }
        /// <summary>
        /// Last Updated time, only valid for reads
        /// </summary>
        public DateTime LastUpdated { get; }

        /// <summary>
        /// Deconstructs the object into <paramref name="id"/>, <paramref name="username"/>,
        /// and <paramref name="secret"/>
        /// </summary>
        public void Deconstruct(out string id, out string username, out string secret)
        {
            id = Id;
            username = UserName;
            secret = Secret;
        }
        /// <summary>
        /// Returns a copy of the current object populated with the provided values if non-null;
        /// otherwise values from the current instance
        /// </summary>
        /// <exception cref="ArgumentException">
        /// thrown by the constructor under the same conditions defined there
        /// </exception>
        public Credential With(string? id = null, string? username = null, 
            string? secret = null, CredentialFlag? characteristics = null, 
            CredentialType? type = null, 
            CredentialPersistence? persistenceType = null, 
            DateTime? lastUpdated = null) =>
            new Credential(id ?? Id, username ?? UserName, secret ?? Secret,
                characteristics ?? Characteristics, type ?? Type,
                persistenceType ?? PersistenceType, lastUpdated ?? LastUpdated);

        /// <summary>
        /// <inheritdoc cref="Object.ToString"/>
        /// </summary>
        public override string ToString() =>
            $"{Id}: {UserName} {Type}";

        /// <summary>
        /// <inheritdoc cref="IEquatable{Credential}.Equals(Credential)"/>
        /// </summary>
        public bool Equals(Credential? other) =>
            !(other is null) &&
            Id == other.Id && 
            UserName == other.UserName && 
            Type == other.Type && 
            PersistenceType == other.PersistenceType;

        /// <summary>
        /// <inheritdoc cref="object.Equals(object?)"/>
        /// </summary>
        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is Credential other && Equals(other);

        /// <summary>
        /// <inheritdoc cref="object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode()
        {
            #if !NETSTANDARD2_0
            return HashCode.Combine(Id, UserName, (int)Type, (int)PersistenceType);
            #else
            unchecked
            {
                int hashCode = 0;
                hashCode ^= (Id?.GetHashCode() ?? 0) * 397;
                hashCode ^= (UserName?.GetHashCode() ?? 0) * 397;
                hashCode ^= Type.GetHashCode() * 397;
                hashCode ^= PersistenceType.GetHashCode() * 397;
                return hashCode;
            }
            #endif
        }

        private string GetInvalidArgumentNameOrEmpty()
        {
            if (string.IsNullOrEmpty(Id))
                return nameof(Id);
            if (Type == CredentialType.Unknown)
                return nameof(Type);
            return PersistenceType == CredentialPersistence.Unknown
                ? nameof(PersistenceType)
                : string.Empty;
        }
    }
}
