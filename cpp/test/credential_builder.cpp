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

#include "credential_builder.h"

namespace win32::credential_store::tests
{

string_type get_id()
{
    return L"id";
}
string_type get_username()
{
    return L"username";
}
string_type get_secret()
{
    return L"secret";
}
string_type to_upper(string_type const& source)
{
    string_type destintiation{source};
    for (auto& ch : destintiation) {
        ch = static_cast<wchar_t>(towupper(ch));
    }
    return destintiation;
}

credential_builder initialize_builder(std::optional<credential_t> credential)
{
    return credential.has_value()
        ? credential_builder::from(credential.value())
        : credential_builder()
            .with_id(get_id())
            .with_username(get_username())
            .with_secret(get_secret())
            .with_credential_type(get_credential_type())
            .with_persistence_type(get_persistence_type())
            .with_last_updated(get_last_updated());
}
    
credential_builder::credential_builder(credential_t credential)
    : m_credential{credential}
{
}

credential_builder credential_builder::from(credential_t const& credential)
{
    return credential_builder(credential);
}

credential_builder::optional_credential credential_builder::build_if_valid() const
{
    if (m_id.empty())
        return nullopt;
    if (m_username.empty())
        return nullopt;
    if (m_secret.empty())
        return nullopt;
    if (m_credential_type == credential_type::unknown)
        return nullopt;
    if (m_persistence_type == persistence_type::unknown)
        return nullopt;

    return build_credential<wchar_t>(m_id, m_username, m_secret, m_credential_type, m_persistence_type, m_last_updated).value<credential_t>();
}

credential_builder& credential_builder::with_id(string_type const& value)
{
    m_id = value; return *this;
}

credential_builder& credential_builder::with_username(string_type const& value)
{
    m_username = value; return *this;
}

credential_builder& credential_builder::with_secret(string_type const& value)
{
    m_secret = value; return *this;
}

credential_builder& credential_builder::with_credential_type(credential_type const value)
{
    m_credential_type = value; return *this;
}

credential_builder& credential_builder::with_persistence_type(persistence_type const value)
{
    m_persistence_type = value; return *this;
}

credential_builder& credential_builder::with_last_updated(optional_time_point const& value)
{
    m_last_updated = value; return *this;
}

}
