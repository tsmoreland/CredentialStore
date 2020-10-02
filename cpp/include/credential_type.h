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

#pragma once

namespace win32::credential_store
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/wincred/ns-wincred-credentiala
    /// </summary>
    enum class credential_type : int
    {
        unknown = 0,

        /// <summary>
        /// The credential is a generic credential. The credential will not be 
        /// used by any particular authentication package. The credential will 
        /// be stored securely but has no other significant characteristics.
        /// </summary>
        generic = 1,

        /// <summary>
        /// The credential is a password credential and is specific to 
        /// Microsoft's authentication packages. The NTLM, Kerberos, and 
        /// Negotiate authentication packages will automatically use this 
        /// credential when connecting to the named target.
        /// </summary>
        domain_password = 2,

        /// <summary>
        /// The credential is a certificate credential and is specific to 
        /// Microsoft's authentication packages. The Kerberos, Negotiate, and 
        /// Schannel authentication packages automatically use this credential 
        /// when connecting to the named target.
        /// </summary>
        domain_certificate = 3,

        /// <summary>
        /// This error is no longer supported.
        /// Windows Server 2003 and Windows XP:  The credential is a password 
        /// credential and is specific to authentication packages from Microsoft. 
        /// The Passport authentication package will automatically use this credential 
        /// when connecting to the named target.
        /// Additional values will be defined in the future. Applications should be 
        /// written to allow for credential types they do not understand.
        /// </summary>
        domain_visible_password = 4,

        /// <summary>
        /// The credential is a certificate credential that is a generic 
        /// authentication package.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and /// Windows XP: This error is not supported.
        /// </summary>
        generic_certificate =  5,


        /// <summary>
        /// The credential is supported by extended Negotiate packages.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This error is not supported.
        /// </summary>
        domain_extended = 6,

        /// <summary>
        /// The maximum number of supported credential types.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This error is not supported.
        /// </summary>
        maximum = 7,

        /// <summary>
        /// The extended maximum number of supported credential types that now 
        /// allow new applications to run on older operating systems.
        /// Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This error is not supported.
        /// </summary>
        maximum_ex  = maximum+1000,
    };

}