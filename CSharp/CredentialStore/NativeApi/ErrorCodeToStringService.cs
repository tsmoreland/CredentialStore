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
using System.Runtime.CompilerServices;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <inheritdoc cref="IErrorCodeToStringService"/>
    /// </summary>
    internal sealed class ErrorCodeToStringService : IErrorCodeToStringService
    {
        private readonly IMarshalService _marshalService;
        private readonly ILoggerAdapter _logger;
        private static IDictionary<int, string> ErrorMessages { get; } = new Dictionary<int, string>
        {
            {(int)ExpectedError.NotFound, $"Element not found. {(int)ExpectedError.NotFound:x}"},
            {
                (int)ExpectedError.NoSuchLogonSession,
                $"A specified logon session does not exist. It might already have been terminated. {(int)ExpectedError.NoSuchLogonSession:x}"
            },
            {(int)ExpectedError.InvalidFlags, $"Invalid Flags {(int)ExpectedError.InvalidFlags:x}"},
            {(int)ExpectedError.InvalidArgument, $"Invalid Argument {(int)ExpectedError.InvalidFlags:x}"}
        };

        public ErrorCodeToStringService(IMarshalService marshalService, ILoggerAdapter loggerAdapter)
        {
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            _logger = loggerAdapter ?? throw new ArgumentNullException(nameof(loggerAdapter));
        }

        public string GetMessageFor(ExpectedError errorCode) =>
            GetMessageFor((int)errorCode);

        public string GetMessageFor(int errorCode)
        {
            if (ErrorMessages.ContainsKey(errorCode))
                return ErrorMessages[errorCode];

            _logger.Error($"unrecognized error {errorCode:X}");
            return $"Unknown error {errorCode:X}";
        }

        public (bool Logged, int ErrorCode) LogLastWin32Error(ILoggerAdapter logger, IEnumerable<int> ignoredErrors, [CallerMemberName] string callerMemberName = "")
        {
            const int noError = 0;
            int lastError = _marshalService.GetLastWin32Error();
            if (ignoredErrors?.Contains(lastError) == true || lastError == noError)
                return (false, lastError);
            logger?.Error(GetMessageFor(lastError), callerMemberName: callerMemberName);
            return (true, lastError);
        }
    }
}
