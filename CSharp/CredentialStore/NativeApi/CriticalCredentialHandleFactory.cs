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
    internal class CriticalCredentialHandleFactory : ICriticalCredentialHandleFactory
    {
        private readonly ICredentialApi _credentialApi;
        private readonly IMarshalService _marshalService;
        private readonly IErrorCodeToStringService _errorCodeToStringService;
        private readonly ILoggerAdapter _logger;

        /// <summary>
        /// Instantiates a new instance of the <see cref="CriticalCredentialHandleFactory"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if any of the provided arguments are null
        /// </exception>
        public CriticalCredentialHandleFactory(ICredentialApi credentialApi, IMarshalService marshalService, IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
        {
            _credentialApi = credentialApi ?? throw new ArgumentNullException(nameof(credentialApi));
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            _errorCodeToStringService = errorCodeToStringService ?? throw new ArgumentNullException(nameof(errorCodeToStringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICriticalCredentialHandle Build(IntPtr handle) =>
            new CriticalCredentialHandle(handle, _credentialApi, _marshalService, 
                _errorCodeToStringService, _logger);
    }
}
