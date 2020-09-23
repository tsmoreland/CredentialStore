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
    enum class persistence_type
    {
        Unknown = 0,

        /// <summary>
        /// The credential persists for the life of the logon session. It will 
        /// not be visible to other logon sessions of this same user. It will 
        /// not exist after this user logs off and back on.
        /// </summary>
        session = 1,

        /// <summary>
        /// The credential persists for all subsequent logon sessions on this 
        /// same computer. It is visible to other logon sessions of this same 
        /// user on this same computer and not visible to logon sessions for 
        /// this user on other computers.  
        ///
        /// Windows Vista Home Basic, Windows Vista Home Premium, Windows 
        /// Vista Starter and Windows XP Home Edition:  
        /// This value is not supported.
        /// </summary>
        local_machine = 2, 

        /// <summary>
        /// The credential persists for all subsequent logon sessions on this 
        /// same computer. It is visible to other logon sessions of this same 
        /// user on this same computer and to logon sessions for this user on 
        /// other computers.
        /// This option can be implemented as locally persisted credential if 
        /// the administrator or user configures the user account to not have 
        /// roam-able state. For instance, if the user has no roaming profile, 
        /// the credential will only persist locally.
        ///
        /// Windows Vista Home Basic, Windows Vista Home Premium, 
        /// Windows Vista Starter and Windows XP Home Edition:  
        /// This value is not supported.
        /// </summary>
        enterprise = 3,
    };
}