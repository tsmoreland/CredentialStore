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

// ignore methods that may be static to keep up appearances of them being part of the fixture
// ReSharper disable CppMemberFunctionMayBeStatic 

#include "credential_manager_test_fixture.h"

namespace win32::credential_store::tests
{

credential_manager_test_fixture::credential_manager_test_fixture()
    : m_id{L"ARBITRARY_IDENTIFIER"}
    , m_type{credential_type::generic}
{
}

void credential_manager_test_fixture::SetUp()
{
    m_manager = credential_manager_test();
    m_credential = make_credential();

    set_cred_delete_result(0UL);
    set_cred_enumerate_result(0UL);
    set_cred_read_result(0UL);
    set_cred_write_result(0UL);
}

void credential_manager_test_fixture::TearDown()
{
    // nothing to tear down at this time...
}

wchar_t const* credential_manager_test_fixture::id() const noexcept
{
    return m_id;
}

credential_type credential_manager_test_fixture::type() const noexcept
{
    return m_type;
}

credential_manager_test& credential_manager_test_fixture::manager()
{
    return m_manager.value();
}

credential_t& credential_manager_test_fixture::credential()
{
    return m_credential.value();
}

void credential_manager_test_fixture::set_cred_read_result(DWORD const value) const noexcept
{
    mock_credential_traits::s_cred_read_result = value;
}
void credential_manager_test_fixture::set_cred_write_result(DWORD const value) const noexcept
{
    mock_credential_traits::s_cred_write_result = value;
}
void credential_manager_test_fixture::set_cred_enumerate_result(DWORD const value) const noexcept
{
    mock_credential_traits::s_cred_enumerate_result = value;
}
void credential_manager_test_fixture::set_cred_delete_result(DWORD const value) const noexcept
{
    mock_credential_traits::s_cred_delete_result = value;
}

}
