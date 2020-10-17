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
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using NativeCredential = Moreland.Security.Win32.CredentialStore.NativeApi.Credential;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    public sealed class TestData : IDisposable
    {
        private static readonly RandomNumberGenerator _generator = RandomNumberGenerator.Create();

        public TestData(CredentialType type, bool includesTarget)
        {
            Target = $"{Guid.NewGuid():N}@{Guid.NewGuid():N}";
            CredentialType = CredentialType.Generic;
            Secret = $"secret-${Guid.NewGuid():N}-secret";
            var credentials = new List<NativeApi.Credential>
            {
                BuildRandomNativeCredential(CredentialFlag.None, type, CredentialPersistence.LocalMachine),
                BuildRandomNativeCredential(CredentialFlag.None, type, CredentialPersistence.Enterprise),
                BuildRandomNativeCredential(CredentialFlag.None, type, CredentialPersistence.Session),
                BuildRandomNativeCredential(CredentialFlag.None, type, CredentialPersistence.LocalMachine),
                BuildRandomNativeCredential(CredentialFlag.None, type, CredentialPersistence.Enterprise),
            };
            if (includesTarget)
                credentials.Add(BuildRandomNativeCredential(Target, Secret, CredentialFlag.None,
                    CredentialType,
                    CredentialPersistence.LocalMachine));
            Credentials = credentials;
            IncludesTarget = includesTarget;
        }

        public static string GetRandomString() => Guid.NewGuid().ToString("N");

        public static TEnum GetRandomEnum<TEnum>(params TEnum[] exclude) where TEnum : struct, IComparable
        {
            var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToArray();
            TEnum value;

            do
            {
                int index = Math.Abs(GetRandomInteger()) % values.Length;
                value = values[index];
            } while (exclude.Contains(value));

            return value;
        }
        public static int GetRandomInteger(params int[] exclude)
        {
            var buffer = new byte[Marshal.SizeOf<int>()];
            int value;
            do
            {
                _generator.GetBytes(buffer);
                value = BitConverter.ToInt32(buffer);

            } while (exclude.Contains(value));

            return value;
        }
        public static bool GetRandomBool() =>
            GetRandomInteger() % 2 == 0;

        public string Target { get; }
        public string Secret { get; }
        public CredentialType CredentialType { get; }
        public IEnumerable<NativeApi.Credential> Credentials { get; }
        public bool IncludesTarget { get; }


        public static NativeCredential BuildRandomNativeCredential() =>
            new NativeCredential 
            { 
                TargetName = GetRandomString(),
                Type = (int)GetRandomEnum(CredentialType.Unknown),
                Comment = GetRandomString(),
                CredentialBlob = IntPtr.Zero,
                CredentialBlobSize = 0,
                Persist = (int)GetRandomEnum(CredentialPersistence.Unknown),
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                TargetAlias = null,
                UserName = GetRandomString(),
            };
        public static IEnumerable<NativeCredential> BuildRandomNativeCredentials() =>
            Enumerable.Range(0, Math.Abs(GetRandomInteger()) % 10)
                .Select(i => BuildRandomNativeCredential());

        public static Credential BuildRandomCredential() =>
            BuildRandomCredential(GetRandomEnum<CredentialFlag>(), GetRandomEnum(CredentialType.Unknown),
                GetRandomEnum(CredentialPersistence.Unknown));
        public static Credential BuildRandomCredential(CredentialFlag flags, CredentialType type,
            CredentialPersistence persistanceType) =>
            new Credential($"{Guid.NewGuid():N}@{Guid.NewGuid():N}", $"user-{Guid.NewGuid():N}",
                $"secret-{Guid.NewGuid():N}", flags, type, persistanceType, DateTime.Now);

        public static IEnumerable<Credential> BuildRandomCredentials() =>
            Enumerable.Range(0, Math.Abs(GetRandomInteger()) % 10)
                .Select(i => BuildRandomCredential());

        private static NativeApi.Credential BuildRandomNativeCredential(CredentialFlag flags, CredentialType type,
            CredentialPersistence persistanceType) =>
            BuildRandomNativeCredential($"{Guid.NewGuid():N}@{Guid.NewGuid():N}", flags, type, persistanceType);

        private static NativeApi.Credential BuildRandomNativeCredential(string target, CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType) =>
            BuildRandomNativeCredential(target, $"{Guid.NewGuid():N}", flags, type, persistanceType);

        private static NativeApi.Credential BuildRandomNativeCredential(string target, string secret,  CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType)
        {
            var credentialBlob = Marshal.StringToCoTaskMemUni(secret);
            int credentialSize = Encoding.Unicode.GetBytes(secret).Length;

            var credential = new NativeApi.Credential
            {
                Flags = (int)flags,
                Type = (int)type,
                TargetName = target,
                Comment = string.Empty,
                LastWritten = new FILETIME {dwHighDateTime = 0, dwLowDateTime = 0},
                CredentialBlobSize = credentialSize,
                CredentialBlob = credentialBlob,
                Persist = (int)persistanceType,
                UserName = $"{Guid.NewGuid():N}",
            };
            return credential;
        }


        #region IDisposable

        ///<summary>Finalize</summary>
        ~TestData() => Dispose(false);

        ///<summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///<summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        ///<param name="disposing">if <c>true</c> then release managed resources in addition to unmanaged</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // nothing to dispose
            }

            foreach (var credential in Credentials)
            {
                Marshal.FreeHGlobal(credential.CredentialBlob);
                credential.CredentialBlob = IntPtr.Zero;
            }
        }

        #endregion

        
    }
}
