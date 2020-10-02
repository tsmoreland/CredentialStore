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
#include <credential.h>
#include <string>
#pragma warning(pop)

using namespace win32::credential_store;

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

