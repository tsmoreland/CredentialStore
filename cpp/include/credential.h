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

#include <string>
#include <chrono>
#include <tuple>
#include <optional>
#include <utility>
#include <stdexcept>
#include <Windows.h>
#include <wincred.h>

#include <either.h>
#include <result_detail.h>
#include <credential_type.h>
#include <persistence_type.h>

namespace win32::credential_store
{
    /// <summary>
    /// Credential Entity as populated by Win32 api
    /// </summary>
    template <typename TCHAR>
    class credential final
    {
    public:

        using string_type = std::basic_string<TCHAR>;
        using optional_time_point = std::optional<std::chrono::time_point<std::chrono::system_clock>>;
        using deconstruct_type = std::tuple<string_type, string_type, string_type, credential_type, persistence_type, optional_time_point>;

        /// <summary>
        /// initializes a new instance with provided values
        /// </summary>
        /// <exception cref="std::invalid_argument">
        /// when id is empty, credential_type is unknown or persistent_type is unknown
        /// </exception>
        credential(string_type id, string_type username, string_type secret,credential_type const credential_type, persistence_type const persistence_type, optional_time_point const & last_updated)
            : m_id{std::move(id)}
            , m_username{std::move(username)}
            , m_secret{std::move(secret)}
            , m_credential_type{credential_type}
            , m_persistence_type{persistence_type}
            , m_last_updated{last_updated}
        {
            if (char const *invalid_argument_name = get_invalid_argument_name_or_nullptr();
                invalid_argument_name != nullptr) {
                throw std::invalid_argument(invalid_argument_name);
            }
        }

        /// <summary>
        /// Returns the unqiue Identifier, representing the target name of the credential
        /// </summary>
        [[nodiscard]] string_type get_id() const
        {
            return m_id;
        }

        /// <summary>
        /// Returns the the user name of the account used to connect to <see cref="Id"/> 
        /// </summary>
        [[nodiscard]] string_type get_username() const
        {
            return m_username;
        }

        /// <summary>
        /// Returns the secret (password)
        /// </summary>
        [[nodiscard]] string_type get_secret() const
        {
            return m_secret;
        }

        /// <summary>
        /// <see cref="credential_type"/>
        /// </summary>
        [[nodiscard]] credential_type get_credential_type() const
        {
            return m_credential_type;
        }

        /// <summary>
        /// <see cref="persistence_type"/>
        /// </summary>
        [[nodiscard]] persistence_type get_persistence_type() const
        {
            return m_persistence_type;
        }

        /// <summary>
        /// Returns the last update time, this may be nullopt for credentials which have no yet been saved
        /// </summary>
        [[nodiscard]] optional_time_point get_last_updated() const
        {
            return m_last_updated;
        }
    private:
        string_type m_id;
        string_type m_username;
        string_type m_secret;
        credential_type m_credential_type;
        persistence_type m_persistence_type;
        optional_time_point m_last_updated;

        [[nodiscard]] char const* get_invalid_argument_name_or_nullptr() const noexcept
        {
            if (get_id().empty())
                return "id";
            if (get_credential_type() == credential_type::unknown)
                return "credential_type";
            if (get_persistence_type() == persistence_type::unknown)
                return "persistence_type";
            return nullptr;
        }
        
    };

    template <typename TCHAR>
    using credential_or_error = either<credential<TCHAR>, result_detail>;

    [[nodiscard]] constexpr credential_type to_credential_type(DWORD const type)
    {
        switch (type) {
        case CRED_TYPE_DOMAIN_PASSWORD:
            return credential_type::domain_password;
        case CRED_TYPE_GENERIC:
            return credential_type::generic;
        case CRED_TYPE_DOMAIN_CERTIFICATE:
            return credential_type::domain_certificate;
        case CRED_TYPE_DOMAIN_EXTENDED:
            return credential_type::domain_extended;
        case CRED_TYPE_DOMAIN_VISIBLE_PASSWORD:
            return credential_type::domain_visible_password;
        case CRED_TYPE_GENERIC_CERTIFICATE:
            return credential_type::generic_certificate;
        case CRED_TYPE_MAXIMUM:
            return credential_type::maximum;
        case CRED_TYPE_MAXIMUM_EX:
            return credential_type::maximum_ex;
        default:
            return credential_type::unknown;
        }
    }
    [[nodiscard]] constexpr persistence_type to_persistence_type(DWORD const type)
    {
        switch (type) {
        case CRED_PERSIST_ENTERPRISE:
            return persistence_type::enterprise;
        case CRED_PERSIST_LOCAL_MACHINE:
            return persistence_type::local_machine;
        case CRED_PERSIST_SESSION:
            return persistence_type::session;
        default:
            return persistence_type::unknown;
        }
    }

    [[nodiscard]] std::wstring get_secret(CREDENTIALW const*const credential_ptr);
    [[nodiscard]] inline wchar_t const* value_or_empty(wchar_t const* const value);

    template<typename TCHAR>
    [[nodiscard]] static credential_or_error<TCHAR> build_credential(
        typename credential<TCHAR>::string_type const& id, 
        typename credential<TCHAR>::string_type const& username, 
        typename credential<TCHAR>::string_type const& secret,
        credential_type const credential_type, 
        persistence_type const persistence_type, 
        typename credential<TCHAR>::optional_time_point const & last_updated)
    {
        try {
            return make_left<credential<TCHAR>, result_detail>({id, username, secret, credential_type, persistence_type, last_updated});

        } catch (std::invalid_argument const& e) {
            return make_right<credential<TCHAR>, result_detail>(result_detail(e.what(), result_code::invalid_argument));
        }
    }

    [[nodiscard]] static either<credential<wchar_t>, result_detail> from_win32_credential(CREDENTIALW const * const credential_ptr);

}