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
    internal class NativeUtilities : INativeUtilities
    {
        private readonly ILoggerAdapter _logger;

        public NativeUtilities(ILoggerAdapter logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            MarshalService = new MarshalService();
            PointerMath = new PointerMath(MarshalService, logger);
            NativeInterop = new NativeInterop(new CredentialApi(MarshalService), MarshalService,
                ErrorCodeToStringService, logger);
            ErrorCodeToStringService = new ErrorCodeToStringService(MarshalService, logger);
        }

        public NativeUtilities(IMarshalService marshalService, IPointerMath pointerMath, INativeInterop nativeInterop,
            IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
        {
            MarshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            PointerMath = pointerMath ?? throw new ArgumentNullException(nameof(pointerMath));
            NativeInterop = nativeInterop ?? throw new ArgumentNullException(nameof(nativeInterop));
            ErrorCodeToStringService = errorCodeToStringService ?? throw new ArgumentNullException(nameof(errorCodeToStringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IMarshalService MarshalService { get; }
        public INativeInterop NativeInterop { get; }
        public IErrorCodeToStringService ErrorCodeToStringService { get; }
        public IPointerMath PointerMath { get; }
    }
}
