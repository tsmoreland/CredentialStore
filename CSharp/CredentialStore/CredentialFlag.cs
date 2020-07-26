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

namespace Moreland.Security.Win32.CredentialStore
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/wincred/ns-wincred-credentiala
    /// see Flags of Credential structure
    /// </summary>
    [Flags]
    public enum CredentialFlag : uint
    {
        None = 0,

        /// <summary>
        /// Bit set if the credential does not persist the CredentialBlob and 
        /// the credential has not been written during this logon session. This 
        /// bit is ignored on input and is set automatically when queried.
        /// If Type is CRED_TYPE_DOMAIN_CERTIFICATE, the CredentialBlob is not 
        /// persisted across logon sessions because the PIN of a certificate is 
        /// very sensitive information. Indeed, when the credential is written 
        /// to credential manager, the PIN is passed to the CSP associated with 
        /// the certificate. The CSP will enforce a PIN retention policy 
        /// appropriate to the certificate.
        /// If Type is CRED_TYPE_DOMAIN_PASSWORD or CRED_TYPE_DOMAIN_CERTIFICATE, 
        /// an authentication package always fails an authentication attempt when 
        /// using credentials marked as CRED_FLAGS_PROMPT_NOW. The application 
        /// (typically through the key ring UI) prompts the user for the password. 
        /// The application saves the credential and retries the authentication. 
        /// Because the credential has been recently written, the authentication 
        /// package now gets a credential that is not marked as <see cref="PromptNow"/>
        /// </summary>
        PromptNow = 0x2,

        /// <summary>
        /// Bit is set if this credential has a TargetName member set to the same 
        /// value as the UserName member. Such a credential is one designed to 
        /// store the CredentialBlob for a specific user. For more information, 
        /// see the CredMarshalCredential function.
        /// This bit can only be specified if Type is CRED_TYPE_DOMAIN_PASSWORD 
        /// or CRED_TYPE_DOMAIN_CERTIFICATE.
        /// </summary>
        UsernameTarget = 0x4,
    }
}
