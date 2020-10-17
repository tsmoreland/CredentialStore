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

        [SetUp]
        public void Setup()
        {
            _id = $"id-{Guid.NewGuid():N}";
            _username = $"user-{Guid.NewGuid():N}";
            _secret = $"secret-{Guid.NewGuid():N}";
        }

        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenIdIsNull()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(null!, _username, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShouldNot_ThrowArgumentException_WhenUserNameIsNull()
        {
            Assert.DoesNotThrow(() => _ = new Credential(_id, null!, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShouldNot_ThrowArgumentException_WhenCharacteristicsAreNone()
        {
            Assert.DoesNotThrow(() => _ = new Credential(_id, _username, _secret, CredentialFlag.None, CredentialType.DomainPassword, CredentialPersistence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenTypeIsUnknown()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(_id, _username, _secret, CredentialFlag.PromptNow, CredentialType.Unknown, CredentialPersistence.LocalMachine, DateTime.Now));
        }
        [Test]
        public void ConstructorShould_ThrowArgumentException_WhenPersistentTypeIsUnknown()
        {
            Assert.Throws<ArgumentException>(() => _ = new Credential(_id, _username, _secret, CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Unknown, DateTime.Now));
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void DeconstructShould_ReturnProvidedValues(CredentialFlag flags, CredentialType type, CredentialPersistence persistanceType)
        {
            var credential = TestData.BuildRandomCredential(flags, type, persistanceType);

            var (id, username, secret) = credential;

            Assert.That(credential.Id, Is.EqualTo(id));
            Assert.That(credential.UserName, Is.EqualTo(username));
            Assert.That(credential.Secret, Is.EqualTo(secret));
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void WithShould_UpdateWithProvidedValues(CredentialFlag flags, CredentialType type, CredentialPersistence persistanceType)
        {
            var first = TestData.BuildRandomCredential(flags, type, persistanceType);
            var (id, _, _) = TestData.BuildRandomCredential(flags, type, persistanceType);

            var (updatedId, updatedUsername, _) = first.With(id);

            Assert.That(updatedId, Is.EqualTo(id));
            Assert.That(updatedUsername, Is.EqualTo(first.UserName));
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void EqualsShould_ReturnTrueForEqualIdUserTypeAndPersistenceType(CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType)
        {
            var first = TestData.BuildRandomCredential(flags, type, persistanceType);
            var second = TestData.BuildRandomCredential(flags, type, persistanceType);

            Assert.That(first.Equals(second), Is.False);
            Assert.That(first.Equals((object)second), Is.False);
            Assert.That(first.Secret.Equals(second.Secret), Is.False);

            first = first.With(_id, _username);
            second = second.With(_id, _username);

            Assert.That(first.Equals(second), Is.True);
            Assert.That(first.Equals((object)second), Is.True);

            Assert.That(first.Equals((object?)first), Is.True);
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void EqualsShould_ReturnFalseForNull(CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType)
        {
            var first = TestData.BuildRandomCredential(flags, type, persistanceType);

            Assert.That(first.Equals(null!), Is.False);
            Assert.That(first!.Equals((object?)null!), Is.False); // ! is to silence warning, this should never be null here
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void GetHashCodeShould_ReturnSameValueForEqualObjects(CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType)
        {
            var first = TestData.BuildRandomCredential(flags, type, persistanceType);
            var second = TestData.BuildRandomCredential(flags, type, persistanceType);
            first = first!.With(_id, _username);
            second = second.With(_id, _username);

            int firstHashCode = first.GetHashCode();
            int secondHashCode = second.GetHashCode();

            Assert.That(firstHashCode, Is.EqualTo(secondHashCode));
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPersistence.LocalMachine)]
        [TestCase(CredentialFlag.PromptNow, CredentialType.DomainPassword, CredentialPersistence.Enterprise)]
        public void GetHashCodeShould_ReturnDifferentValueForNonEqualObjects(CredentialFlag flags,
            CredentialType type, CredentialPersistence persistanceType)
        {
            var first = TestData.BuildRandomCredential(flags, type, persistanceType);
            var second = TestData.BuildRandomCredential(flags, type, persistanceType);

            int firstHashCode = first.GetHashCode();
            int secondHashCode = second.GetHashCode();

            Assert.That(firstHashCode, Is.Not.EqualTo(secondHashCode));
        }
    }
}
