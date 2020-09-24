//
// Copyright � 2020 Terry Moreland
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
#include <iostream>
#include <string_view>
#include "credential_executor.h"

using std::wcout;
using std::endl;
using win32::credential_store::credential_manager;
using win32::credential_store::cli::credential_executor;

int main(int const argc, char* argv[])
{
    try {
        if (argc < 2)
        {
            wcout << L"Usage: credential_store.cli <verb>" << endl;
            //return 1;
        }

         auto const* raw_verb = argc >= 2
             ? argv[1]
             : "list";

        std::vector<std::string_view> arguments;
        for (int i=2; i < argc; i++)
            arguments.emplace_back(argv[i]);

        credential_manager const manager;
        credential_executor const executor(manager);

        executor.get_operation(raw_verb)(arguments);

    } catch (std::exception const& ex) {
        std::cout << "Error: " << ex.what() << endl;
    }

    return 0;
}

