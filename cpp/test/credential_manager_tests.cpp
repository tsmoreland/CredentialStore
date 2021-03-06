//
// Copyright � 2020 Terry Moreland
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

TEST_F(credential_manager_test_fixture, add_or_update__returns_invalid_argument__id_is_empty)
{
    auto& id = const_cast<std::wstring&>(credential().get_id());
    id = L"";

    auto const result = manager().add_or_update(credential());

    ASSERT_EQ(result, std::errc::invalid_argument);
}
TEST_F(credential_manager_test_fixture, add_or_update__returns_invalid_argument__username_is_empty)
{
    auto& username = const_cast<std::wstring&>(credential().get_username());
    username = L"";

    auto const result = manager().add_or_update(credential());

    ASSERT_EQ(result, std::errc::invalid_argument);
}
TEST_F(credential_manager_test_fixture, add_or_update__returns_error__api_returns_error)
{
    set_cred_write_result(42UL);

    auto const result = manager().add_or_update(credential());

    ASSERT_EQ(result, 42UL);
}

TEST_F(credential_manager_test_fixture, find__returns_matching_credential__when_api_returns_success)
{
    mock_credential_factory_traits::set_credential(&credential());
    set_cred_read_result(0UL);

    auto const match = manager().find(id(), type()).value<credential_t>();

    ASSERT_EQ(credential(), match);
}
TEST_F(credential_manager_test_fixture, find__returns_not_found__when_api_returns_not_found)
{
    set_cred_read_result(ERROR_NOT_FOUND);

    auto const result = manager().find(id(), type()).value<result_t>();

    ASSERT_EQ(result, ERROR_NOT_FOUND);
}
TEST_F(credential_manager_test_fixture, find__returns_error__when_api_returns_error)
{
    set_cred_read_result(42UL);

    auto const result = manager().find(id(), type()).value<result_t>();

    ASSERT_EQ(result, 42UL);
}

TEST_F(credential_manager_test_fixture, remove__by_id__returns_success__when_api_returns_success)
{
    set_cred_delete_result(0UL);

    auto const result = manager().remove(id(), type());

    ASSERT_TRUE(result.is_success());
}
TEST_F(credential_manager_test_fixture, remove__by_id__returns_success__when_not_found)
{
    set_cred_delete_result(ERROR_NOT_FOUND);

    auto const result = manager().remove(id(), type());

    ASSERT_TRUE(result.is_success());
}
TEST_F(credential_manager_test_fixture, remove__by_id__returns_error__when_api_returns_error)
{
    set_cred_delete_result(42UL);

    auto const result = manager().remove(id(), type());

    ASSERT_EQ(result, 42UL);
}

TEST_F(credential_manager_test_fixture, remove__by_object__returns_success__when_api_returns_success)
{
    set_cred_delete_result(0UL);

    auto const result = manager().remove(credential());

    ASSERT_TRUE(result.is_success());
}
TEST_F(credential_manager_test_fixture, remove__by_object__returns_success__when_not_found)
{
    set_cred_delete_result(ERROR_NOT_FOUND);

    auto const result = manager().remove(credential());

    ASSERT_TRUE(result.is_success());
}
TEST_F(credential_manager_test_fixture, remove__by_object__returns_error__when_api_returns_error)
{
    set_cred_delete_result(42UL);

    auto const result = manager().remove(credential());

    ASSERT_EQ(result, 42UL);
}

}
