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
using System.Collections.Generic;
using System.Linq;
using Moq;
using Moreland.Security.Win32.CredentialStore.NativeApi;
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    [TestFixture]
    public sealed class ErrorCodeToStringServiceTests
    {
        private Mock<ILoggerAdapter> _logger = null!;
        private ErrorCodeToStringService _errorCodeToStringService = null!;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggerAdapter>();
            _errorCodeToStringService = new ErrorCodeToStringService(_logger.Object);
        }

        [Test]
        public void ConstructorShould_ThrowArgumentNullExceptionWhenArgumentsAreNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _ = new ErrorCodeToStringService(null!));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        [TestCaseSource(nameof(ExpectedErrors))]
        public void GetMessageForShould_ReturnStringForExpectedErrors(int errorCode)
        {
            var message = _errorCodeToStringService.GetMessageFor((ExpectedError)errorCode);

            Assert.That(message, Is.Not.Null);
            Assert.That(message, Is.Not.Empty);

            message = _errorCodeToStringService.GetMessageFor(int.MaxValue);

            Assert.That(message, Is.Not.Null);
            Assert.That(message, Is.Not.Empty);
        }

        private static IEnumerable<int> ExpectedErrors =>
            Enum.GetValues(typeof(ExpectedError))
                .OfType<ExpectedError>()
                .Cast<int>();
    }
}
