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

#include <credential_manager_interface.h>
#include <string_view>
#include <functional>
#include <vector>

namespace win32::credential_store::cli
{
    //using verb_processor = void (*)(credential_manager const& manager, std::vector<std::string_view> const& arguments);
    using verb_processor = std::function<void (std::vector<std::string_view> const&)>;
    
    class credential_executor final
    {
    public:
        explicit credential_executor(credential_manager_interface const& manager);

        [[nodiscard]] verb_processor get_operation(std::string_view const& verb) const;

        void none(std::vector<std::string_view> const& arguments) const;
        void add(std::vector<std::string_view> const& arguments) const;
        void find(std::vector<std::string_view> const& arguments) const;
        void list(std::vector<std::string_view> const& arguments) const;
        void remove(std::vector<std::string_view> const& arguments) const;

    private:
        credential_manager_interface const& m_manager;
    };

}
