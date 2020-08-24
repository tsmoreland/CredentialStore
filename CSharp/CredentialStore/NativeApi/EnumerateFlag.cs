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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// Flags passed to <see cref="NativeMethods.CredEnumerate(string, int, out int, out System.IntPtr)"/>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/wincred/nf-wincred-credenumeratew">Win32 CredEnumerateW</a>
    /// </summary>
    internal enum EnumerateFlag : uint
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// This function enumerates all of the credentials in the user's
        /// credential set. The target name of each credential is returned in
        /// the "namespace:attribute=target" format. If this flag is set and
        /// the Filter parameter is not NULL, the function fails and returns
        /// <see cref="ExpectedError.InvalidFlags"/>
        /// </summary>
        EnumerateAllCredentials = 0x1,

    }
}
