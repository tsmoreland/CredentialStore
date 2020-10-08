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
#include <string>
#include <vector>
#include <credential.h>

namespace win32::credential_store::tests
{
    using std::nullopt;
    using credential_t = credential<wchar_t>;
    using string_type = std::wstring;
    using optional_string = std::optional<string_type>;
    using optional_credential_type = std::optional<credential_type>;
    using optional_persistence_type = std::optional<persistence_type>;
    using optional_time_point = credential_t::optional_time_point;

    [[nodiscard]] string_type get_id();
    [[nodiscard]] string_type get_username();
    [[nodiscard]] string_type get_secret();
    constexpr auto get_credential_type() { return credential_type::generic; }
    constexpr auto get_persistence_type() { return persistence_type::local_machine; }
    constexpr auto get_last_updated() { return std::nullopt; }

    [[nodiscard]] string_type to_upper(string_type const& source);

    class credential_builder final
    {
    public:
        using credential_t = credential<wchar_t>;
        using string_type = credential_t::string_type;
        using optional_time_point = credential_t::optional_time_point;
        using deconstruct_type = credential_t::deconstruct_type;
        using optional_credential = std::optional<credential_t>;
    private:
        string_type m_id;
        string_type m_username;
        string_type m_secret;
        credential_type m_credential_type{credential_type::generic};
        persistence_type m_persistence_type{persistence_type::local_machine};
        optional_time_point m_last_updated{};
        optional_credential m_credential{};
    public:
        explicit credential_builder() = default;
        explicit credential_builder(credential_t credential); 
        [[nodiscard]] static credential_builder from(credential_t const& credential);

        template <typename TArg>
        [[nodiscard]] constexpr credential_builder& set_value_and_return(TArg& destination, TArg& source)
        {
            destination = source;
            return *this;
        }
        [[nodiscard]] optional_credential build_if_valid() const;

        credential_builder& with_id(string_type const& value);
        credential_builder& with_username(string_type const& value);
        credential_builder& with_secret(string_type const& value);
        credential_builder& with_credential_type(credential_type const value);
        credential_builder& with_persistence_type(persistence_type const value);
        credential_builder& with_last_updated(optional_time_point const& value);
    };

    [[nodiscard]] credential_builder initialize_builder(std::optional<credential_t> credential = nullopt);
    [[nodiscard]] credential_t make_credential();
    [[nodiscard]] std::vector<credential_t> make_credentials(size_t const size);
}