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
using System.ComponentModel;
using Moq;
using Moreland.Security.Win32.CredentialStore.NativeApi;
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class NativeInteropTests
    {
        private NativeInterop _nativeInterop = null!;
        private Mock<ICredentialApi> _credentialApi = null!;
        private Mock<ICriticalCredentialHandleFactory> _criticalCredentialHandleFactory = null!;
        private Mock<IMarshalService> _marshalService = null!;
        private Mock<IErrorCodeToStringService> _errorCodeToStringService = null!;
        private Mock<ILoggerAdapter> _logger = null!;
        private int _credReadReturnValue;
        private IntPtr _credReadOutPtr = IntPtr.Zero;

        private delegate void CredReadCallback(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

        [SetUp]
        public void Setup()
        {
            _credReadOutPtr = IntPtr.Zero;
            _credentialApi = new Mock<ICredentialApi>();
            _criticalCredentialHandleFactory = new Mock<ICriticalCredentialHandleFactory>();
            _marshalService = new Mock<IMarshalService>();
            _errorCodeToStringService = new Mock<IErrorCodeToStringService>();
            _logger = new Mock<ILoggerAdapter>();

            _nativeInterop = new NativeInterop(_credentialApi.Object, _criticalCredentialHandleFactory.Object,
                _marshalService.Object, _errorCodeToStringService.Object, _logger.Object);

            _credentialApi
                .Setup(api => api.CredRead(It.IsAny<string>(), It.IsAny<CredentialType>(), It.IsAny<int>(),
                    out _credReadOutPtr))
                .Callback(new CredReadCallback((string target, CredentialType type, int flag, out IntPtr ptr) => ptr = _credReadOutPtr)) 
                .Returns(() => _credReadReturnValue);
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullExceptionWhenArgumentsAreNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new NativeInterop(null!,
                _criticalCredentialHandleFactory.Object, _marshalService.Object, _errorCodeToStringService.Object,
                _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("credentialApi"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new NativeInterop(_credentialApi.Object,
                null!, _marshalService.Object, _errorCodeToStringService.Object,
                _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("criticalCredentialHandleFactory"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new NativeInterop(_credentialApi.Object,
                _criticalCredentialHandleFactory.Object, null!, _errorCodeToStringService.Object,
                _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("marshalService"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new NativeInterop(_credentialApi.Object,
                _criticalCredentialHandleFactory.Object, _marshalService.Object, null!,
                _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("errorCodeToStringService"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new NativeInterop(_credentialApi.Object,
                _criticalCredentialHandleFactory.Object, _marshalService.Object, _errorCodeToStringService.Object,
                null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void CredReadShould_ThrowExceptionWhenApiReturnsErrorUnlessNotFound()
        {
            _credReadReturnValue = TestData.GetRandomInteger(0, (int)ExpectedError.NotFound);

            Assert.Throws<Win32Exception>(() => _ = _nativeInterop.CredRead(TestData.GetRandomString(),
                TestData.GetRandomCredentialType(), TestData.GetRandomInteger()));
        }

        [Test]
        public void CredReadShould_NotThrowExceptionWhenApiReturnsNotFoundError()
        {
            _credReadReturnValue = (int)ExpectedError.NotFound;
            Assert.DoesNotThrow(() => _ = _nativeInterop.CredRead(TestData.GetRandomString(),
                TestData.GetRandomCredentialType(), TestData.GetRandomInteger()));
        }
    }
}
