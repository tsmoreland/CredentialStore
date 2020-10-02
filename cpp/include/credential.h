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
#include <result_t.h>
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
    private:
        string_type m_id;
        string_type m_username;
        string_type m_secret;
        credential_type m_credential_type;
        persistence_type m_persistence_type;
        optional_time_point m_last_updated;

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
        [[nodiscard]] string_type const& get_id() const
        {
            return m_id;
        }

        /// <summary>
        /// Returns the the user name of the account used to connect to <see cref="Id"/> 
        /// </summary>
        [[nodiscard]] string_type const& get_username() const
        {
            return m_username;
        }

        /// <summary>
        /// Returns the secret (password)
        /// </summary>
        [[nodiscard]] string_type const& get_secret() const
        {
            return m_secret;
        }

        /// <summary>
        /// <see cref="credential_type"/>
        /// </summary>
        [[nodiscard]] credential_type const& get_credential_type() const
        {
            return m_credential_type;
        }

        /// <summary>
        /// <see cref="persistence_type"/>
        /// </summary>
        [[nodiscard]] persistence_type const& get_persistence_type() const
        {
            return m_persistence_type;
        }

        /// <summary>
        /// Returns the last update time, this may be nullopt for credentials which have no yet been saved
        /// </summary>
        [[nodiscard]] optional_time_point const& get_last_updated() const
        {
            return m_last_updated;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type
        /// </summary>
        /// <remarks>
        /// Only compares id, username, credential_type and persistent_type
        /// </remarks>
        [[nodiscard]] friend bool equal(credential const& first, credential const& second) noexcept
        {
            using std::equal;
            return
                equal(begin(first.get_id()), end(first.get_id()), begin(second.get_id())) &&
                equal(begin(first.get_username()), end(first.get_username()), begin(second.get_username())) &&
                first.get_credential_type() == second.get_credential_type() &&
                first.get_persistence_type() == second.get_persistence_type();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type
        /// </summary>
        /// <remarks>
        /// Only compares id, username, credential_type and persistent_type
        /// </remarks>
        [[nodiscard]] bool equals(credential const& other) const noexcept
        {
            return equal(*this, other);
        }

        [[nodiscard]] deconstruct_type deconstruct() const
        {
            return std::make_tuple(m_id, m_username, m_secret, m_credential_type, m_persistence_type, m_last_updated);
        }

        friend void swap(credential& left, credential& right) noexcept
        {
            std::swap(left.m_id, right.m_id);
            std::swap(left.m_username, right.m_username);
            std::swap(left.m_secret, right.m_secret);
            std::swap(left.m_credential_type, right.m_credential_type);
            std::swap(left.m_persistence_type, right.m_persistence_type);
            std::swap(left.m_last_updated, right.m_last_updated);
        }
    private:

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

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type
    /// </summary>
    /// <remarks>
    /// Only compares id, username, credential_type and persistent_type
    /// </remarks>
    template <typename TCHAR>
    [[nodiscard]] bool operator==(credential<TCHAR> const& first, credential<TCHAR> const& second) noexcept
    {
        return equal(first, second);
    }
    /// <summary>
    /// Indicates whether the current object is not equal to another object of the same type
    /// </summary>
    /// <remarks>
    /// Only compares id, username, credential_type and persistent_type
    /// </remarks>
    template <typename TCHAR>
    [[nodiscard]] bool operator!=(credential<TCHAR> const& first, credential<TCHAR> const& second) noexcept
    {
        return !equal(first, second);
    }


    template <typename TCHAR>
    using credential_or_error = either<credential<TCHAR>, result_t>;

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
            return make_left<credential<TCHAR>, result_t>({id, username, secret, credential_type, persistence_type, last_updated});

        } catch (std::invalid_argument const& e) {
            return make_right<credential<TCHAR>, result_t>(result_t::from_error_code(e.what(), std::errc::invalid_argument));
        }
    }



}

namespace std  // NOLINT(cert-dcl58-cpp) -- haven't found an alternate way to add template specialization
{
    using namespace win32::credential_store;

    template <typename TCHAR>
    struct hash<credential<TCHAR>>
    {
        [[nodiscard]] size_t operator()(credential<TCHAR> const& key_value) const noexcept
        {
            using string_type = basic_string<TCHAR>;
            size_t hash_code = 0;
            hash_code ^= 397 * hash<string_type>()(key_value.get_id());
            hash_code ^= 397 * hash<string_type>()(key_value.get_username());
            hash_code ^= 397 * hash<DWORD>()(to_dword(key_value.get_credential_type()));
            hash_code ^= 397 * hash<DWORD>()(to_dword(key_value.get_persistence_type()));

            return hash_code;
        }
    private:
        [[nodiscard]] static DWORD to_dword(credential_type const value) 
        {
            using underlying_type = std::underlying_type<credential_type>::type;
            return static_cast<DWORD>(static_cast<underlying_type>(value));
        }
        [[nodiscard]] static DWORD to_dword(persistence_type const value) 
        {
            using underlying_type = std::underlying_type<persistence_type>::type;
            return static_cast<DWORD>(static_cast<underlying_type>(value));
        }
    };

}
