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

#include <credential.h>
#include <string>
#include "credential_builder.h"

using std::nullopt;

namespace win32::credential_store::tests
{

credential_t make_credential()
{
    return build_credential<wchar_t>(get_id(), get_username(), get_secret(), get_credential_type(), get_persistence_type(), get_last_updated())
        .value<credential_t>();
}
TEST(credential, build_credential__returns_error__when_id_is_empty)
{
    auto const result = build_credential<wchar_t>(L"", L"username", L"secret", credential_type::generic, persistence_type::local_machine, std::nullopt);
    ASSERT_TRUE(result.has_value<result_t>());
}
TEST(credential, build_credential__error_is_invalid_argument__when_id_is_empty)
{
    auto const result = build_credential<wchar_t>(L"", L"username", L"secret", credential_type::generic, persistence_type::local_machine, std::nullopt);
    ASSERT_EQ(result.value<result_t>().error().value(), static_cast<int>(std::errc::invalid_argument));
}
TEST(credential, build_credential__returns_error__when_credential_type_is_unknown)
{
    auto const result = build_credential<wchar_t>(L"id", L"username", L"secret", credential_type::unknown, persistence_type::local_machine, std::nullopt);
    ASSERT_TRUE(result.has_value<result_t>());
}
TEST(credential, build_credential__error_is_invalid_argument__when_credential_type_is_unknown)
{
    auto const result = build_credential<wchar_t>(L"id", L"username", L"secret", credential_type::unknown, persistence_type::local_machine, std::nullopt);
    ASSERT_EQ(result.value<result_t>().error().value(), static_cast<int>(std::errc::invalid_argument));
}
TEST(credential, build_credential__returns_error__when_persistence_type_is_unknown)
{
    auto const result = build_credential<wchar_t>(L"id", L"username", L"secret", credential_type::generic, persistence_type::unknown, std::nullopt);
    ASSERT_TRUE(result.has_value<result_t>());
}
TEST(credential, build_credential__error_is_invalid_argument__when_persistence_type_is_unknown)
{
    auto const result = build_credential<wchar_t>(L"id", L"username", L"secret", credential_type::generic, persistence_type::unknown, std::nullopt);
    ASSERT_EQ(result.value<result_t>().error().value(), static_cast<int>(std::errc::invalid_argument));
}

TEST(credential, constructor__throws_invalid_argument__when_id_is_empty)
{
    ASSERT_THROW(
        static_cast<void>(credential<wchar_t>(L"", L"username", L"secret", credential_type::generic, persistence_type::local_machine, std::nullopt)),
        std::invalid_argument);
}
TEST(credential, constructor__throws_invalid_argument__when_credential_type_is_unknown)
{
    ASSERT_THROW(
        static_cast<void>(credential<wchar_t>(L"id", L"username", L"secret", credential_type::unknown, persistence_type::local_machine, std::nullopt)),
        std::invalid_argument);
}
TEST(credential, constructor__throws_invalid_argument__when_persistence_type_is_unknown)
{
    ASSERT_THROW(
        static_cast<void>(credential<wchar_t>(L"id", L"username", L"secret", credential_type::generic, persistence_type::unknown, std::nullopt)),
        std::invalid_argument);
}

TEST(credential, deconstruct__returns_given_id__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(username);
    static_cast<void>(secret);
    static_cast<void>(credential_type);
    static_cast<void>(persistence_type);
    static_cast<void>(last_updated);

    ASSERT_EQ(id, get_id());
}
TEST(credential, deconstruct__returns_given_username__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(id);
    static_cast<void>(secret);
    static_cast<void>(credential_type);
    static_cast<void>(persistence_type);
    static_cast<void>(last_updated);

    ASSERT_EQ(username, get_username());
}
TEST(credential, deconstruct__returns_given_secret__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(id);
    static_cast<void>(username);
    static_cast<void>(credential_type);
    static_cast<void>(persistence_type);
    static_cast<void>(last_updated);

    ASSERT_EQ(secret, get_secret());
}
TEST(credential, deconstruct__returns_given_credential_type__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(id);
    static_cast<void>(username);
    static_cast<void>(secret);
    static_cast<void>(persistence_type);
    static_cast<void>(last_updated);

    ASSERT_EQ(credential_type, get_credential_type());
}
TEST(credential, deconstruct__returns_given_persistence_type__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(id);
    static_cast<void>(username);
    static_cast<void>(secret);
    static_cast<void>(credential_type);
    static_cast<void>(last_updated);

    ASSERT_EQ(persistence_type, get_persistence_type());
}
TEST(credential, deconstruct__returns_given_last_updated__always)
{
    auto const [id, username, secret, credential_type, persistence_type, last_updated] = make_credential().deconstruct();
    static_cast<void>(id);
    static_cast<void>(username);
    static_cast<void>(secret);
    static_cast<void>(credential_type);
    static_cast<void>(persistence_type);

    ASSERT_EQ(last_updated, get_last_updated());
}

TEST(credential, equals__returns_true__when_values_equal)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder.build_if_valid().value();

    bool const is_equal = left.equals(right);

    ASSERT_TRUE(is_equal);
}
TEST(credential, equals__returns_false__when_values_not_equal)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder
        .with_id(to_upper(left.get_id()))
        .build_if_valid()
        .value();

    bool const is_equal = left.equals(right);

    ASSERT_FALSE(is_equal);
}

TEST(credential, equal__returns_true__when_values_equal)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder.build_if_valid().value();

    bool const is_equal = equal(left, right);

    ASSERT_TRUE(is_equal);
}
TEST(credential, equal__returns_false__when_values_not_equal)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder
        .with_id(to_upper(left.get_id()))
        .build_if_valid()
        .value();

    bool const is_equal = equal(left, right);

    ASSERT_FALSE(is_equal);
}

TEST(credential, hash__returns_equal_code__when_using_equal_values)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder.build_if_valid().value();

    EXPECT_TRUE(equal(left, right));

    auto const left_code = std::hash<credential_t>()(left);
    auto const right_code = std::hash<credential_t>()(right);

    ASSERT_EQ(left, right);
}

TEST(credential, hash__returns_non_equal_code__when_using_non_equal_values)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder
        .with_id(to_upper(left.get_id()))
        .build_if_valid()
        .value();

    EXPECT_FALSE(equal(left, right));

    auto const left_code = std::hash<credential_t>()(left);
    auto const right_code = std::hash<credential_t>()(right);

    ASSERT_NE(left, right);
}

TEST(credential, swap__swaps_left_and_right__always)
{
    auto builder = initialize_builder();
    auto left = builder.build_if_valid().value();
    auto right = builder
        .with_id(to_upper(left.get_id()))
        .with_username(to_upper(left.get_username()))
        .build_if_valid()
        .value();

    auto const expected_right_id = left.get_id();

    swap(left, right);

    ASSERT_EQ(expected_right_id, right.get_id());
}

}
