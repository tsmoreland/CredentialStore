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
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public class CredentialTest
    {
        private string _id = string.Empty;
        private string _username = string.Empty;
        private string _secret = string.Empty;

        #region Setup
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // nothing to do yet
        }

        [SetUp]
        public void Setup()
        {
            _id = $"id-{Guid.NewGuid():N}";
            _username = $"user-{Guid.NewGuid():N}";
            _secret = $"secret-{Guid.NewGuid():N}";
        }
        #endregion

        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenIdIsNull()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(null!, _username, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPeristence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShouldNot_ThrowArgumentException_WhenUserNameIsNull()
        {
            Assert.DoesNotThrow(() => _ = new Credential(_id, null!, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPeristence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShouldNot_ThrowArgumentException_WhenCharacteristicsAreNone()
        {
            Assert.DoesNotThrow(() => _ = new Credential(_id, _username, _secret, CredentialFlag.None, CredentialType.DomainPassword, CredentialPeristence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenTypeIsUnknown()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(_id, _username, _secret, CredentialFlag.PromptNow, CredentialType.Unknown, CredentialPeristence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenPersistentTypeIsUnknown()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(_id, _username, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPeristence.Unknown, DateTime.Now));
        }

        #region Teardown
        [TearDown]
        public void TearDown()
        {
            // nothing to do yet
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // nothing to do yet
        }
        #endregion
    }
}
