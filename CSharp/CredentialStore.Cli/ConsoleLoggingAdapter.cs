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
using Microsoft.Extensions.Logging;

namespace Moreland.Security.Win32.CredentialStore.Cli
{
    /// <summary>
    /// <inheritdoc cref="ILoggerAdapter"/>
    /// </summary>
    public sealed class ConsoleLoggingAdapter : ILoggerAdapter
    {
        private readonly ILogger _logger;

        public ConsoleLoggingAdapter(ILoggerFactory loggerFactory)
        {
            var factory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = factory.CreateLogger("CLI");
        }

        public void Error(string message, Exception? exception = null, string callerMemberName = "") =>
            _logger.LogError(FormatMessage(message, callerMemberName, exception));

        public void Info(string message, string callerMemberName = "") =>
            _logger.LogInformation(FormatMessage(message, callerMemberName));

        public void Verbose(string message, string callerMemberName = "") =>
            _logger.LogDebug(FormatMessage(message, callerMemberName));

        public void Warning(string message, string callerMemberName = "") =>
            _logger.LogWarning(FormatMessage(message, callerMemberName));

        private static string FormatMessage(string message, string callerMemberName, Exception? exception = null)
        {
            if (exception != null)
                message = $"{message}; {exception.Message}";

            if (!string.IsNullOrEmpty(callerMemberName))
                message = $"[{callerMemberName}]: {message}";

            return message;
        }
    }
}
