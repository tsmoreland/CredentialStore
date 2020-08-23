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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// <inheritdoc cref="ILoggerAdapter"/>
    /// </summary>
    internal sealed class MsExtensionsLoggerAdapter : ILoggerAdapter
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Instantiates a new instance of the <see cref="MsExtensionsLoggerAdapter"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> wrapped by this class, it is used to perform the actual logging</param>
        /// <exception cref="ArgumentNullException">if <paramref name="logger"/> is null.</exception>
        public MsExtensionsLoggerAdapter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Error"/>
        /// </summary>
        public void Error(string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            _logger.LogError(exception, FormatMessage(message, callerMemberName));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Info"/>
        /// </summary>
        public void Info(string message, [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            _logger.LogInformation(FormatMessage(message, callerMemberName));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Verbose"/>
        /// </summary>
        public void Verbose(string message, [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            _logger.LogDebug(FormatMessage(message, callerMemberName));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Warning"/>
        /// </summary>
        public void Warning(string message, [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            _logger.LogWarning(FormatMessage(message, callerMemberName));
        }
        private static string FormatMessage(string message, string callerMemberName) =>
            $"[{DateTime.UtcNow:o}][{callerMemberName}]: {message}";
    }
}
