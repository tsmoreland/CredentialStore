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

#include <vector>
#include <memory>
#include <optional>
#include <credential.h>
#include <credential_type.h>

namespace win32::credential_store
{
    class credential_manager_impl;

    /// <summary>
    /// Win32 Credential Manager (Credential Repository) providing CRUD 
    /// operations for Windows Credential Manager 
    /// </summary>
    class credential_manager final
    {
    public:
        using credential_t = credential<wchar_t>;
        using optional_credential_t = std::optional<credential_t>;

        /// <summary>
        /// 
        /// </summary>
        explicit credential_manager();
        credential_manager(credential_manager const&) = delete;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        credential_manager(credential_manager &&other) noexcept;
        /// <summary>
        /// 
        /// </summary>
        ~credential_manager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></params>
        void add_or_update(credential_t const& credential) const;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [[nodiscard]] optional_credential_t find(wchar_t const* id, credential_type type) const;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="search_all"></param>
        /// <returns></returns>
        [[nodiscard]] std::vector<credential_t> find(wchar_t const* filter, bool const search_all) const;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        void remove(credential_t const& credential) const;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [[nodiscard]] credential_manager& operator=(const credential_manager& other) = delete;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [[nodiscard]] credential_manager& operator=(credential_manager&& other) noexcept;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        friend void swap(credential_manager& first, credential_manager& second) noexcept;
    private:
        std::unique_ptr<credential_manager_impl> m_p_impl;
    };



}
