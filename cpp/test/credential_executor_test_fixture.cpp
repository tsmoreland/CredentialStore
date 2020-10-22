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
#include <tuple>

namespace win32::credential_store::tests
{

using std::string_view;
using arg_container = std::vector<std::string>;
using arg_container_view = std::vector<std::string_view>;
using std::transform;

std::tuple<arg_container, arg_container_view> make_arguments(std::initializer_list<char const *> const arguments)
{
    arg_container container;
    container.insert(end(container), begin(arguments), end(arguments));
    arg_container_view container_view;
    transform(begin(container), end(container), std::back_inserter(container_view), 
        [](auto const& arg) -> string_view {
            return arg;
        });

    return std::make_tuple<arg_container, arg_container_view>(std::move(container), std::move(container_view));
}

TEST_F(credential_executor_test_fixture, none__returns_unrecognized_argument__always)
{
    auto [args, args_view] = make_arguments({"bad", "args"});
    auto const result = executor().none(args_view);

    ASSERT_EQ(cli::cli_result_code::unrecognized_argument, result);
}

TEST_F(credential_executor_test_fixture, add__returns_insuffient_arguments__when_arguments_less_than_three)
{
    auto [args, args_view] = make_arguments({"bad", "args"});

    auto const result = executor().add(args_view);

    ASSERT_EQ(cli::cli_result_code::insufficient_arguments, result);
}

TEST_F(credential_executor_test_fixture, add__returns_unrecognized_argument__when_credential_type_invalid)
{
    auto [args, args_view] = make_arguments({"invalid_type", "target", "username"});

    auto const result = executor().add(args_view);

    ASSERT_EQ(cli::cli_result_code::unrecognized_argument, result);
}
TEST_F(credential_executor_test_fixture, add__returns_success__when_arguments_valid_and_manager_succeeds)
{
    auto [args, args_view] = make_arguments({"generic", "target", "username"});

    auto const result = executor().add(args_view);

    ASSERT_EQ(cli::cli_result_code::success, result);
}

TEST_F(credential_executor_test_fixture, find__returns_insuffient_arguments__when_arguments_less_than_two)
{
    auto [args, args_view] = make_arguments({"bad_arg"});

    auto const result = executor().find(args_view);

    ASSERT_EQ(cli::cli_result_code::insufficient_arguments, result);
}

TEST_F(credential_executor_test_fixture, list__returns_success__for_any_arguments)
{
    auto [args, args_view] = make_arguments({"arguments", "are", "not", "used"});

    auto const result = executor().list(args_view);

    ASSERT_EQ(cli::cli_result_code::success, result);
}

TEST_F(credential_executor_test_fixture, remove__returns_insuffient_arguments__when_arguments_less_than_two)
{
    auto [args, args_view] = make_arguments({"bad_arg"});

    auto const result = executor().remove(args_view);

    ASSERT_EQ(cli::cli_result_code::insufficient_arguments, result);
}

class credential_type_parse_test_fixture
    : public ::testing::TestWithParam<std::tuple<char const *, credential_type>>
{
};
TEST_P(credential_type_parse_test_fixture, get_credential_type__returns_non_unknown_value__when_string_matches_type)
{
    auto [credential_type_as_string, expected_type]  = GetParam();

    auto const actual_type = cli::get_credential_type(credential_type_as_string);

    ASSERT_EQ(expected_type, actual_type);
}
char const *valid_credential_type_strings[] = { "generic", "domain_certificate", "DOMAIN_extended", "DOMAIN_PASSWORD"};
std::tuple<char const *, credential_type> string_to_credential_type_pairs[] = 
{
    std::make_tuple("generic", credential_type::generic),
    std::make_tuple("domain_CERTIFICATE", credential_type::domain_certificate),
    std::make_tuple("DOMAIN_EXTENDED", credential_type::domain_extended),
    std::make_tuple("DOMAIN_password", credential_type::domain_password),
    std::make_tuple("domain_VISIBLE_password", credential_type::domain_visible_password),
    std::make_tuple("generic_certificate", credential_type::generic_certificate),
    std::make_tuple("maximum", credential_type::maximum),
    std::make_tuple("MAXIMUM_EX", credential_type::maximum_ex),
    std::make_tuple("invalid or unkonwn", credential_type::unknown),
};
INSTANTIATE_TEST_SUITE_P(valid_types, credential_type_parse_test_fixture, ::testing::ValuesIn(string_to_credential_type_pairs));

void credential_executor_test_fixture::SetUp()
{
    m_buffer.pubseekpos(0);

    set_cred_delete_result(0UL);
    set_cred_enumerate_result(0UL);
    set_cred_read_result(0UL);
    set_cred_write_result(0UL);
}
void credential_executor_test_fixture::TearDown()
{
    /// ... nothing to tear down at this time ...
}

credential_executor_test_fixture::mock_credential_manager const& credential_executor_test_fixture::manager() const
{
    return m_manager;
}

cli::credential_executor const& credential_executor_test_fixture::executor() const
{
    return m_executor;
}

std::wostream& credential_executor_test_fixture::stream()
{
    return m_stream;
}

credential_executor_test_fixture::credential_executor_test_fixture()
    : m_stream(&m_buffer)
    , m_secret_provider{[]() { return L"password"; }}
    , m_executor{m_manager, m_stream, m_secret_provider}
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

void credential_executor_test_fixture::set_cred_read_result(DWORD const value) noexcept
{
    mock_credential_traits::s_cred_read_result = value;
}
void credential_executor_test_fixture::set_cred_write_result(DWORD const value) noexcept
{
    mock_credential_traits::s_cred_write_result = value;
}
void credential_executor_test_fixture::set_cred_enumerate_result(DWORD const value) noexcept
{
    mock_credential_traits::s_cred_enumerate_result = value;
}
void credential_executor_test_fixture::set_cred_delete_result(DWORD const value) noexcept
{
    mock_credential_traits::s_cred_delete_result = value;
}

}
