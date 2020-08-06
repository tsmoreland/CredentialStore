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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    internal class CriticalCredentialHandleFactory
    {
        private readonly INativeHelper _nativeHelper;
        private readonly ILoggerAdapter _logger;

        /// <summary>
        /// Instantiates a new instance of the <see cref="CriticalCredentialHandleFactory"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="nativeHelper"/> or <paramref name="logger"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <see cref="INativeHelper.IsValid"/> for <paramref name="nativeHelper"/>
        /// returns false
        /// </exception>
        public CriticalCredentialHandleFactory(INativeHelper nativeHelper, ILoggerAdapter logger)
        {
            _nativeHelper = nativeHelper ?? throw new ArgumentNullException(nameof(nativeHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (!_nativeHelper.IsValid)
                throw new ArgumentException("nativeHelper is not valid", nameof(nativeHelper));
        }

        public CriticalCredentialHandle Build(IntPtr handle) =>
            new CriticalCredentialHandle(handle, _nativeHelper.NativeCredentialApi,
                _nativeHelper.ErrorCodeToStringService, _logger);
    }
}
