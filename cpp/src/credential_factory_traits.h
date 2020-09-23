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

#include <Windows.h>
#include <wincred.h>
#include <credential.h>

namespace win32::credential_store
{
    struct credential_factory_traits final
    {
        using credential_t = credential<wchar_t>;

        [[nodiscard]] static credential_t from_win32_credential(CREDENTIALW const * const credential_ptr)
        {
            return credential_t(
                value_or_empty(credential_ptr->TargetName),
                value_or_empty(credential_ptr->UserName),
                get_secret(credential_ptr),
                to_credential_type(credential_ptr->Type),
                to_persistence_type(credential_ptr->Persist),
                std::nullopt);
        }

            /*
        static CREDENTIALW to_credential_w(credential_t const& credential)
        {
            CREDENTIALW credential_w{};
             TODO: change return type to tuple containing the CREDENTIAL_W + unique_ptr to the strings?
                   -or- take the easy route and use &credential.get_id()[0]
            credential_w.TargetName = credential.get_id().c_str();
            credential_w.UserName = credential.get_username().c_str();
            credential_w.Type = 
            credential_w.
            credential_w.
            credential_w.
        }
                */

    private:
        static credential_type to_credential_type(DWORD const type)
        {
            switch (type) {
            case CRED_TYPE_DOMAIN_PASSWORD:
                return credential_type::domain_password;
            case CRED_TYPE_GENERIC:
                return credential_type::generic;
            case CRED_TYPE_DOMAIN_CERTIFICATE:
                return credential_type::domain_certificate;
            case CRED_TYPE_DOMAIN_EXTENDED:
                return credential_type::domain_extended;
            case CRED_TYPE_DOMAIN_VISIBLE_PASSWORD:
                return credential_type::domain_visible_password;
            case CRED_TYPE_GENERIC_CERTIFICATE:
                return credential_type::generic_certificate;
            case CRED_TYPE_MAXIMUM:
                return credential_type::maximum;
            case CRED_TYPE_MAXIMUM_EX:
                return credential_type::maximum_ex;
            default:
                return credential_type::Unknown;
            }
        }
        static persistence_type to_persistence_type(DWORD const type)
        {
            switch (type) {
            case CRED_PERSIST_ENTERPRISE:
                return persistence_type::enterprise;
            case CRED_PERSIST_LOCAL_MACHINE:
                return persistence_type::local_machine;
            case CRED_PERSIST_SESSION:
                return persistence_type::session;
            default:
                return persistence_type::Unknown;
            }
        }

        static std::wstring get_secret(CREDENTIALW const*const credential_ptr)
        {
            if (credential_ptr->CredentialBlobSize == 0UL || credential_ptr->CredentialBlob == nullptr) {
                return std::wstring();
            }

            switch (credential_ptr->Type) {
            case CRED_TYPE_DOMAIN_PASSWORD:
            case CRED_TYPE_GENERIC:
                return std::wstring(reinterpret_cast<wchar_t*>(credential_ptr->CredentialBlob), credential_ptr->CredentialBlobSize / sizeof(wchar_t));
            default:
                return std::wstring();
            }
        }
        static wchar_t const* value_or_empty(wchar_t const* const value)
        {
            return value != nullptr
                ? value
                : L"";
        }
    };
    
}
