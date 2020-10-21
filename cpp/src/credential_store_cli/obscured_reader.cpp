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

#include "obscured_reader.h"
#include <iostream>
#include <sstream>
#include <Windows.h>

namespace win32::credential_store::cli
{
    using std::wstring;
    using std::wcout;
    using std::endl;
    using const_handle = void* const;

    class console_mode_setting final
    {
        const_handle m_input_handle;
        DWORD m_mode;
    public:
        explicit console_mode_setting(const_handle input_handle)
            : m_input_handle(input_handle)
            , m_mode{0UL}
        {
            GetConsoleMode(input_handle, &m_mode);
        }
        [[nodiscard]]
        DWORD get_mode() const
        {
            return m_mode;
        }
        void set_mode(DWORD const mode) const
        {
            SetConsoleMode(m_input_handle, mode);
        }
        console_mode_setting(console_mode_setting const&) = delete;
        console_mode_setting(console_mode_setting&&) noexcept = delete;
        console_mode_setting& operator=(console_mode_setting const&) = delete;
        console_mode_setting& operator=(console_mode_setting&&) noexcept = delete;
        ~console_mode_setting()
        {
            SetConsoleMode(m_input_handle, m_mode);
        }
    };

    std::wstring read_secret(wchar_t const* prompt)
    {
        wchar_t const BACKSPACE = 8;
        wchar_t const RETURN=13;

        if (prompt != nullptr && wcslen(prompt) != 0) {
            std::wcout << prompt << L" ";
        }

        const_handle standard_input_handle = GetStdHandle(STD_INPUT_HANDLE);

        console_mode_setting const console_mode(standard_input_handle);
        console_mode.set_mode(console_mode.get_mode() & ~(ENABLE_ECHO_INPUT | ENABLE_LINE_INPUT));

        std::wstringstream stream;
        size_t count{0};
        DWORD character_read{0};
        wchar_t input{0};
        while(ReadConsoleW(standard_input_handle, &input, 1, &character_read, nullptr) && input != RETURN) {
            if (input == BACKSPACE) {
                if (count == 0) {
                    wcout << L"\b \b";
                }
                continue;
            }
            count++;
            stream << input;
        }
        wcout <<endl;
        return stream.str();
    }
}
