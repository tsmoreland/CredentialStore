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

#pragma warning(push)
#pragma warning(disable : 26495 26812)
#include <gtest/gtest.h>
#pragma warning(pop)

#include "../src/credential_manager_impl_using_traits.h"
#include "mock_credential_factory_traits.h"
#include "mock_credential_traits.h"
#include "credential_builder.h"

namespace win32::credential_store::tests
{

using credential_manager_test = credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>;
    
TEST(credential_manager, add_or_update__returns_invalid_argument__id_is_empty)
{
    auto const credential = make_credential();
    auto& id = const_cast<std::wstring&>(credential.get_id());
    id = L"";

    credential_manager_test manager;

    auto const result = manager.add_or_update(credential);

    ASSERT_TRUE(result.error_equals(std::errc::invalid_argument));
}

TEST(credential_manager, add_or_update__returns_invalid_argument__username_is_empty)
{
    auto const credential = make_credential();
    auto& username = const_cast<std::wstring&>(credential.get_username());
    username = L"";

    credential_manager_test manager;

    auto const result = manager.add_or_update(credential);

    ASSERT_TRUE(result.error_equals(std::errc::invalid_argument));
}

TEST(credential_manager, add_or_update__returns_error__api_returns_error)
{
    mock_credential_traits::set_cred_write_result(42UL);
    credential_manager_test manager;
    auto const credential{make_credential()};

    auto const result = manager.add_or_update(credential);

    ASSERT_TRUE(result.error_equals(42UL));
}

}
