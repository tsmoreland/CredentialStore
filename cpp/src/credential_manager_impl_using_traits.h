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

#pragma once

#define WIN32_LEAN_AND_MEAN 

#include <system_error>
#include <stdexcept>
#include <optional>
#include <credential.h>
#include "credential_manager_impl.h"
#include "win32_credential_traits.h"
#include "credential_factory_traits.h"

namespace win32::credential_store
{
    template <typename WIN32_CREDENTIAL_TRAITS, typename CREDENTIAL_FACTORY_TRAITS>
    class credential_manager_impl_using_traits final : public credential_manager_impl
    {
    public:
        using credential_t = credential<wchar_t>;
        using optional_credential_t = std::optional<credential<wchar_t>>;

        void add_or_update(credential_t const& credential) override
        {
        }
        [[nodiscard]] optional_credential_t find(wchar_t const* id, credential_type type) const override
        {
            if (is_null_or_empty(id)) {
                throw std::invalid_argument("id cannot be empty");
            }

            CREDENTIALW credential;
            if (auto result = WIN32_CREDENTIAL_TRAITS::cred_read(id, type, 0, credential);
                result == SUCCESS) {
                return optional_credential_t(CREDENTIAL_FACTORY_TRAITS::from_win32_credential(credential));
            } else if (result == ERROR_NOT_FOUND) {
                return std::nullopt;
            } else {
                throw std::system_error(std::error_code(result, std::system_category()));
            }
        }
        [[nodiscard]] std::vector<credential_t> find(wchar_t const* filter, bool search_all) const override
        {
            std::vector<credential_t> matches;

            return matches;
        }
        void remove(credential_t const& credential) const override
        {
            remove(credential.get_id().c_str(), credential.get_credential_type());
        }
        void remove(wchar_t const* id, credential_type type) const override
        {
            // static_cast<std::underlying_type<credential_type>::type>(type) -- move into WIN32_CREDENTIAL_TRAITS
            if (auto const result = WIN32_CREDENTIAL_TRAITS::cred_delete(id, type, 0);
                result != SUCCESS && result != ERROR_NOT_FOUND) {
                throw std::system_error(std::error_code(result, std::system_category()));
            } 
        }

        credential_manager_impl_using_traits(const credential_manager_impl_using_traits& other) = delete;
        credential_manager_impl_using_traits(credential_manager_impl_using_traits&& other) noexcept = default;
        credential_manager_impl_using_traits& operator=(const credential_manager_impl_using_traits& other) = delete;
        credential_manager_impl_using_traits& operator=(credential_manager_impl_using_traits&& other) noexcept = default;
        ~credential_manager_impl_using_traits() override = default;
    private:
        const DWORD SUCCESS = 0UL;

        [[nodiscard]] static bool is_null_or_empty(wchar_t const *string)
        {
            return string == nullptr || wcslen(string) == 0;
        }
    };

    using win32_credential_manager_impl_using_traits = credential_manager_impl_using_traits<win32_credential_traits, credential_factory_traits>;
    
}
