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
#include <result_code.h>

namespace win32::credential_store
{

    class result_detail final 
    {
        std::string m_error_message{};
        result_code m_result_code;

    public:
        explicit result_detail() 
            : result_detail("", result_code::success)
        {
        }
        explicit result_detail(result_code const result_code) 
            : result_detail("", result_code)
        {
        }
        explicit result_detail(char const* message) 
            : result_detail(message, result_code::unknown)
        {
        }
        explicit result_detail(char const* message, result_code const result_code)
            : m_error_message{message}
            , m_result_code(result_code)
        {
        }

        [[nodiscard]] constexpr result_code value() const noexcept
        {
            return m_result_code;
        }
        [[nodiscard]] constexpr std::string const& message() const noexcept
        {
            return m_error_message;
        }

        [[nodiscard]] explicit operator bool() const
        {
            return m_result_code == result_code::success;
        }

    };
    
}
