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
    [TestFixture]
    public sealed class CredentialManagerTest
    {
        private Mock<ILoggerAdapter> _logger = null!;
        private Mock<INativeCredentialApi> _nativeCredentialApi = null!;
        private CredentialManager _manager = null!;
        private TestData? _dataSource;

        [SetUp]
        public void Setup()
        {
            _nativeCredentialApi = new Mock<INativeCredentialApi>();
            _logger = new Mock<ILoggerAdapter>();
            _manager = new CredentialManager(_nativeCredentialApi.Object, _logger.Object);
            _targetDeleted = false;
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

        [Test]
        [TestCaseSource(typeof(TestDataSource), nameof(TestDataSource.Source))]
        public void FindShould_ReturnCredentialIfExists(TestData data)
        {
            InitializeFromTestData(data);
            var credential = _manager.Find(data.Target, data.CredentialType);

            if (data.IncludesTarget)
            {
                Assert.That(credential, Is.Not.Null);
                if (credential == null)
                    return; // to prevent warning of possible use of null
                Assert.That(credential.Id, Is.EqualTo(data.Target));
                Assert.That(credential.Secret, Is.EqualTo(data.Secret));
            }
            else
                Assert.That(credential, Is.Null);
        }

        [Test]
        [TestCaseSource(typeof(TestDataSource), nameof(TestDataSource.SourceWhereTargetIsIncluded))]
        public void DeleteShould_DeleteExistingValue_WhenGivenCredential(TestData data)
        {
            InitializeFromTestData(data);
            var beforeDelete = _manager.Find(data.Target, data.CredentialType);
            Assert.That(beforeDelete, Is.Not.Null);
            if (beforeDelete == null)
                return;

            var deleted = _manager.Delete(beforeDelete);
            var afterDelete = _manager.Find(data.Target, data.CredentialType);

            Assert.That(deleted, Is.True);
            Assert.That(afterDelete, Is.Null);
        }

        [Test]
        [TestCaseSource(typeof(TestDataSource), nameof(TestDataSource.SourceWhereTargetIsIncluded))]
        public void DeleteShould_DeleteExistingValue_WhenGivenIdAndType(TestData data)
        {
            InitializeFromTestData(data);
            var beforeDelete = _manager.Find(data.Target, data.CredentialType);
            Assert.That(beforeDelete, Is.Not.Null);
            if (beforeDelete == null)
                return;

            var deleted = _manager.Delete(data.Target, data.CredentialType);
            var afterDelete = _manager.Find(data.Target, data.CredentialType);

            Assert.That(deleted, Is.True);
            Assert.That(afterDelete, Is.Null);
        }

        [Test]
        public void DeleteShould_ThrowArgumentNullExceptionIfCredentialIsNull()
        {
            Credential credential = null!;
            Assert.Throws<ArgumentNullException>(() => _manager.Delete(credential));
        }

        [Test]
        [TestCase(null!)]
        [TestCase("")]
        public void DeleteShould_ThrowArgumentExceptionIfIdIsNullOrEmpty(string id)
        {
            Assert.Throws<ArgumentException>(() => _manager.Delete(id, CredentialType.Generic));
        }

        [Test]
        [TestCaseSource(typeof(TestDataSource), nameof(TestDataSource.Source))]
        public void CredentialsShould_ReturnAllCredentials(TestData data)
        {
            InitializeFromTestData(data);
            var credentials = _manager.Credentials;

            CollectionAssert.AreEquivalent(
                data.Credentials.Select(c => c.TargetName),
                credentials.Select(c => c.Id));
        }

        [TearDown]
        public void TearDown()
        {
            _dataSource?.Dispose();
            _dataSource = null!;
        }

        private bool _targetDeleted;
        private void InitializeFromTestData(TestData dataSource)
        {
            _dataSource = dataSource;
            _nativeCredentialApi
                .Setup(native => native.CredRead(_dataSource.Target, It.IsAny<CredentialType>(), 0))
                .Returns(() =>
                {
                    if (_targetDeleted)
                        return null;

                    IEnumerable<NativeApi.Credential?> credentials = _dataSource.Credentials;
                    return credentials.DefaultIfEmpty(null!)
                        .FirstOrDefault(c => c?.TargetName == _dataSource.Target);
                });
            _nativeCredentialApi
                .Setup(native => native.CredEnumerate(It.IsAny<string?>(), It.IsAny<int>()))
                .Returns(() => _dataSource.Credentials);
            _nativeCredentialApi
                .Setup(native => native.CredDelete(_dataSource.Target, (int)_dataSource.CredentialType, It.IsAny<int>()))
                .Returns(() => true);
            _nativeCredentialApi
                .Setup(native =>
                    native.CredDelete(_dataSource.Target, (int)_dataSource.CredentialType, It.IsAny<int>()))
                .Callback<string, int, int>((target, type, flag) => _targetDeleted = true)
                .Returns(true);
        }
    }
}
