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
    public sealed class CredentialManagerDependeniesAggregateTests
    {
        private Mock<ILoggerAdapter> _logger = null!;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggerAdapter>();
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullExceptionIfArgumentsAreNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new CredentialManagerDependeniesAggregate(null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }


        [Test]
        public void ConstructorShould_InitializeAllProperties()
        {
            CredentialManagerDependeniesAggregate aggregate = null!;
            Assert.DoesNotThrow(() => aggregate = new CredentialManagerDependeniesAggregate(_logger.Object));
            if (aggregate == null)
                return;

            Assert.That(aggregate.MarshalService, Is.Not.Null);
            Assert.That(aggregate.NativeInterop, Is.Not.Null);
            Assert.That(aggregate.ErrorCodeToStringService, Is.Not.Null);
            Assert.That(aggregate.PointerMath, Is.Not.Null);
        }
    }
}