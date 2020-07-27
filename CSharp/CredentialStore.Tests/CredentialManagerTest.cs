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

using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CredentialManagerTest 
    {
        private Mock<ILoggerAdapter> _logger = null!;
        private Credential _tempCredential = null!;
        private CredentialManager _manager = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
#           if DEBUG
            var logger = new Mock<ILoggerAdapter>();
            var credentialManager = new CredentialManager(logger.Object);

            _tempCredential = new Credential(
                "tsmoreland@credential:test",
                "username",
                "password",
                CredentialFlag.None,
                CredentialType.Generic,
                CredentialPeristence.LocalMachine,
                DateTime.MinValue);

            credentialManager.Add(_tempCredential);
#           endif
        }

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggerAdapter>();
            _manager = new CredentialManager(_logger.Object);
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNull_WhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(null!));
        }

#       if DEBUG
        [Test]
        public void FindShould_ReturnMatchingValue()
        {
            const string id = "target-54d456e0bae74269b167f031515b7761";
            const CredentialType type = CredentialType.Generic;

            var credential = _manager.Find(id, type);

            Assert.That(credential, Is.Not.Null);
            if (credential == null)
                return;

            var newCredential = credential.With(id: credential.Id.Replace("target", "tgt"));
            bool saved = _manager.Add(newCredential);
            if (!saved)
            {
                // ...
            }

        }
#       endif

        [Test]
        public void ConstructorShouldNot_ThrowException_WhenSetupIsValid()
        {
            Assert.DoesNotThrow(() => _ = new CredentialManager(_logger.Object));
        }

#       if DEBUG
        [Test]
        public void CredentialsShould_ReturnAtLeastOneEntry()
        {
            var credentials = _manager.Credentials.ToArray();

            Assert.That(credentials, Is.Not.Null);
            Assert.That(credentials.Length, Is.AtLeast(1));
        }
#       endif

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
#           if DEBUG
            var logger = new Mock<ILoggerAdapter>();
            var credentialManager = new CredentialManager(logger.Object);

            credentialManager.Delete(_tempCredential);
#           endif
        }
    }
}
