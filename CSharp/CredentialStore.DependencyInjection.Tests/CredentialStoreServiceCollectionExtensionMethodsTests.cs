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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Moreland.Security.Win32.CredentialStore.DependencyInjection.Tests
{
    [TestFixture]
    public sealed class CredentialStoreServiceCollectionExtensionMethodsTests
    {
        private Mock<IServiceCollection> _serviceCollection = null!;

        [SetUp]
        public void Setup()
        {
            _serviceCollection = new Mock<IServiceCollection>();
        }

        [Test]
        public void AddCredentialStore_ThrowsArgumentNullException_WhenServiceCollectionIsNull()
        {
            var options = new CredentialStoreOptions(LoggerType.None, ServiceLifetime.Singleton);
            var ex = Assert.Throws<ArgumentNullException>(() =>
                CredentialStoreServiceCollectionExtensionMethods.AddCredentialStore(null!, options));
            Assert.That(ex.ParamName, Is.EqualTo("serviceCollection"));
        }

    }
}
