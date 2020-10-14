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

#include "credential_executor_test_fixture.h"

namespace win32::credential_store::tests
{

void credential_executor_test_fixture::SetUp()
{
    m_buffer.pubseekpos(0);
}
void credential_executor_test_fixture::TearDown()
{
    /// ... nothing to tear down at this time ...
}

credential_executor_test_fixture::credential_executor_test_fixture()
    : m_stream(&m_buffer)
    , m_executor{m_manager, m_stream}
{

}

credential_executor_test_fixture::mock_credential_manager::credentials_or_error_detail
    credential_executor_test_fixture::mock_credential_manager::get_credentials() const
{
    return m_manager.get_credentials();
}

result_t credential_executor_test_fixture::mock_credential_manager::add_or_update(credential_t const& credential) const
{
    return m_manager.add_or_update(credential);
}

credential_executor_test_fixture::mock_credential_manager::credential_or_error_detail
    credential_executor_test_fixture::mock_credential_manager::find(wchar_t const* id, credential_type type) const
{
    return m_manager.find(id, type);
}

credential_executor_test_fixture::mock_credential_manager::credentials_or_error_detail
    credential_executor_test_fixture::mock_credential_manager::find(wchar_t const* filter, bool const search_all) const
{
    return m_manager.find(filter, search_all);
}

result_t credential_executor_test_fixture::mock_credential_manager::remove(credential_t const& credential) const
{
    return m_manager.remove(credential);
}

}
