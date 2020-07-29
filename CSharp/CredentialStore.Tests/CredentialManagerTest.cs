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
using System.Collections.Generic;
using System.Linq;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixtureSource(typeof(TestDataSource))]
    public sealed class CredentialManagerTest 
    {
        private Mock<ILoggerAdapter> _logger = null!;
        private Mock<INativeCredentialApi> _nativeCredentialApi = null!;
        private CredentialManager _manager = null!;
        private readonly TestData _dataSource;

        public CredentialManagerTest(TestData dataSource)
        {
            _dataSource = dataSource;
        }

        [SetUp]
        public void Setup()
        {
            _nativeCredentialApi = new Mock<INativeCredentialApi>();
            _logger = new Mock<ILoggerAdapter>();
            _manager = new CredentialManager(_nativeCredentialApi.Object, _logger.Object);

            _nativeCredentialApi
                .Setup(native => native.CredRead(_dataSource.KnownTarget, It.IsAny<CredentialType>(), 0))
                .Returns(() =>
                {
                    IEnumerable<NativeApi.Credential?> credentials = _dataSource.Credentials;
                    return credentials.DefaultIfEmpty(null!)
                        .FirstOrDefault(c => c?.TargetName == _dataSource.KnownTarget);
                });
            _nativeCredentialApi
                .Setup(native => native.CredEnumerate(It.IsAny<string?>(), It.IsAny<int>()))
                .Returns(() => _dataSource.Credentials);
            _nativeCredentialApi
                .Setup(native => native.CredDelete(_dataSource.KnownTarget, (int)_dataSource.KnownCredentialType, It.IsAny<int>()))
                .Returns(() => true);
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNull_WhenNativeCredentialsApiIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(null!, _logger.Object));
        }
        [Test]
        public void ConstructorShould_ThrowArgumentNull_WhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(_nativeCredentialApi.Object, null!));
        }

        [Test]
        public void ConstructorShouldNot_ThrowException_WhenSetupIsValid()
        {
            Assert.DoesNotThrow(() => _ = new CredentialManager(_logger.Object));
        }

        /*
        [TestCase(true, ExpectedResult = true)]
        [TestCase(false, ExpectedResult = false)]
        public bool FindShould_ReturnCredentialIfExists(bool useKnownTarget)
        {
            string target = useKnownTarget
                ? _dataSource.KnownTarget
                : Guid.NewGuid().ToString();
            var type = useKnownTarget
                ? _dataSource.KnownCredentialType
                : CredentialType.DomainPassword;

            var credential = _manager.Find(target, type);

            return credential?.Id == target;
        }
        */



        /*

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

        [Test]
        public void CredentialsShould_ReturnAtLeastOneEntry()
        {
            var credentials = _manager.Credentials.ToArray();

            Assert.That(credentials, Is.Not.Null);
            Assert.That(credentials.Length, Is.AtLeast(1));
        }
        */

        private static Credential BuildRandomCredential(CredentialFlag flags, CredentialType type, CredentialPeristence persistanceType) =>
            new Credential($"{Guid.NewGuid():N}@{Guid.NewGuid():N}", $"{Guid.NewGuid():N}", $"{Guid.NewGuid():N}", flags, type, persistanceType, DateTime.Now);
    }
}
