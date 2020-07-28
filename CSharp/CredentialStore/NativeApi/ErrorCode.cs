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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    internal static class ErrorCode
    {
        private static IDictionary<int, string> ErrorMessages { get; } = new Dictionary<int, string>
        {
            {NotFound, $"Element not found. {NotFound:x}"},
            {
                NoSuchLogonSession,
                $"A specified logon session does not exist. It might already have been terminated. {NoSuchLogonSession:x}"
            },
            {InvalidFlags, $"Invalid Flags {InvalidFlags:x}"},
            {InvalidArgument, $"Invalid Argument {InvalidFlags:x}"}
        };

        // ReSharper disable once MemberCanBePrivate.Global
        public const int NotFound = 0x00000490;
        // ReSharper disable once MemberCanBePrivate.Global
        public const int NoSuchLogonSession = 0x00000520;
        // ReSharper disable once MemberCanBePrivate.Global
        public const int InvalidFlags = 0x000003EC; 
        // ReSharper disable once MemberCanBePrivate.Global
        public const int InvalidArgument = 87; 

        /// <summary>
        /// Returns the error message for <paramref name="errorCode"/> if found, or a message
        /// saying the error is unknown
        /// </summary>
        public static string ErrorOrUnknownMessage(int errorCode) =>
            (ErrorMessages.ContainsKey(errorCode))
                ? ErrorMessages[errorCode]
                : $"Unknown error code {errorCode}";

        /// <summary>
        /// logs the last win32 error if not in <paramref name="ignoredErrors"/>
        /// </summary>
        /// <returns>true if error is logged; otherwise false</returns>
        public static bool LogLastWin32Error(ILoggerAdapter logger, IEnumerable<int> ignoredErrors, [CallerMemberName] string callerMemberName = "")
        {
            const int noError = 0;
            int lastError = Marshal.GetLastWin32Error();
            if (ignoredErrors?.Contains(lastError) == true || lastError == noError)
                return false;
            logger?.Error(ErrorOrUnknownMessage(lastError), callerMemberName: callerMemberName);
            return true;
        }
    }
}
