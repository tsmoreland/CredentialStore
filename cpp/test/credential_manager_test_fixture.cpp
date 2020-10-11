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
    std::optional<credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>> obj{std::nullopt};
    //credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits> obj2;
    std::optional<credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>> obj2 = std::make_optional<credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>>();
    obj = std::move(obj2);
    //obj = obj2;

        //std::make_optional( credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>());
    //m_manager = std::make_optional(credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>());
    //m_manager = std::make_optional<credential_manager_test>();
    //m_manager = std::make_optional<credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>>();
    m_manager = 
    m_credential = make_credential();
}

void credential_manager_test_fixture::TearDown()
{
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

}
