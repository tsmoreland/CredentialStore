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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moreland.Security.Win32.CredentialStore.Tests
{
    /// <summary>
    /// <inheritdoc cref="IEnumerable{TestData}"/>
    /// </summary>
    internal class TestDataSource : IEnumerable<TestData>
    {

        public static IEnumerable<TestData> Source => new TestDataSource();

        public static IEnumerable<TestData> SourceWhereTargetIsIncluded =>
            (new TestDataSource())
            .Where(data => data.IncludesTarget);


        public IEnumerator<TestData> GetEnumerator()
        {
            yield return new TestData(CredentialType.Generic, true);
            yield return new TestData(CredentialType.DomainPassword, true);
            yield return new TestData(CredentialType.Generic, false);
            yield return new TestData(CredentialType.DomainPassword, false);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
