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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// <inheritdoc cref="ILoggerAdapter"/>
    /// </summary>
    public sealed class TraceLoggerAdapter : ILoggerAdapter
    {
        private static readonly TraceSwitch _credentialStoreSwitch =
            new TraceSwitch("CredentialStore", ".NET Trace diagnostics switch");

        /// <summary>
        /// Singleton instance of Diagnostics logger used if no other logger is provided
        /// </summary>
        public static ILoggerAdapter DiagnosticsLogger => _diagnosticsLogger.Value;
        private static readonly Lazy<ILoggerAdapter> _diagnosticsLogger =
            new Lazy<ILoggerAdapter>(() => new TraceLoggerAdapter());

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Error"/>
        /// </summary>
        public void Error(string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "")
        {
            Trace.WriteLineIf(_credentialStoreSwitch.TraceError, (FormatMessage(TraceLevel.Error, message, callerMemberName)));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Info"/>
        /// </summary>
        public void Info(string message, [CallerMemberName] string callerMemberName = "")
        {
            Trace.WriteLineIf(_credentialStoreSwitch.TraceInfo, (FormatMessage(TraceLevel.Error, message, callerMemberName)));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Verbose"/>
        /// </summary>
        public void Verbose(string message, [CallerMemberName] string callerMemberName = "")
        {
            Trace.WriteLineIf(_credentialStoreSwitch.TraceVerbose, (FormatMessage(TraceLevel.Error, message, callerMemberName)));
        }

        /// <summary>
        /// <inheritdoc cref="ILoggerAdapter.Warning"/>
        /// </summary>
        public void Warning(string message, [CallerMemberName] string callerMemberName = "")
        {
            Trace.WriteLineIf(_credentialStoreSwitch.TraceWarning, (FormatMessage(TraceLevel.Error, message, callerMemberName)));
        }

        private static string FormatMessage(TraceLevel level, string message, string callerMemberName) =>
            $"[{DateTime.UtcNow:o}][{level}][{callerMemberName}]: {message}";
    }
}
