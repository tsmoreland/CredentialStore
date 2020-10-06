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

#define WIN32_LEAN_AND_MEAN

#include <credential_type.h>
#include <memory>
#include <Windows.h>
#include <wincred.h>
#include <credential_store_export.h>

namespace win32::credential_store
{

    struct WIN32_CREDENTIAL_STORE_EXPORT credential_traits final
    {
    private:
        static const DWORD SUCCESS = 0;

    public:
        using unique_credential_w = std::unique_ptr<CREDENTIALW, void (*)(CREDENTIALW*)>;  
        using unique_credentials_w = std::unique_ptr<CREDENTIALW*, void (*)(CREDENTIALW*)>;  

        [[nodiscard]] static DWORD cred_read(wchar_t const* id, credential_type const type, unique_credential_w& out_credential);
        [[nodiscard]] static DWORD cred_write(CREDENTIALW * const  credential, DWORD const flags);
        [[nodiscard]] static DWORD cred_enumerate(wchar_t const* filter, DWORD const flags, DWORD& count, CREDENTIALW**& credentials);
        [[nodiscard]] static DWORD cred_delete(wchar_t const* id, credential_type const type);

        static void credential_deleter(CREDENTIALW* credential_ptr);
        static void credential_deleter(CREDENTIALW** credential_ptr);

        [[nodiscard]] static DWORD to_dword(credential_type const type);
        [[nodiscard]] static DWORD to_dword(persistence_type const type);

    };
    
}
