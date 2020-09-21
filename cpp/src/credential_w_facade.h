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
#include <string>
#include <wincred.h>

namespace win32::credential_store
{

    class credential_w_facade
    {
    public:
        CREDENTIALW& get_credential() 
        {
            return m_credential;
        }
        credential_w_facade& set_username(std::wstring const& username)
        {
            wstring_copy(&m_credential.UserName, username.c_str(), username.size());
            return *this;
        }
        credential_w_facade& set_target_name(std::wstring const& target_name)
        {
            wstring_copy(&m_credential.TargetName, target_name.c_str(), target_name.size());
            return *this;
        }
        credential_w_facade& set_credential_blob(std::wstring const& credential_blob)
        {
            auto const size = credential_blob.size() * sizeof(wchar_t);
            m_credential.CredentialBlob = new (std::nothrow) unsigned char[size]; 
            if (m_credential.CredentialBlob == nullptr)
                return *this;
            memcpy(m_credential.CredentialBlob, credential_blob.c_str(), size); 
            m_credential.CredentialBlobSize = static_cast<DWORD>(size);
            return *this;
        }
        credential_w_facade& set_credential_type(DWORD const type)
        {
            m_credential.Type = type;
            return *this;
        }
        credential_w_facade& set_persistence_type(DWORD const type)
        {
            m_credential.Persist = type;
            return *this;
        }

        credential_w_facade() = default;
        credential_w_facade(credential_w_facade const&) = delete;
        credential_w_facade(credential_w_facade &&) = delete;
        credential_w_facade& operator=(credential_w_facade const&) = delete;
        credential_w_facade& operator=(credential_w_facade &&) = delete;
        ~credential_w_facade()
        {
            delete[] m_credential.TargetName;
            delete[] m_credential.UserName;
            delete[] m_credential.CredentialBlob;
        }
    private:
        CREDENTIALW m_credential{};

        static void wstring_copy(wchar_t** destination, wchar_t const * const source, size_t size)
        {
            *destination = new (std::nothrow) wchar_t[size + 1];
            if (*destination == nullptr)
                return;
            wmemcpy(*destination, source, size);
            (*destination)[size] = L'\0';
        }
    };
    
}
