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
using NativeCredential = Moreland.Security.Win32.CredentialStore.NativeApi.Credential;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CredentialManagerTest
    {
        private Mock<ILoggerAdapter> _logger = null!;
        private Mock<INativeInterop> _nativeInterop = null!;
        private CredentialManager _manager = null!;
        private NativeCredential _credential = null!;

        [SetUp]
        public void Setup()
        {
            _nativeInterop = new Mock<INativeInterop>();
            _logger = new Mock<ILoggerAdapter>();

            _manager = new CredentialManager(_nativeInterop.Object, _logger.Object);

            _credential = new NativeCredential 
            { 
                TargetName = TestData.GetRandomString(),
                Comment = TestData.GetRandomString(),
                CredentialBlob = IntPtr.Zero,
                CredentialBlobSize = 0,
                Persist = (int)TestData.GetRandomEnum<CredentialPeristence>(),
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                TargetAlias = null,
                UserName = TestData.GetRandomString(),
            };
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenNativeInteropIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(null!, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("nativeInterop"));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CredentialManager(_nativeInterop.Object, null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void AddShould_ThrowArgumentNullException_WhenCredentialIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _manager.Add(null!));
            Assert.That(exception.ParamName, Is.EqualTo("credential"));
        }

        [Test]
        [TestCase(null!)]
        [TestCase("")]
        public void FindShould_ThrowArgumentExceptionIfIdIsNullOrEmpty(string id)
        {
            var ex = Assert.Throws<ArgumentException>(() => _manager.Find(id, CredentialType.Generic));
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }
    }
}
