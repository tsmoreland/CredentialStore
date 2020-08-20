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
#include "credential_type.h"
#include "persistence_type.h"

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
        using optional_time_point = std::optional<std::chrono::time_point>;
        using deconstruct_type = std::tuple<string_type, string_type, string_type, credential_type, persistence_type, optional_time_point>;

        credential(string_type const& id, string_type const& username, string_type const& secret,credential_type const credential_type, persistence_type const persistence_type, optional_time_point const & last_updated)
            : m_id{id}
            , m_username{username}
            , m_secret{secret}
            , m_credential_type{credential_type}
            , m_persistence_type{persistence_type}
            , m_last_updated{last_updated}
        {
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

    };

}