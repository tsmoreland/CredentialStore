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

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    public sealed class TestData 
    {
        public TestData(CredentialType type, bool includeKnownTarget)
        {
            KnownTarget = $"{Guid.NewGuid():N}@{Guid.NewGuid():N}";
            KnownCredentialType = CredentialType.Generic;
            var credentials = new List<Credential>
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
        public IEnumerable<Credential> Credentials { get; }

        private static Credential BuildRandomCredential(CredentialFlag flags, CredentialType type,
            CredentialPeristence persistanceType) =>
            BuildRandomCredential($"{Guid.NewGuid():N}@{Guid.NewGuid():N}", flags, type, persistanceType);
        private static Credential BuildRandomCredential(string target, CredentialFlag flags, CredentialType type, CredentialPeristence persistanceType) =>
            new Credential($"{target}", $"{Guid.NewGuid():N}", $"{Guid.NewGuid():N}", flags, type, persistanceType, DateTime.Now);

    }
}
