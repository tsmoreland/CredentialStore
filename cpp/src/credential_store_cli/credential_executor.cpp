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

#include <algorithm>
#include <iostream>
#include "credential_executor.h"

#include <functional>

using std::begin;
using std::end;
using std::wcout;
using std::endl;

namespace win32::credential_store::cli
{
    credential_executor::credential_executor(credential_manager const& manager)
        : m_manager{manager}
    {
    }

    verb_processor credential_executor::get_operation(std::string_view const& verb) const
{
    auto const verb_type = get_verb_type(verb);

    switch (verb_type) {
    case verb_type::add:
        return [this](std::vector<std::string_view> const& arguments) { add(arguments); };
    case verb_type::find:
        return [this](std::vector<std::string_view> const& arguments) { find(arguments); };
    case verb_type::list:
        return [this](std::vector<std::string_view> const& arguments) { list(arguments); };
    case verb_type::remove:
        return [this](std::vector<std::string_view> const& arguments) { remove(arguments); };
    case verb_type::none:
    default:  // NOLINT(clang-diagnostic-covered-switch-default) -- without default it complains that not all paths return
        return [this](std::vector<std::string_view> const& arguments) { none(arguments); };
    }
}

void credential_executor::none(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);
    static_cast<void>(m_manager);
}

void credential_executor::add(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);
    static_cast<void>(m_manager);
}
void credential_executor::find(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);
    static_cast<void>(m_manager);
}
void credential_executor::list(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);

    auto credentials =  m_manager.get_credentials();

    for (auto const& credential : credentials) {
        wcout << credential.get_id() << L" " << credential.get_username() << endl;
    }
}
void credential_executor::remove(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);
    static_cast<void>(m_manager);
}

verb_type credential_executor::get_verb_type(std::string_view const& verb)
{
    std::string upper_verb(verb.size(), '\0');

    auto to_upper = [](auto ch) {
        return static_cast<char>(::toupper(static_cast<int>(ch)));
    };

    std::transform(begin(verb), end(verb), begin(upper_verb), to_upper);

    if (upper_verb.find("ADD", 0) != std::string::npos)
        return verb_type::add;
    if (upper_verb.find("FIND", 0) != std::string::npos)
        return verb_type::find;
    if (upper_verb.find("LIST", 0) != std::string::npos)
        return verb_type::list;
    if (upper_verb.find("REMOVE", 0) != std::string::npos)
        return verb_type::remove;
    return verb_type::none;
}

}
