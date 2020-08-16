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
using Moq;
using Moreland.Security.Win32.CredentialStore.NativeApi;
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CriticalCredentialHandleFactoryTest
    {
        private Mock<ICredentialApi> _credentialApi = null!;
        private Mock<IMarshalService> _marshalService = null!;
        private Mock<IErrorCodeToStringService> _errorCodeToStringService = null!;
        private Mock<ILoggerAdapter> _logger = null!;
        private ICriticalCredentialHandleFactory _factory = null!;

        [SetUp]
        public void Setup()
        {
            _credentialApi = new Mock<ICredentialApi>();
            _marshalService = new Mock<IMarshalService>();
            _errorCodeToStringService = new Mock<IErrorCodeToStringService>();
            _logger = new Mock<ILoggerAdapter>();

            _factory = new CriticalCredentialHandleFactory(_credentialApi.Object, _marshalService.Object,
                _errorCodeToStringService.Object, _logger.Object);
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullExceptionWhenArgumentsAreNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                _ = new CriticalCredentialHandleFactory(null!, _marshalService.Object,
                    _errorCodeToStringService.Object, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("credentialApi"));

            ex = Assert.Throws<ArgumentNullException>(() =>
                _ = new CriticalCredentialHandleFactory(_credentialApi.Object, null!,
                    _errorCodeToStringService.Object, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("marshalService"));

            ex = Assert.Throws<ArgumentNullException>(() =>
                _ = new CriticalCredentialHandleFactory(_credentialApi.Object, _marshalService.Object,
                    null!, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("errorCodeToStringService"));

            ex = Assert.Throws<ArgumentNullException>(() =>
                _ = new CriticalCredentialHandleFactory(_credentialApi.Object, _marshalService.Object,
                    _errorCodeToStringService.Object, null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void BuildShould_ReturnCriticalCredentialHandleWithoutException()
        {
            ICriticalCredentialHandle handle = null!;
            Assert.DoesNotThrow(() => handle = _factory.Build(IntPtr.Zero));
            handle.Dispose();

            var badPointer = IntPtr.Add(IntPtr.Zero, 1024);
            Assert.DoesNotThrow(() => handle = _factory.Build(badPointer));
            handle.Dispose();
        }
    }
}
