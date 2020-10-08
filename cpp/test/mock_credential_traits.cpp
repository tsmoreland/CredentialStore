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

#include "mock_credential_traits.h"
#include "../src/credential_traits.h"

namespace win32::credential_store::tests
{
    //using unique_credential_w = std::unique_ptr<CREDENTIALW, void (*)(CREDENTIALW*)>;  
    CREDENTIALW* MOCK_CREDENTIAL_ADDRESS = reinterpret_cast<CREDENTIALW*>(0x1234);

    mock_credential_traits::credential_t* mock_credential_traits::s_mock_result_ptr = nullptr;

    DWORD mock_credential_traits::s_cred_read_result = 0UL;
    DWORD mock_credential_traits::s_cred_write_result = 0UL;
    DWORD mock_credential_traits::s_cred_enumerate_result = 0UL;
    DWORD mock_credential_traits::s_cred_delete_result = 0UL;

    DWORD mock_credential_traits::cred_read(wchar_t const*, credential_type const, unique_credential_w& out_credential)
    {
        out_credential = unique_credential_w(MOCK_CREDENTIAL_ADDRESS, mock_credential_traits::credential_deleter);

        return s_cred_read_result;
    }
    DWORD mock_credential_traits::cred_write(PCREDENTIALW, DWORD const)
    {
        return s_cred_write_result;
    }
    DWORD mock_credential_traits::cred_enumerate(wchar_t const*, DWORD const flags, DWORD& count, CREDENTIALW**& credentials)
    {
        // not implemented yet, count and credentials will be used eventually
        static_cast<void>(count);
        credentials = nullptr;
        return s_cred_enumerate_result;
    }
    DWORD mock_credential_traits::cred_delete(wchar_t const*, credential_type const)
    {
        return s_cred_delete_result;
    }
    void mock_credential_traits::credential_deleter(CREDENTIALW*)
    {
        // ... ignore ...
    }
    void mock_credential_traits::credential_deleter(CREDENTIALW**)
    {
        // ... ignore ...
    }
    DWORD mock_credential_traits::to_dword(credential_type const type)
    {
        return credential_traits::to_dword(type);
    }
    DWORD mock_credential_traits::to_dword(persistence_type const type)
    {
        return credential_traits::to_dword(type);
    }
    void mock_credential_traits::set_credential(credential_t* value) noexcept
    {
        s_mock_result_ptr = value;
    }
    void mock_credential_traits::set_cred_read_result(DWORD const value) noexcept
    {
        s_cred_read_result = value;
    }
    void mock_credential_traits::set_cred_write_result(DWORD const value) noexcept
    {
        s_cred_write_result = value;
    }
    void mock_credential_traits::set_cred_enumerate_result(DWORD const value) noexcept
    {
        s_cred_enumerate_result = value;
    }
    void mock_credential_traits::set_cred_delete_result(DWORD const value) noexcept
    {
        s_cred_delete_result = value;
    }
}