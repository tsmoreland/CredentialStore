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
    /// <summary>
    /// <inheritdoc cref="ICredentialManagerDependeniesAggregate"/>
    /// </summary>
    internal sealed class CredentialManagerDependeniesAggregate : ICredentialManagerDependeniesAggregate
    {
        public CredentialManagerDependeniesAggregate(ILoggerAdapter logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            MarshalService = new MarshalService();
            PointerMath = new PointerMath(MarshalService, logger);
            ErrorCodeToStringService = new ErrorCodeToStringService(logger);
            var credentialApi = new CredentialApi(MarshalService);
            NativeInterop = new NativeInterop(credentialApi, new CriticalCredentialHandleFactory(credentialApi, MarshalService, ErrorCodeToStringService, logger), MarshalService,
                ErrorCodeToStringService, logger);
        }

        /// <summary>
        /// <inheritdoc cref="ICredentialManagerDependeniesAggregate.MarshalService"/>
        /// </summary>
        public IMarshalService MarshalService { get; }
        /// <summary>
        /// <inheritdoc cref="ICredentialManagerDependeniesAggregate.NativeInterop"/>
        /// </summary>
        public INativeInterop NativeInterop { get; }
        /// <summary>
        /// <inheritdoc cref="ICredentialManagerDependeniesAggregate.ErrorCodeToStringService"/>
        /// </summary>
        public IErrorCodeToStringService ErrorCodeToStringService { get; }
        /// <summary>
        /// <inheritdoc cref="ICredentialManagerDependeniesAggregate.PointerMath"/>
        /// </summary>
        public IPointerMath PointerMath { get; }
    }
}
