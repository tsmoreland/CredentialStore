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

#include <memory>
#include <credential.h>
#include <credential_type.h>
#include <result_t.h>
#include <credential_manager_interface.h>
#include <credential_store_export.h>

namespace win32::credential_store
{
    class credential_manager_impl;

    /// <summary>
    /// Win32 Credential Manager (Credential Repository) providing CRUD 
    /// operations for Windows Credential Manager 
    /// </summary>
    class WIN32_CREDENTIAL_STORE_EXPORT credential_manager final : public credential_manager_interface
    {
#       pragma warning(push)
#       pragma warning(disable : 4251)
        std::unique_ptr<credential_manager_impl> m_p_impl;
#       pragma warning(pop)


    public:
        /// <summary>
        /// Returns all credentials from the Users credential set
        /// </summary>
        [[nodiscard]] credentials_or_error_detail get_credentials() const override;

        /// <summary>
        /// Adds or updates <paramref name="credential"/> to Win32
        /// credential manager
        /// </summary>
        /// <param name="credential">credential to be saved</param>
        /// <returns>result_t with error() of result_code::success on success</returns>
        [[nodiscard]] result_t add_or_update(credential_t const& credential) const override;

        /// <summary>
        /// Finds a credential with the given id error and optionally credential_type
        /// </summary>
        /// <param name="id">id of the credential to be found</param>
        /// <param name="type">credential type of the credential to be found</param>
        /// <returns>optional of credential with error if found; otherwise nullopt</returns>
        /// <exception cref="std::system_error">
        /// if native api returns error
        /// </exception>
        [[nodiscard]]  credential_or_error_detail find(wchar_t const* id, credential_type type) const override;

        /// <summary>
        /// Returns all credentials matching wildcard based <paramref name="filter"/>
        /// </summary>
        /// <param name="filter">filter using wildcards</param>
        /// <param name="search_all">if true all credentials are searched</param>
        /// <returns>vector containing all credentials returned </returns>
        [[nodiscard]] credentials_or_error_detail find(wchar_t const* filter, bool const search_all) const override;

        /// <summary>
        /// removes a credential from the user's credential set
        /// </summary>
        /// <param name="credential">credential to be removed</param>
        /// <returns>result_t with error() of result_code::success on success</returns>
        [[nodiscard]] result_t remove(credential_t const& credential) const override;

        explicit credential_manager();
        credential_manager(credential_manager const&) = delete;
        credential_manager(credential_manager &&other) noexcept;
        ~credential_manager() override;

        [[nodiscard]] credential_manager& operator=(const credential_manager& other) = delete;
        [[nodiscard]] credential_manager& operator=(credential_manager&& other) noexcept;

        /// <summary>
        /// Swaps the values first and second
        /// </summary>
        /// <param name="first">error to be swapped</param>
        /// <param name="second">error to be swapped</param>
        friend void swap(credential_manager& first, credential_manager& second) noexcept;
    };



}
