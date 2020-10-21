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
#include <iostream>
#include <algorithm>
#include <string_view>
#include "credential_executor.h"
#include "obscurred_reader.h"

using std::wcout;
using std::endl;
using win32::credential_store::credential_manager;
using win32::credential_store::cli::credential_executor;

int main(int const argc, char const* argv[])
{

    try {
        using win32::credential_store::cli::cli_result_code;
        using win32::credential_store::cli::read_secret;

        if (argc < 2)
        {
            wcout << L"Usage: credential_store.cli <verb>" << endl;
            return static_cast<int>(cli_result_code::insufficient_arguments);
        }

         auto const* raw_verb = argc >= 2
             ? argv[1]
             : "add";

        std::vector<std::string_view> arguments;
        int i = 2;
        std::generate_n(std::back_inserter(arguments), argc - 2,
            [argv, &i]() -> std::string_view
            {
                return argv[i++];
            });

        auto const* arg0 = "generic";
        auto const* arg1 = "sample-target";
        auto const* arg2 = "sample-username";

        arguments.clear();
        arguments.emplace_back(arg0);
        arguments.emplace_back(arg1);
        arguments.emplace_back(arg2);

        std::function<std::wstring()> const read_user_secret(
            []() {
                return read_secret(L"enter password:");
            }); 

        credential_manager const manager;
        credential_executor const executor(manager, std::wcout);

        auto const result = executor.get_operation(raw_verb)(arguments);

        switch (result) {
        case cli_result_code::not_found:
        case cli_result_code::insufficient_arguments:
        case cli_result_code::unrecognized_argument:
            return static_cast<int>(result);
        case cli_result_code::success:
            return 0;
        case cli_result_code::error_occurred:
            std::cout << "Error occurred processing request" << endl;
            return -1;
        }

    } catch (std::exception const& ex) {
        std::cout << "Error: " << ex.what() << endl;
        return -1;
    }

}

