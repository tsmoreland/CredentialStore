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
using NativeCredential = Moreland.Security.Win32.CredentialStore.NativeApi.Credential;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CredentialManagerTest
    {
        private Mock<IErrorCodeToStringService> _errorCodeToStringService = null!;
        private Mock<INativeInterop> _nativeInterop = null!;
        private CredentialManager _manager = null!;
        private NativeCredential _credential = null!;
        private NativeCredential? _outputCredential;
        private IEnumerable<NativeCredential> _credentials = null!;
        private int _apiErrorCode;


        private delegate void CredReadCallback(string target, CredentialType type, int reservedFlag, out NativeCredential? credentialPtr);

        [SetUp]
        public void Setup()
        {
            _apiErrorCode = 0;
            _nativeInterop = new Mock<INativeInterop>();
            _errorCodeToStringService = new Mock<IErrorCodeToStringService>();

            _manager = new CredentialManager(_nativeInterop.Object, _errorCodeToStringService.Object);

            _credential = new NativeCredential 
            { 
                TargetName = TestData.GetRandomString(),
                Type = (int)TestData.GetRandomEnum(CredentialType.Unknown),
                Comment = TestData.GetRandomString(),
                CredentialBlob = IntPtr.Zero,
                CredentialBlobSize = 0,
                Persist = (int)TestData.GetRandomEnum(CredentialPeristence.Unknown),
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                TargetAlias = null,
                UserName = TestData.GetRandomString(),
            };

            _nativeInterop
                .Setup(api => api.CredRead(It.IsAny<string>(), It.IsAny<CredentialType>(), It.IsAny<int>(), out _outputCredential))
                .Callback(new CredReadCallback((string target, CredentialType type, int flag, out NativeCredential? ptr) => ptr = _outputCredential)) 
                .Returns(() => _apiErrorCode);
            _nativeInterop
                .Setup(api => api.CredWrite(It.IsAny<NativeCredential>(), It.IsAny<int>()))
                .Returns(() => _apiErrorCode);
            _nativeInterop
                .Setup(api => api.CredEnumerate(It.IsAny<string?>(), It.IsAny<int>()))
                .Returns(() => _credentials);
        }
        private Mock<INativeInterop> SetupNativeInteropThatThrows(int errorCode)
        {
            var nativeInterop = new Mock<INativeInterop>();
            nativeInterop
                .Setup(api => api.CredEnumerate(It.IsAny<string?>(), It.IsAny<int>()))
                .Throws(new Win32Exception(errorCode));

            return nativeInterop;
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenNativeInteropIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(null!, _errorCodeToStringService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("nativeInterop"));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(_nativeInterop.Object, null!));
            Assert.That(ex.ParamName, Is.EqualTo("errorCodeToStringService"));
        }

        [Test]
        public void Add_ThrowArgumentNullException_WhenCredentialIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _manager.Add(null!));
            Assert.That(exception.ParamName, Is.EqualTo("credential"));
        }

        [Test]
        public void Add_ThrowsWin32Exception_WhenApiReturnsError()
        {
            _apiErrorCode = TestData.GetRandomInteger(0);
            var credential = TestData.BuildRandomCredential();

            Assert.Throws<Win32Exception>(() => _manager.Add(credential));
        }

        [Test]
        public void Add_NoThrow_OnSuccess()
        {
            _apiErrorCode = 0;
            var credential = TestData.BuildRandomCredential();

            Assert.DoesNotThrow(() => _manager.Add(credential));
        }

        [Test]
        [TestCase(null!)]
        [TestCase("")]
        public void Find_ThrowArgumentException_WhenIdIsNullOrEmpty(string id)
        {
            var ex = Assert.Throws<ArgumentException>(() => _manager.Find(id, TestData.GetRandomEnum(CredentialType.Unknown)));
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void Find_ThrowWin32Exception_WhenApiReturnsError()
        {
            _apiErrorCode = TestData.GetRandomInteger(0, (int)NativeApi.ExpectedError.NotFound);
            string id = TestData.GetRandomString();
            var type = TestData.GetRandomEnum(CredentialType.Unknown);
            Credential? credential = null;

            var ex = Assert.Throws<Win32Exception>(() => credential = _manager.Find(id, type));

            Assert.That(ex.NativeErrorCode, Is.EqualTo(_apiErrorCode));
            Assert.That(credential, Is.Null);
        }

        [Test] 
        public void Find_ReturnsCredential_WhenApiReturnsNonNullSuccess()
        {
            _apiErrorCode = 0;
            _outputCredential = _credential;
            string id = TestData.GetRandomString();
            var type = TestData.GetRandomEnum(CredentialType.Unknown);

            var actual = _manager.Find(id, type);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual?.Id, Is.EqualTo(_outputCredential.TargetName));
        }

        [Test]
        public void Find_ReturnsNull_WhenApiReturnsNotFound()
        {
            _apiErrorCode = (int)NativeApi.ExpectedError.NotFound;
            _outputCredential = _credential;
            string id = TestData.GetRandomString();
            var type = TestData.GetRandomEnum(CredentialType.Unknown);

            var actual = _manager.Find(id, type);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Find_ReturnsCredentials_OnSuccess()
        {
            _credentials = TestData.BuildRandomNativeCredentials().ToArray();

            var actualCredentials = _manager.Find(TestData.GetRandomString(), TestData.GetRandomBool());

            Assert.That(actualCredentials.Select(ac => ac.Id), Is.EquivalentTo(_credentials.Select(c => c.TargetName)));
        }

        [Test]
        public void Find_Rethrows_WhenCredEnumerateThrows()
        {
            int eFail;
            unchecked
            {
                eFail = (int)0x80004005;
            }
            var nativeInterop = SetupNativeInteropThatThrows(eFail);
            var manager = new CredentialManager(nativeInterop.Object, _errorCodeToStringService.Object);

            var ex = Assert.Throws<Win32Exception>(() =>
                _ = manager.Find(TestData.GetRandomString(), TestData.GetRandomBool()).ToArray());
            Assert.That(ex.ErrorCode, Is.EqualTo(eFail));

        }

        [Test]
        public void Credentials_ReturnsAll_OnSuccess()
        {
            _credentials = TestData.BuildRandomNativeCredentials().ToArray();

            var actualCredentials = _manager.Credentials;

            Assert.That(actualCredentials.Select(ac => ac.Id), Is.EquivalentTo(_credentials.Select(c => c.TargetName)));
        }

        [Test]
        public void Credentials_Rethrows_WhenCredEnumerateThrows()
        {
            int eFail;
            unchecked
            {
                eFail = (int)0x80004005;
            }
            var nativeInterop = SetupNativeInteropThatThrows(eFail);
            var manager = new CredentialManager(nativeInterop.Object, _errorCodeToStringService.Object);

            var ex = Assert.Throws<Win32Exception>(() =>
                _ = manager.Credentials.ToArray());
            Assert.That(ex.ErrorCode, Is.EqualTo(eFail));
        }


    }
}
