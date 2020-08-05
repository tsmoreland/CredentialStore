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

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Aggregrate root like container for common native helper utilities
    /// </summary>
    public interface INativeHelper
    {
        /// <summary>
        /// <see cref="IPointerMath"/>
        /// </summary>
        IPointerMath PointerMath { get; }
        /// <summary>
        /// <see cref="IMarshalService"/>
        /// </summary>
        IMarshalService MarshalService { get; }
        /// <summary>
        /// <see cref="IErrorCodeToStringService"/>
        /// </summary>
        IErrorCodeToStringService ErrorCodeToStringService { get; }
        /// <summary>
        /// <see cref="INativeCredentialApi"/>
        /// </summary>
        INativeCredentialApi NativeCredentialApi { get; }

        /// <summary>
        /// Returns true if all members are non-null 
        /// </summary>
        bool IsValid { get; }
    }
}
