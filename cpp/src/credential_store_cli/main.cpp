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

#include <credential_manager.h>
#include <algorithm>
#include <iostream>
#include <map>
#include <string_view>

using std::wcout;
using std::endl;
using std::begin;
using std::end;
using win32::credential_store::credential_manager;

enum class verb_type
{
    none,
    add,
    find,
    list,
    remove,
};

using verb_processor = void (*)(credential_manager const& manager, std::vector<std::string_view> const& arguments);
verb_type parse_verb(std::string_view verb_string);

void none(credential_manager const& manager, std::vector<std::string_view> const& arguments);
void add(credential_manager const& manager, std::vector<std::string_view> const& arguments);
void find(credential_manager const& manager, std::vector<std::string_view> const& arguments);
void list(credential_manager const& manager, std::vector<std::string_view> const& arguments);
void remove(credential_manager const& manager, std::vector<std::string_view> const& arguments);

int main(int const argc, char* argv[])
{
    try {
        if (argc < 2)
        {
            wcout << L"Usage: credential_store.cli <verb>" << endl;
            //return 1;
        }

        std::map<verb_type, verb_processor> verb_to_processor{
            { verb_type::none, none },
            { verb_type::add, add },
            { verb_type::find, find },
            { verb_type::list, list },
            { verb_type::remove, remove },
        };

         auto const* raw_verb = argc >= 2
             ? argv[1]
             : "list";

        std::vector<std::string_view> arguments;
        for (int i=2; i < argc; i++)
            arguments.emplace_back(argv[i]);

        verb_type const verb = parse_verb(raw_verb);
        if (verb == verb_type::none) {
            wcout << L"Unrecognized verb" << endl;
            return 2;
        }

        credential_manager const manager;
        try {
            verb_to_processor[verb](manager, arguments);

        } catch (std::exception const& ex) {
            std::cout << "Error: " << ex.what() << endl;
        }

    } catch (std::exception const& ex) {
        std::cout << "Error: " << ex.what() << endl;
    }

    return 0;
}

verb_type parse_verb(std::string_view verb_string)
{
    std::string upper_verb(verb_string.size(), '\0');

    auto to_upper = [](auto ch) {
        return static_cast<char>(::toupper(static_cast<int>(ch)));
    };

    std::transform(begin(verb_string), end(verb_string), begin(upper_verb), to_upper);

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

void none(credential_manager const& manager, std::vector<std::string_view> const& arguments)
{
    static_cast<void>(manager);
    static_cast<void>(arguments);
}
void add(credential_manager const& manager, std::vector<std::string_view> const& arguments)
{
    static_cast<void>(manager);
    static_cast<void>(arguments);
}
void find(credential_manager const& manager, std::vector<std::string_view> const& arguments)
{
    static_cast<void>(manager);
    static_cast<void>(arguments);
}
void list(credential_manager const& manager, std::vector<std::string_view> const& arguments)
{
    static_cast<void>(arguments);
    auto credentials =  manager.get_credentials();

    for (auto const& credential : credentials) {
        wcout << credential.get_id() << L" " << credential.get_username() << endl;
    }
}
void remove(credential_manager const& manager, std::vector<std::string_view> const& arguments)
{
    static_cast<void>(manager);
    static_cast<void>(arguments);
}
