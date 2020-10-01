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
#include <credential.h>

namespace win32::credential_store
{

    class credential_manager_interface
    {
    public:
        using credential_t = credential<wchar_t>;
        using optional_credential_t = std::optional<credential_t>;
        using credential_or_error_detail = either<credential<wchar_t>, result_detail>;

        credential_manager_interface(credential_manager_interface const&) = delete;
        virtual ~credential_manager_interface() = default;

        /// <summary>
        /// Returns all credentials from the Users credential set
        /// </summary>
        [[nodiscard]] virtual std::vector<credential_t> get_credentials() const = 0;

        /// <summary>
        /// Adds or updates <paramref name="credential"/> to Win32
        /// credential manager
        /// </summary>
        /// <param name="credential">credential to be saved</param>
        /// <returns>result_detail with value() of result_code::success on success</returns>
        [[nodiscard]] virtual result_detail add_or_update(credential_t const& credential) const = 0;

        /// <summary>
        /// Finds a credential with the given id value and optionally credential_type
        /// </summary>
        /// <param name="id">id of the credential to be found</param>
        /// <param name="type">credential type of the credential to be found</param>
        /// <returns>optional of credential with value if found; otherwise nullopt</returns>
        /// <exception cref="std::system_error">
        /// if native api returns error
        /// </exception>
        [[nodiscard]] virtual credential_or_error_detail find(wchar_t const* id, credential_type type) const = 0;

        /// <summary>
        /// Returns all credentials matching wildcard based <paramref name="filter"/>
        /// </summary>
        /// <param name="filter">filter using wildcards</param>
        /// <param name="search_all">if true all credentials are searched</param>
        /// <returns>vector containing all credentials returned </returns>
        [[nodiscard]] virtual std::vector<credential_t> find(wchar_t const* filter, bool const search_all) const = 0;

        /// <summary>
        /// removes a credential from the user's credential set
        /// </summary>
        /// <param name="credential">credential to be removed</param>
        /// <returns>result_detail with value() of result_code::success on success</returns>
        [[nodiscard]] virtual result_detail remove(credential_t const& credential) const = 0;

        [[nodiscard]] credential_manager_interface& operator=(const credential_manager_interface& other) = delete;
    protected:
        credential_manager_interface() = default;
        credential_manager_interface(credential_manager_interface &&other) noexcept = default;
        [[nodiscard]] credential_manager_interface& operator=(credential_manager_interface&& other) noexcept = default;
    };

}
