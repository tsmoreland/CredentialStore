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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <inheritdoc cref="INativeHelper"/>
    /// </summary>
    internal sealed class NativeHelper : INativeHelper
    {
        private readonly ILoggerAdapter _logger;

        public NativeHelper(ILoggerAdapter logger)
            : this(new MarshalService(), logger)
        {
        }
        private NativeHelper(IMarshalService marshalService, ILoggerAdapter logger)
            : this(marshalService, new ErrorCodeToStringService(marshalService, logger), logger)
        {
        }
        private NativeHelper(IMarshalService marshalService, IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
            : this(
                new PointerMath(marshalService, logger), 
                marshalService, 
                new NativeCredentialApi(marshalService, errorCodeToStringService, logger),
                errorCodeToStringService,
                logger)
        {
        }
        private NativeHelper(IPointerMath pointerMath, IMarshalService marshalService,
            INativeCredentialApi nativeCredentialApi, IErrorCodeToStringService errorCodeToStringService,
            ILoggerAdapter logger)
            : this(
                pointerMath,
                marshalService, 
                nativeCredentialApi,
                new CriticalCredentialHandleFactory(nativeCredentialApi, errorCodeToStringService, logger),
                errorCodeToStringService,
                logger)
        {
        }

        public NativeHelper(IPointerMath pointerMath, IMarshalService marshalService,
            INativeCredentialApi nativeCredentialApi, ICriticalCredentialHandleFactory criticalCredentialHandleFactory,
            IErrorCodeToStringService errorCodeToStringService, ILoggerAdapter logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PointerMath = pointerMath ?? throw new ArgumentNullException(nameof(pointerMath));
            MarshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            NativeCredentialApi = nativeCredentialApi ?? throw new ArgumentNullException(nameof(nativeCredentialApi));
            CriticalCredentialHandleFactory = criticalCredentialHandleFactory ??
                                              throw new ArgumentNullException(nameof(criticalCredentialHandleFactory));
            ErrorCodeToStringService = errorCodeToStringService ??
                                       throw new ArgumentNullException(nameof(errorCodeToStringService));
        }

        public IPointerMath PointerMath { get; }
        public IMarshalService MarshalService { get; }
        public INativeCredentialApi NativeCredentialApi { get; }
        public ICriticalCredentialHandleFactory CriticalCredentialHandleFactory { get; }
        public IErrorCodeToStringService ErrorCodeToStringService { get; }

        /// <summary>
        /// Returns true if all members are non-null 
        /// </summary>
        public bool IsValid => GetMemberValues().All(@object => !ReferenceEquals(@object, null));


        private IEnumerable<object> GetMemberValues()
        {
            yield return PointerMath;
            yield return MarshalService;
            yield return NativeCredentialApi;
            yield return CriticalCredentialHandleFactory;
            yield return ErrorCodeToStringService;
        }
    }
}
