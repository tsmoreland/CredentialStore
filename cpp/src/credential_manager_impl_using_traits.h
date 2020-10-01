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

#define WIN32_LEAN_AND_MEAN 

#include <system_error>
#include <stdexcept>
#include <optional>
#include <credential.h>
#include "credential_manager_impl.h"
#include "credential_traits.h"
#include "credential_factory_traits.h"
#include "credential_w_facade.h"

namespace win32::credential_store
{
    using unique_credential_w = std::unique_ptr<CREDENTIALW, void (*)(CREDENTIALW*)>;  

    template <typename CREDENTIAL_TRAITS, typename CREDENTIAL_FACTORY_TRAITS>
    class credential_manager_impl_using_traits final : public credential_manager_impl
    {
    public:
        using credential_t = credential<wchar_t>;
        using optional_credential_t = std::optional<credential<wchar_t>>;

        [[nodiscard]] std::vector<credential_t> get_credentials() const override
        {
            return get_credentials(nullptr, true);
        }
        [[nodiscard]] result_detail add_or_update(credential_t const& credential) override
        {
            credential_w_facade win32_credential;
            win32_credential
                .set_target_name(credential.get_id())
                .set_username(credential.get_username())
                .set_credential_blob(credential.get_secret())
                .set_credential_type(CREDENTIAL_TRAITS::to_dword(credential.get_credential_type()))
                .set_persistence_type(CREDENTIAL_TRAITS::to_dword(credential.get_persistence_type()));
            return result_detail::from_win32_error(CREDENTIAL_TRAITS::cred_write(&win32_credential.get_credential(), CRED_PRESERVE_CREDENTIAL_BLOB));
        }
        [[nodiscard]] credential_or_error_detail find(wchar_t const* id, credential_type type) const override
        {
            if (is_null_or_empty(id)) {
                return make_right<credential_t, result_detail>(result_detail::from_error_code(std::errc::invalid_argument));
            }

            unique_credential_w credential{nullptr, CREDENTIAL_TRAITS::credential_deleter};
            if (auto result = CREDENTIAL_TRAITS::cred_read(id, type, credential);
                result == SUCCESS) {
                return make_left<credential_t, result_detail>(CREDENTIAL_FACTORY_TRAITS::from_win32_credential(credential.get()));

            } else if (result == ERROR_NOT_FOUND) {
                return make_right<credential_t, result_detail>(result_detail::from_win32_error(result));

            } else {
                return make_right<credential_t, result_detail>(result_detail::from_win32_error(result));
            }
        }
        [[nodiscard]] std::vector<credential_t> find(wchar_t const* filter, const bool search_all) const override
        {
            return get_credentials(filter, search_all);
        }
        [[nodiscard]] result_detail remove(credential_t const& credential) const override
        {
            return remove(credential.get_id().c_str(), credential.get_credential_type());
        }
        [[nodiscard]] result_detail remove(wchar_t const* id, credential_type type) const override
        {
            // static_cast<std::underlying_type<credential_type>::type>(type) -- move into CREDENTIAL_TRAITS
            if (auto const result = CREDENTIAL_TRAITS::cred_delete(id, type);
                result != SUCCESS && result != ERROR_NOT_FOUND) {
                return result_detail::from_win32_error(result);
            } else {
                return result_detail::success();
            }
        }

        explicit credential_manager_impl_using_traits() = default;
        credential_manager_impl_using_traits(const credential_manager_impl_using_traits& other) = delete;
        credential_manager_impl_using_traits(credential_manager_impl_using_traits&& other) noexcept = default;
        credential_manager_impl_using_traits& operator=(const credential_manager_impl_using_traits& other) = delete;
        credential_manager_impl_using_traits& operator=(credential_manager_impl_using_traits&& other) noexcept = default;
        ~credential_manager_impl_using_traits() override = default;
    private:
        static const DWORD SUCCESS = 0UL;

        [[nodiscard]] static bool is_null_or_empty(wchar_t const *string)
        {
            return string == nullptr || wcslen(string) == 0;
        }

        [[nodiscard]] std::vector<credential_t> get_credentials(wchar_t const* filter, const bool search_all) const 
        {
            std::vector<credential_t> matches;

            auto flags = search_all ? CRED_ENUMERATE_ALL_CREDENTIALS : 0;

            struct credentials_container
            {
                credentials_container() = default;
                credentials_container(credentials_container const&) = delete;
                credentials_container(credentials_container &&) noexcept = delete;
                credentials_container& operator=(credentials_container const&) = delete;
                credentials_container& operator=(credentials_container &&) noexcept = delete;
                ~credentials_container()
                {
                    if (credentials != nullptr) {
                        CREDENTIAL_TRAITS::credential_deleter(credentials);
                    }
                }
                CREDENTIALW** credentials{nullptr};
                DWORD count{0};
            };
            credentials_container container;

            auto const result = CREDENTIAL_TRAITS::cred_enumerate(filter, flags, container.count, container.credentials); 
            if (result != SUCCESS) {
                throw std::system_error(std::error_code(result, std::system_category()));
            }

            for (DWORD i = 0; i< container.count; i++) {
                matches.push_back(CREDENTIAL_FACTORY_TRAITS::from_win32_credential(container.credentials[i]));
            }

            return matches;
        }

    };

    using win32_credential_manager_impl_using_traits = credential_manager_impl_using_traits<credential_traits, credential_factory_traits>;
    
}
