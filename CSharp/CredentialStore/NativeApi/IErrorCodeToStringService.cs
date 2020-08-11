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

using System.Collections.Generic;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    internal interface IErrorCodeToStringService
    {
        /// <summary>
        /// Gets the error message for <paramref name="errorCode"/> if known,
        /// ottherwise generic error message
        /// </summary>
        string GetMessageFor(int errorCode);

        /// <summary>
        /// logs the last win32 error if not in <paramref name="ignoredErrors"/>
        /// </summary>
        /// <returns>true if error is logged; otherwise false</returns>
        (bool Logged, int ErrorCode) LogLastWin32Error(ILoggerAdapter logger, IEnumerable<int> ignoredErrors,
            string callerMemberName = "");
    }
}