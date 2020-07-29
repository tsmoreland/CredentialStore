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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    public sealed class TestData : IDisposable
    {
        public TestData(CredentialType type, bool includeKnownTarget)
        {
            KnownTarget = $"{Guid.NewGuid():N}@{Guid.NewGuid():N}";
            KnownCredentialType = CredentialType.Generic;
            var credentials = new List<NativeApi.Credential>
            {
                BuildRandomCredential(CredentialFlag.None, type, CredentialPeristence.LocalMachine),
                BuildRandomCredential(CredentialFlag.None, type, CredentialPeristence.Enterprise),
                BuildRandomCredential(CredentialFlag.None, type, CredentialPeristence.Session),
                BuildRandomCredential(CredentialFlag.None, type, CredentialPeristence.LocalMachine),
                BuildRandomCredential(CredentialFlag.None, type, CredentialPeristence.Enterprise),
            };
            if (includeKnownTarget)
                credentials.Add(BuildRandomCredential(KnownTarget, CredentialFlag.None, KnownCredentialType,
                    CredentialPeristence.LocalMachine));

            Credentials = credentials;
        }

        public string KnownTarget { get; }
        public CredentialType KnownCredentialType { get; }
        public IEnumerable<NativeApi.Credential> Credentials { get; }

        private static NativeApi.Credential BuildRandomCredential(CredentialFlag flags, CredentialType type,
            CredentialPeristence persistanceType) =>
            BuildRandomCredential($"{Guid.NewGuid():N}@{Guid.NewGuid():N}", flags, type, persistanceType);

        private static NativeApi.Credential BuildRandomCredential(string target, CredentialFlag flags,
            CredentialType type, CredentialPeristence persistanceType)
        {
            string secret = $"{Guid.NewGuid():N}";

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
