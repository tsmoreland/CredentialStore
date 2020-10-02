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
#include "credential_manager_impl_using_traits.h"

namespace win32::credential_store
{

credential_manager::credential_manager()
    : m_p_impl(std::make_unique<win32_credential_manager_impl_using_traits>())
{
}
credential_manager::credential_manager(credential_manager &&other) noexcept
    : m_p_impl(other.m_p_impl.release())
{
}
credential_manager::~credential_manager()
{
    m_p_impl.reset();
}

std::vector<credential_manager::credential_t> credential_manager::get_credentials() const
{
    if (!static_cast<bool>(m_p_impl)) {
    }

    return m_p_impl->get_credentials();
}

result_t credential_manager::add_or_update(credential_t const& credential) const
{
    if (!static_cast<bool>(m_p_impl)) {
    }

    return m_p_impl->add_or_update(credential);
}

credential_manager::credential_or_error_detail credential_manager::find(wchar_t const* id, credential_type type) const
{
    if (!static_cast<bool>(m_p_impl)) {
        return make_right<credential_t, result_t>(result_t::from_error_code(std::errc::owner_dead));
    }
    return m_p_impl->find(id, type);
}

std::vector<credential_manager::credential_t> credential_manager::find(wchar_t const* filter, bool const search_all) const
{
    if (!static_cast<bool>(m_p_impl)) {
        return std::vector<credential_t>();
    }
    return m_p_impl->find(filter, search_all);
}

result_t credential_manager::remove(credential_t const& credential) const
{
    if (!static_cast<bool>(m_p_impl)) {
        return result_t::from_error_code(std::errc::owner_dead);
    }
    return m_p_impl->remove(credential);
}

credential_manager& credential_manager::operator=(credential_manager&& other) noexcept
{
    if (&other == this) {
        return *this;
    }

    m_p_impl = std::move(other.m_p_impl);

    return *this;
}

void swap(credential_manager& first, credential_manager& second) noexcept
{
    swap(first.m_p_impl, second.m_p_impl);
}

}