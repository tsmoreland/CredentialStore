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

#pragma once

#include <string>
#include <optional>
#include <system_error>

namespace win32::credential_store
{
    class result_t final 
    {
        std::string m_error_message{};
        std::optional<std::error_code> m_error_code;

    public:
        using optional_error_code = std::optional<std::error_code>;
        static const DWORD SUCCESS = 0UL;

        [[nodiscard]] static result_t from_error_code(std::errc const error_code)
        {
            return from_error_code("", error_code);
        }
        [[nodiscard]] static result_t from_error_code(char const* message, std::errc const error_code)
        {
            return result_t(message, std::make_error_code(error_code));
        }
        [[nodiscard]] static result_t from_win32_error(DWORD const& error_value)
        {
            return from_win32_error("", error_value);
        }
        [[nodiscard]] static result_t from_win32_error(char const* message, DWORD const& error_value)
        {
            return error_value == SUCCESS
                ? success() 
                : result_t(message, optional_error_code(std::error_code(error_value, std::system_category())));

        }
        [[nodiscard]] static result_t success()
        {
            return result_t();
        }

        [[nodiscard]] constexpr std::optional<std::error_code> const& value() const noexcept
        {
            return m_error_code;
        }
        [[nodiscard]] constexpr std::string const& message() const noexcept
        {
            return m_error_message;
        }

        [[nodiscard]] explicit operator bool() const
        {
            return !m_error_code.has_value();
        }

        explicit result_t() 
            : result_t("", std::nullopt)
        {
        }
        explicit result_t(optional_error_code const result_code) 
            : result_t("", result_code)
        {
        }
        explicit result_t(char const* message) 
            : result_t(message, std::nullopt)
        {
        }
        explicit result_t(char const* message, optional_error_code const result_code)
            : m_error_message{message}
            , m_error_code(result_code)
        {
        }

    };
    
}
