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

void print_unreognized_type();

void print_credential(credential<wchar_t> const& credential, std::wostream& output_stream) {
    output_stream << credential.get_id() << L" " << credential.get_username() << endl;
}

credential_executor::credential_executor(credential_manager_interface const& manager, std::wostream& output_stream)
    : m_manager{manager}
    , m_output_stream(output_stream)
{
}

verb_processor credential_executor::get_operation(std::string_view const& verb) const
{
    auto const verb_type = get_verb_type(verb);

    switch (verb_type) {
    case verb_type::add:
        return [this](std::vector<std::string_view> const& arguments) { return add(arguments); };
    case verb_type::find:
        return [this](std::vector<std::string_view> const& arguments) { return find(arguments); };
    case verb_type::list:
        return [this](std::vector<std::string_view> const& arguments) { return list(arguments); };
    case verb_type::remove:
        return [this](std::vector<std::string_view> const& arguments) { return remove(arguments); };
    case verb_type::none:
    default:  // NOLINT(clang-diagnostic-covered-switch-default) -- without default it complains that not all paths return
        return [this](std::vector<std::string_view> const& arguments) { return none(arguments); };
    }
}

cli_result_code credential_executor::none(argument_container_view const& arguments) const
{
    static_cast<void>(arguments);
    static_cast<void>(m_manager);
    return cli_result_code::unrecognized_argument;
}

cli_result_code credential_executor::add(argument_container_view const& arguments) const
{
    if (arguments.size() < 3) {
        m_output_stream << L"Usage: credential_store.cli add <type> <target> <username>" << endl;
        return cli_result_code::insufficient_arguments;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {

        std::wstring const& id{begin(arguments[1]), end(arguments[1])};
        std::wstring const& username{begin(arguments[2]), end(arguments[2])};

        credential<wchar_t> credential{id, username, L"", type, persistence_type::local_machine, std::nullopt};

    } else {
        print_unreognized_type();
        return cli_result_code::unrecognized_argument;
    }

    static_cast<void>(m_manager);
    return cli_result_code::success;
}
cli_result_code credential_executor::find(argument_container_view const& arguments) const
{
    if (arguments.size() < 2) {
        m_output_stream << L"Usage: credential_store.cli find <type> <target>" << endl;
        return cli_result_code::insufficient_arguments;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {

        std::wstring const& id{begin(arguments[1]), end(arguments[1])};

        if (auto credential = m_manager.find(id.c_str(), type);
            credential.has_value<win32::credential_store::credential<wchar_t>>()) {
            print_credential(credential.value<win32::credential_store::credential<wchar_t>>(), m_output_stream);

        } else if (credential.value<result_t>().equals(ERROR_NOT_FOUND)) {
            m_output_stream << id << L" not found." << endl;

        } else {
            auto const& result = credential.value<result_t>();
            std::wstring const message(std::begin(result.message()), std::end(result.message()));
            m_output_stream << L"Error occured: " << result.error().value() << L" " << message << endl;
        }

    } else {
        print_unreognized_type();
        return cli_result_code::unrecognized_argument;
    }

    static_cast<void>(m_manager);
    return cli_result_code::success;
}
cli_result_code credential_executor::list(argument_container_view const& arguments) const
{
    static_cast<void>(arguments);

    m_output_stream << L"list of all credentials" << endl;

    using credentials_t = std::vector<credential<wchar_t>>;

    auto credentials =  m_manager
        .get_credentials()
        .value_or(credentials_t());

    for (auto const& credential : credentials) {
        print_credential(credential, m_output_stream);
    }

    return cli_result_code::success;
}
cli_result_code credential_executor::remove(argument_container_view const& arguments) const
{
    if (arguments.size() < 2) {
        m_output_stream << L"Usage: credential_store.cli remove <type> <target>" << endl;
        return cli_result_code::insufficient_arguments;
    }
    if (auto const type = get_credential_type(arguments[0]);
        type != credential_type::unknown) {
            std::wstring const& id{begin(arguments[1]), end(arguments[1])};

        auto credential = m_manager.find(id.c_str(), type);

        if (credential.has_value<win32::credential_store::credential<wchar_t>>()) {
            static_cast<void>(m_manager.remove(credential.value<win32::credential_store::credential<wchar_t>>()));
            return cli_result_code::success;
        } else
            return cli_result_code::not_found;

    } else {
        print_unreognized_type();
        return cli_result_code::unrecognized_argument;
    }
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

    if (upper_type == "GENERIC")
        return credential_type::generic;
    if (upper_type == "DOMAIN_CERTIFICATE")
        return credential_type::domain_certificate;
    if (upper_type == "DOMAIN_EXTENDED")
        return credential_type::domain_extended;
    if (upper_type == "DOMAIN_PASSWORD")
        return credential_type::domain_password;
    if (upper_type == "DOMAIN_VISIBLE_PASSWORD")
        return credential_type::domain_visible_password;
    if (upper_type == "GENERIC_CERTIFICATE")
        return credential_type::generic_certificate;
    if (upper_type == "MAXIMUM")
        return credential_type::maximum;
    if (upper_type == "MAXIMUM_EX")
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
