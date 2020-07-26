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

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// Facade for Logging interface allowing allowing wrapping of external
    /// loggers 
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// log an error
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="exception">optional exception object</param>
        /// <param name="callerMemberName">
        /// method which called logger, /// implementors should decorate with 
        /// <see cref="CallerMemberNameAttribute"/>
        /// </param>
        void Error(string message, Exception? exception = null, string callerMemberName = "");
        /// <summary>
        /// log a warning 
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="callerMemberName">
        /// method which called logger, /// implementors should decorate with 
        /// <see cref="CallerMemberNameAttribute"/>
        /// </param>
        void Warning(string message, string callerMemberName = "");
        /// <summary>
        /// log informational message
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="callerMemberName">
        /// method which called logger, /// implementors should decorate with 
        /// <see cref="CallerMemberNameAttribute"/>
        /// </param>
        void Info(string message, string callerMemberName = "");
        /// <summary>
        /// log verbose message
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="callerMemberName">
        /// method which called logger, /// implementors should decorate with 
        /// <see cref="CallerMemberNameAttribute"/>
        /// </param>
        void Verbose(string message, string callerMemberName = "");
    }
}
