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

#include <optional>
#include <credential.h>
#include <credential_manager_interface.h>

namespace win32::credential_store
{
    class credential_manager_impl 
    {
    public:
        using credential_t = credential_manager_interface::credential_t; //credential<wchar_t>;
        using optional_credential_t = credential_manager_interface::optional_credential_t; // std::optional<credential_t>;
        using credential_or_error_detail = credential_manager_interface::credential_or_error_detail; // either<credential_t, result_t>;
        using credentials_or_error_detail = credential_manager_interface::credentials_or_error_detail; // either<std::vector<credential_t>, result_t>;

        credential_manager_impl(const credential_manager_impl& other) = delete;
        credential_manager_impl(credential_manager_impl&& other) noexcept = delete;
        virtual ~credential_manager_impl() = default;

        [[nodiscard]] virtual credentials_or_error_detail get_credentials() const = 0;
        [[nodiscard]] virtual result_t add_or_update(credential_t const& credential) = 0;
        [[nodiscard]] virtual credential_or_error_detail find(wchar_t const* id, credential_type type) const = 0;
        [[nodiscard]] virtual credentials_or_error_detail find(wchar_t const* filter, bool search_all) const = 0;
        [[nodiscard]] virtual result_t remove(credential_t const& credential) const = 0;
        [[nodiscard]] virtual result_t remove(wchar_t const* id, credential_type type) const = 0;

        credential_manager_impl& operator=(const credential_manager_impl& other) = delete;
        credential_manager_impl& operator=(credential_manager_impl&& other) noexcept = default;

    protected:
        explicit credential_manager_impl() = default;
    };
    
}
