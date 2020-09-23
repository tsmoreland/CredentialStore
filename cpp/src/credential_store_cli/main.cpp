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

verb_type parse_verb(std::string_view verb_string);

int main(int const argc, char* argv[])
{
    if (argc < 2)
    {
        wcout << L"Usage: credential_store.cli <verb>" << endl;
        return 1;
    }


    verb_type const verb = parse_verb(argv[1]);
    if (verb == verb_type::none) {
        wcout << L"Unrecognized verb" << endl;
        return 2;
    }

    credential_manager manager;

    return 0;
}

verb_type parse_verb(std::string_view verb_string)
{
    std::string upper_verb;

    std::transform(begin(verb_string), end(verb_string), begin(upper_verb), ::toupper);

    if (verb_string.find("ADD", 0) != std::string::npos)
        return verb_type::add;
    if (verb_string.find("FIND", 0) != std::string::npos)
        return verb_type::add;
    if (verb_string.find("LIST", 0) != std::string::npos)
        return verb_type::add;
    if (verb_string.find("REMOVE", 0) != std::string::npos)
        return verb_type::add;
    return verb_type::none;
}