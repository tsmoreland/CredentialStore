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

#include <optional>
#include <credential_type.h>
#include <persistence_type.h>
#include "credential_factory_traits.h"

namespace win32::credential_store
{

credential_factory_traits::credential_t credential_factory_traits::from_win32_credential(CREDENTIALW const * const credential_ptr)
{
    return credential_t(
        value_or_empty(credential_ptr->TargetName),
        value_or_empty(credential_ptr->UserName),
        get_secret(credential_ptr),
        to_credential_type(credential_ptr->Type),
        to_persistence_type(credential_ptr->Persist),
        std::nullopt);
}

credential_type credential_factory_traits::to_credential_type(DWORD const type)
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
        return credential_type::unknown;
    }
}
persistence_type credential_factory_traits::to_persistence_type(DWORD const type)
{
    switch (type) {
    case CRED_PERSIST_ENTERPRISE:
        return persistence_type::enterprise;
    case CRED_PERSIST_LOCAL_MACHINE:
        return persistence_type::local_machine;
    case CRED_PERSIST_SESSION:
        return persistence_type::session;
    default:
        return persistence_type::unknown;
    }
}

std::wstring credential_factory_traits::get_secret(CREDENTIALW const*const credential_ptr)
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
wchar_t const* credential_factory_traits::value_or_empty(wchar_t const* const value)
{
    return value != nullptr
        ? value
        : L"";
}
    
}
