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

#pragma warning(push)
#pragma warning(disable : 26495 26812)
#include <gtest/gtest.h>
#pragma warning(pop)

#include "mock_credential_factory_traits.h"
#include "mock_credential_traits.h"
#include "credential_builder.h"
#include <winerror.h>
#include "../src/credential_store_cli/credential_executor.h"
#include "../src/credential_manager_impl_using_traits.h"


// it is defined but IDE sometimes complains its not
#ifndef ERROR_NOT_FOUND
#define ERROR_NOT_FOUND 1168L
#endif

namespace win32::credential_store::tests
{
    using credential_manager_test = credential_manager_impl_using_traits<mock_credential_traits, mock_credential_factory_traits>;
    
    class credential_executor_test_fixture : public ::testing::Test
    {
    public:
        using credential_t = credential<wchar_t>;
    private:
        class mock_credential_manager final : public credential_manager_interface
        {
            credential_manager_test const m_manager{};
        public:
            [[nodiscard]] credentials_or_error_detail get_credentials() const override;
            [[nodiscard]] result_t add_or_update(credential_t const& credential) const override;
            [[nodiscard]] credential_or_error_detail find(wchar_t const* id, credential_type type) const override;
            [[nodiscard]] credentials_or_error_detail find(wchar_t const* filter, bool const search_all) const override;
            [[nodiscard]] result_t remove(credential_t const& credential) const override;
        };

        std::wstringbuf m_buffer{};
        std::wostream m_stream;
        mock_credential_manager m_manager;
        cli::credential_executor m_executor;
    public:
        void SetUp() override;
        void TearDown() override;

        explicit credential_executor_test_fixture();
    };

}
