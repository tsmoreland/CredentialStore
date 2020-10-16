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

void credential_executor_test_fixture::SetUp()
{
    m_buffer.pubseekpos(0);
}
void credential_executor_test_fixture::TearDown()
{
    /// ... nothing to tear down at this time ...
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
