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
using System.ComponentModel;
using System.Linq;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CredentialManagerTest
    {
        private INativeHelper _nativeHelper = null!;
        private Mock<ILoggerAdapter> _logger = null!;
        private Mock<INativeCredentialApi> _nativeCredentialApi = null!;
        private Mock<IErrorCodeToStringService> _errorCodeToString = null!;
        private Mock<IPointerMath> _pointerMath = null!;
        private Mock<IMarshalService> _marshalService = null!;
        private CredentialManager _manager = null!;
        private TestData? _dataSource;
        private NativeApi.IntermediateCredential? _addedCredential;

        [SetUp]
        public void Setup()
        {
            _nativeCredentialApi = new Mock<INativeCredentialApi>();
            _errorCodeToString = new Mock<IErrorCodeToStringService>();
            _marshalService = new Mock<IMarshalService>();
            _pointerMath = new Mock<IPointerMath>();
            _logger = new Mock<ILoggerAdapter>();

            _nativeHelper = new NativeApi.NativeHelper(
                _pointerMath.Object,
                _marshalService.Object,
                _nativeCredentialApi.Object,
                _errorCodeToString.Object,
                _logger.Object);

            _manager = new CredentialManager(_nativeHelper, _logger.Object);
            _targetDeleted = false;
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullException_WhenNativeHelperIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(null!, _logger.Object));
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullException_WhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(_nativeHelper, null!));
        }

        [Test]
        [TestCase(false, false, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(false, true, true, true)]
        [TestCase(false, true, false, true)]
        public void ConstructorShould_ThrowArgumentException_WhenNativeHelperIsNotValid(bool includePointerMath, bool includeMarshalService, bool includeNativeCredentialsApi, bool includeErrorToStringService)
        {
            Mock<INativeHelper> nativeHelper = new Mock<INativeHelper>();
            if (includePointerMath)
                nativeHelper
                    .SetupGet(helper => helper.PointerMath)
                    .Returns(_pointerMath.Object);
            if (includeMarshalService)
                nativeHelper
                    .SetupGet(helper => helper.MarshalService)
                    .Returns(_marshalService.Object);
            if (includeNativeCredentialsApi)
                nativeHelper
                    .SetupGet(helper => helper.NativeCredentialApi)
                    .Returns(_nativeCredentialApi.Object);
            if (includeErrorToStringService)
                nativeHelper
                    .SetupGet(helper => helper.ErrorCodeToStringService)
                    .Returns(_errorCodeToString.Object);

            var ex = Assert.Throws<ArgumentException>(() => _ = new CredentialManager(nativeHelper.Object, _logger.Object));
            Assert.That(ex.ParamName, Is.EquivalentTo("nativeHelper"));
        }

        [Test]
        public void ConstructorShouldNot_ThrowException_WhenSetupIsValid()
        {
            Assert.DoesNotThrow(() => _ = new CredentialManager(_logger.Object));
        }

        [Test]
        public void AddShould_ThrowArgumentNullException_WhenCredentialIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _manager.Add(null!));
            Assert.That(exception.ParamName, Is.EqualTo("credential"));
        }

        [Test]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPeristence.LocalMachine, true)]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPeristence.LocalMachine, false)]
        [TestCase(CredentialFlag.None, CredentialType.DomainPassword, CredentialPeristence.LocalMachine, true)]
        [TestCase(CredentialFlag.None, CredentialType.DomainPassword, CredentialPeristence.LocalMachine, false)]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPeristence.Enterprise, true)]
        [TestCase(CredentialFlag.None, CredentialType.Generic, CredentialPeristence.Enterprise, false)]
        [TestCase(CredentialFlag.None, CredentialType.DomainPassword, CredentialPeristence.Enterprise, true)]
        [TestCase(CredentialFlag.None, CredentialType.DomainPassword, CredentialPeristence.Enterprise, false)]
        public void AddShould_ReturnsCredWriteResult(CredentialFlag flag, CredentialType type, CredentialPeristence persistType, bool successful)
        {
            using var data = new TestData(type, false);
            var toAdd = TestData.BuildRandomCredential(flag, type, persistType);
            InitializeFromTestData(data, toAdd, successful);

            bool added = _manager.Add(toAdd);

            Assert.That(added, Is.EqualTo(successful));
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
        [TestCase(null!)]
        [TestCase("")]
        public void FindShould_ThrowArgumentExceptionIfIdIsNullOrEmpty(string id)
        {
            var ex = Assert.Throws<ArgumentException>(() => _manager.Find(id, CredentialType.Generic));
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        [TestCase(null!, true)]
        [TestCase("", true)]
        [TestCase(null!, false)]
        [TestCase("", false)]
        public void FindUsingFilter_ShouldReturnAllCredentialsWhenIdIsNullOrEmpty(string filter, bool searchAll)
        {
            using var data = new TestData(CredentialType.Generic, false);
            InitializeFromTestData(data);

            var credentials = _manager.Find(filter, searchAll);

            CollectionAssert.AreEquivalent(
                data.Credentials.Select(c => c.TargetName),
                credentials.Select(c => c.Id));

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

            _manager.Delete(beforeDelete);
            var afterDelete = _manager.Find(data.Target, data.CredentialType);

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

            _manager.Delete(data.Target, data.CredentialType);
            var afterDelete = _manager.Find(data.Target, data.CredentialType);

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
            var ex = Assert.Throws<ArgumentException>(() => _manager.Delete(id, CredentialType.Generic));
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        [TestCase((int)NativeApi.ExpectedError.NotFound, false, CredentialType.Generic)]
        [TestCase((int)NativeApi.ExpectedError.InvalidArgument, true, CredentialType.Generic)]
        [TestCase((int)NativeApi.ExpectedError.NotFound, false, CredentialType.DomainPassword)]
        [TestCase((int)NativeApi.ExpectedError.InvalidArgument, true, CredentialType.DomainPassword)]
        public void DeleteShould_IgnoreNotFoundError(int errorCode, bool throwsException, CredentialType type)
        {
            string id = $"not-found-{Guid.NewGuid():N}";
            using var data = new TestData(CredentialType.Generic, false);
            InitializeFromTestData(data, idNotFound: id, lastErrorCode: errorCode);

            if (throwsException)
                Assert.Throws<Win32Exception>(() => _manager.Delete(id, type));
            else
                Assert.DoesNotThrow(() => _manager.Delete(id, type));
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
            _addedCredential?.Dispose();
            _addedCredential = null;
            _dataSource?.Dispose();
            _dataSource = null!;
        }

        private bool _targetDeleted;
        private void InitializeFromTestData(TestData dataSource, Credential? toAdd = null, bool successfulAdd = true, string idNotFound = null!, int? lastErrorCode = null)
        {
            if (toAdd != null)
                _addedCredential = new NativeApi.IntermediateCredential(toAdd);

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
            if (_addedCredential != null && toAdd != null)
                _nativeCredentialApi
                    .Setup(native => native.CredRead(toAdd.Id, toAdd.Type, 0))
                    .Returns(() => _addedCredential.NativeCredential);
            _nativeCredentialApi
                .Setup(native => native.CredEnumerate(It.IsAny<string?>(), It.IsAny<int>()))
                .Returns(() => _addedCredential == null
                    ? _dataSource.Credentials
                    : _dataSource.Credentials.Union(new[] {_addedCredential.NativeCredential}).ToArray());
            _nativeCredentialApi
                .Setup(native => native.CredDelete(_dataSource.Target, (int)_dataSource.CredentialType, It.IsAny<int>()))
                .Returns(() => true);
            if (!string.IsNullOrEmpty(idNotFound))
            {
                _nativeCredentialApi
                    .Setup(native => native.CredDelete(idNotFound, It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(false);
            }

            if (lastErrorCode != null)
                _errorCodeToString
                    .Setup(errorCodeToString => errorCodeToString.LogLastWin32Error(It.IsAny<ILoggerAdapter>(),
                        It.IsAny<IEnumerable<int>>(), It.IsAny<string>()))
                    .Callback<ILoggerAdapter, IEnumerable<int>, string>((logger, ignored, caller) => _targetDeleted = ignored.Contains((int)lastErrorCode))
                    .Returns(() => (!_targetDeleted, (int)lastErrorCode));

            _nativeCredentialApi
                .Setup(native =>
                    native.CredDelete(_dataSource.Target, (int)_dataSource.CredentialType, It.IsAny<int>()))
                .Callback<string, int, int>((target, type, flag) => _targetDeleted = true)
                .Returns(true);
            if (_addedCredential != null)
                _nativeCredentialApi
                    .Setup(native => native.CredWrite(It.IsAny<NativeApi.Credential>(), It.IsAny<int>()))
                    .Callback<NativeApi.Credential, int>((credential, flag) =>
                    {
                        Assert.That(credential.TargetName, Is.EqualTo(_addedCredential.NativeCredential.TargetName));
                    })
                    .Returns(() => successfulAdd);
        }
    }
}
