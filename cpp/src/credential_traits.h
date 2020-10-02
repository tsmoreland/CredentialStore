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

#include <credential_type.h>
#include <memory>
#include <Windows.h>
#include <wincred.h>

namespace win32::credential_store
{
    using unique_credential_w = std::unique_ptr<CREDENTIALW, void (*)(CREDENTIALW*)>;  
    using unique_credentials_w = std::unique_ptr<CREDENTIALW*, void (*)(CREDENTIALW*)>;  

    struct credential_traits final
    {

        [[nodiscard]] static DWORD cred_read(wchar_t const* id, credential_type const type, unique_credential_w& out_credential)
        {
            // this needs reworked to output a unique_ptr with deleter calling CredFree which can be cleaned up by the caller
            CREDENTIALW* credential_ptr{nullptr};
            auto const result = CredReadW(id, to_dword(type), 0, &credential_ptr);
            if (result == TRUE) {
                out_credential = unique_credential_w(credential_ptr, credential_deleter);
                credential_ptr = nullptr;
                return SUCCESS;
            } 
            return GetLastError();
        }

        [[nodiscard]] static DWORD cred_write(PCREDENTIALW credential, DWORD const flags)
        {
            auto const result = CredWriteW(credential, flags);
            return result == TRUE 
                ? SUCCESS
                : GetLastError();
        }

        [[nodiscard]] static DWORD cred_enumerate(wchar_t const* filter, DWORD const flags, DWORD& count, CREDENTIALW**& credentials)
        {
#pragma warning(push)
#pragma warning(disable : 6388) // flags might not be 0, spec says it can be 0x1 or CRED_ENUMERATE_ALL_CREDENTIALS
            auto const result = CredEnumerateW(filter, flags, &count, &credentials);
#pragma warning(pop)
            return result == TRUE 
                ? SUCCESS
                : GetLastError();
        }
        

        [[nodiscard]] static DWORD cred_delete(wchar_t const* id, credential_type const type)
        {
            return CredDeleteW(id, to_dword(type), 0) == TRUE
                ? SUCCESS
                : GetLastError();
        }

        static void credential_deleter(CREDENTIALW* credential_ptr)
        {
            CredFree(credential_ptr);
        }
        static void credential_deleter(CREDENTIALW** credential_ptr)
        {
            CredFree(credential_ptr);
        }

        [[nodiscard]] static DWORD to_dword(credential_type const type)
        {
            using underlying_type = std::underlying_type<credential_type>::type;
            return static_cast<DWORD>(static_cast<underlying_type>(type));
        }

        [[nodiscard]] static DWORD to_dword(persistence_type const type)
        {
            using underlying_type = std::underlying_type<persistence_type>::type;
            return static_cast<DWORD>(static_cast<underlying_type>(type));
        }
    private:
        static const DWORD SUCCESS = 0;

    };
    
}
