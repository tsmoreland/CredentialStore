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
    /// Limits as defined in documentation for Win32 Credential API 
    /// https://docs.microsoft.com/en-us/windows/win32/api/wincred/ns-wincred-credentiala
    /// see wincred.h for more details
    /// </summary>
    internal static class Limits
    {
        /// <summary>
        /// Username can be in <domain>\<user> or <user>@<domain>
        /// Length in characters, not including nul termination.
        /// </summary>
        public const int MaximumUserNameLength = 513; 
        /// <summary>
        /// Win32 definition CREDUI_MAX_PASSWORD_LENGTH
        /// </summary>
        public const int MaximumPasswordLength = 256;

        /// <summary>
        /// Maximum length of the various credential string fields (in characters)
        /// </summary>
        public const int MaximumCredentialStringLength = 256;

        /// <summary>
        /// Maximum size of the CredBlob field (in bytes)
        /// </summary>
        public const int MaxCredentialBlobSize = 512;

        /// <summary>
        /// CREDUI_MAX_CAPTION_LENGTH
        /// </summary>
        public const int MaxCaptionLength = 128;

        /// <summary>
        /// CRED_MAX_ATTRIBUTES
        /// </summary>
        public const int MaxAttributes = 64;
    }
}
