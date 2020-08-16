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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using Moq;
using Moreland.Security.Win32.CredentialStore.NativeApi;
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class CriticalCredentialHandleTests
    {
        private IntPtr _pointer;
        private bool _released;
        private Mock<ICredentialApi> _credentialApi = null!;
        private Mock<IMarshalService> _marshalService = null!;
        private Mock<IErrorCodeToStringService> _errorCodeToStringService = null!;
        private Mock<ILoggerAdapter> _logger = null!;
        private NativeApi.Credential _credential = null!;

        [SetUp]
        public void Setup()
        {
            _credential = RandomCredential();
            _credentialApi = new Mock<ICredentialApi>();
            _marshalService = new Mock<IMarshalService>();
            _errorCodeToStringService = new Mock<IErrorCodeToStringService>();
            _logger = new Mock<ILoggerAdapter>();
            _pointer = Marshal.AllocHGlobal(Marshal.SizeOf(_credential));
            Marshal.StructureToPtr(_credential, _pointer, false);
            _released = false;
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullExceptionWhenArgumentsAreNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CriticalCredentialHandle(_pointer, null!, _marshalService.Object, _errorCodeToStringService.Object, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("credentialApi"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new CriticalCredentialHandle(_pointer, _credentialApi.Object, null!, _errorCodeToStringService.Object, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("marshalService"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new CriticalCredentialHandle(_pointer, _credentialApi.Object, _marshalService.Object, null!, _logger.Object));
            Assert.That(ex.ParamName, Is.EqualTo("errorCodeToStringService"));

            ex = Assert.Throws<ArgumentNullException>(() => _ = new CriticalCredentialHandle(_pointer, _credentialApi.Object, _marshalService.Object, _errorCodeToStringService.Object, null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void ConstructorShouldNot_ThrowExceptionWhenArgumentsAreValid()
        {
            Assert.DoesNotThrow(() => _ = new CriticalCredentialHandle(_pointer, _credentialApi.Object, _marshalService.Object, _errorCodeToStringService.Object, _logger.Object));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NativeCredentialShould_ReturnProvidedCredentialIfValid(bool isValid)
        {
            NativeApi.Credential credential = RandomCredential();
            bool released = false;
            IntPtr pointer = IntPtr.Zero;

            try
            {
                if (isValid)
                {
                    pointer = Marshal.AllocHGlobal(Marshal.SizeOf(_credential));
                    Marshal.StructureToPtr(credential, pointer, false);
                    _credentialApi
                        .Setup(api => api.CredFree(pointer))
                        .Callback<IntPtr>(ptr =>
                        {
                            if (ptr == IntPtr.Zero)
                                return;
                            Marshal.FreeHGlobal(ptr);
                            released = true;
                        });
                    _marshalService
                        .Setup(marshal => marshal.PtrToStructure(pointer, typeof(NativeApi.Credential)))
                        .Returns(credential);
                }

                var critialHandle = new CriticalCredentialHandle(pointer, _credentialApi.Object,
                    _marshalService.Object, _errorCodeToStringService.Object, _logger.Object);

                if (isValid)
                    Assert.That(critialHandle.NativeCredential, Is.EqualTo(credential));
                else
                    Assert.That(critialHandle.NativeCredential, Is.Null);
                critialHandle.Dispose();
                if (isValid)
                    Assert.That(released, Is.True);

            }
            finally
            {
                if (!released && pointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pointer);
                }
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsValidShould_ReturnTrueForNonZeroHandle(bool useZero)
        {
            SetupCredentialApi();
            using var criticalHandle = new CriticalCredentialHandle(useZero ? IntPtr.Zero : _pointer,
                _credentialApi.Object, _marshalService.Object, _errorCodeToStringService.Object, _logger.Object);

            if (useZero)
                Assert.That(criticalHandle.IsValid, Is.False);
            else
                Assert.That(criticalHandle.IsValid, Is.True);

            Assert.That(criticalHandle.IsValid, Is.Not.EqualTo(criticalHandle.IsInvalid));
        }

        [Test]
        public void DisposeShould_NotThrowExceptionWhenCredFreeThrowsException()
        {
            SetupCredentialApi();
            _credentialApi
                .Setup(cred => cred.CredFree(_pointer))
                .Throws<Win32Exception>();
            var criticalHandle = new CriticalCredentialHandle(_pointer,
                _credentialApi.Object, _marshalService.Object, _errorCodeToStringService.Object, _logger.Object);
            Assert.DoesNotThrow(() =>  criticalHandle.Dispose());

        }

        private static NativeApi.Credential RandomCredential()
        {
            var random = RandomNumberGenerator.Create();

            var credential = new NativeApi.Credential
            { 
                Flags = GetRandomNumber(random),
                Type = GetRandomNumber(random),
                TargetName = Guid.NewGuid().ToString("N"),
                Comment = Guid.NewGuid().ToString("N"),
                LastWritten = new FILETIME { dwLowDateTime = GetRandomNumber(random), dwHighDateTime = GetRandomNumber(random) },
                CredentialBlobSize =  0,
                CredentialBlob = IntPtr.Zero,
                Attributes = IntPtr.Zero,
                TargetAlias = Guid.NewGuid().ToString("N"),
                UserName = Guid.NewGuid().ToString("N"),
            };
            return credential;

            static int GetRandomNumber(RandomNumberGenerator generator)
            {
                var buffer = new byte[sizeof(int)];
                generator.GetBytes(buffer);
                return BitConverter.ToInt32(buffer);
            }
        }


        private void SetupCredentialApi()
        {
            _credentialApi
                .Setup(api => api.CredFree(_pointer))
                .Callback<IntPtr>(pointer =>
                {
                    if (pointer == IntPtr.Zero)
                        return;
                    Marshal.FreeHGlobal(pointer);
                    _released = true;
                });
        }

        [TearDown]
        public void TearDown()
        {
            if (_released)
                return;
            Marshal.FreeHGlobal(_pointer);
            _pointer = IntPtr.Zero;
        }
    }
}
