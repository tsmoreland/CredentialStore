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
#include "verb_type.h"
#include <functional>

using std::begin;
using std::end;
using std::wcout;
using std::endl;

namespace win32::credential_store::cli
{
    [[nodiscard]] verb_type get_verb_type(std::string_view const& verb);
    [[nodiscard]] credential_type get_credential_type(std::string_view const& type);
    void print_unreognized_type();

    void print_credential(credential<wchar_t> const& credential) {
        wcout << credential.get_id() << L" " << credential.get_username() << endl;
    }

    credential_executor::credential_executor(credential_manager_interface const& manager)
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
    if (arguments.size() < 3) {
        wcout << L"Usage: credential_store.cli add <type> <target> <username>" << endl;
        return;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {

        std::wstring const& id{begin(arguments[1]), end(arguments[1])};
        std::wstring const& username{begin(arguments[2]), end(arguments[2])};

        credential<wchar_t> credential{id, username, L"", type, persistence_type::local_machine, std::nullopt};

    } else {
        print_unreognized_type();
    }

    static_cast<void>(m_manager);
}
void credential_executor::find(std::vector<std::string_view> const& arguments) const
{
    if (arguments.size() < 2) {
        wcout << L"Usage: credential_store.cli find <type> <target>" << endl;
        return;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {

        try {
            std::wstring const& id{begin(arguments[1]), end(arguments[1])};

            auto credential = m_manager.find(id.c_str(), type);

            if (credential.has_value())
                print_credential(credential.value());
            else
                wcout << id << L" not found." << endl;

        } catch (std::system_error const& e) {
            std::cout << "Error occured: " << e.what() << endl;
        }

    } else {
        print_unreognized_type();
    }

    static_cast<void>(m_manager);
}
void credential_executor::list(std::vector<std::string_view> const& arguments) const
{
    static_cast<void>(arguments);

    auto credentials =  m_manager.get_credentials();

    for (auto const& credential : credentials) {
        print_credential(credential);
    }
}
void credential_executor::remove(std::vector<std::string_view> const& arguments) const
{
    if (arguments.size() < 2) {
        wcout << L"Usage: credential_store.cli remove <type> <target>" << endl;
        return;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {
        try {
            std::wstring const& id{begin(arguments[1]), end(arguments[1])};
            auto credential = m_manager.find(id.c_str(), type);

            if (credential.has_value())
                m_manager.remove(credential.value());
            else
                wcout << id << L" not found." << endl;

        } catch (std::system_error const& e) {
            std::cout << "Error occured: " << e.what() << endl;
        }

    } else {
        print_unreognized_type();
    }
    static_cast<void>(m_manager);
}

char to_upper(char const ch)
{
    return static_cast<char>(::toupper(static_cast<int>(ch)));
};

verb_type get_verb_type(std::string_view const& verb)
{
    std::string upper_verb(verb.size(), '\0');
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

credential_type get_credential_type(std::string_view const& type)
{
    std::string upper_type(type.size(), '\0');
    std::transform(begin(type), end(type), begin(upper_type), to_upper);

    if (upper_type.find("GENERIC", 0) != std::string::npos)
        return credential_type::generic;
    if (upper_type.find("DOMAIN_CERTIFICATE", 0) != std::string::npos)
        return credential_type::domain_certificate;
    if (upper_type.find("DOMAIN_EXTENDED", 0) != std::string::npos)
        return credential_type::domain_extended;
    if (upper_type.find("DOMAIN_PASSWORD", 0) != std::string::npos)
        return credential_type::domain_password;
    if (upper_type.find("DOMAIN_VISIBLE_PASSWORD", 0) != std::string::npos)
        return credential_type::domain_visible_password;
    if (upper_type.find("GENERIC_CERTIFICATE", 0) != std::string::npos)
        return credential_type::generic_certificate;
    if (upper_type.find("MAXIMUM", 0) != std::string::npos)
        return credential_type::maximum;
    if (upper_type.find("MAXIMUM_EX", 0) != std::string::npos)
        return credential_type::maximum_ex;

    return credential_type::unknown;
}
void print_unreognized_type()
{
    wcout
        << L"Unrecognized credential type, must be one of: " << endl
        << L"\tgeneric" << endl
        << L"\tdomain_certificate" << endl
        << L"\tdomain_extended" << endl
        << L"\tdomain_password" << endl
        << L"\tdomain_visible_password" << endl
        << L"\tgeneric_certificate" << endl
        << L"\tmaximum" << endl
        << L"\tmaximum_ex" << endl;
}

}
