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

#include "mock_credential_factory_traits.h"
#include "invalid_test_setup_exception.h"

namespace win32::credential_store::tests
{
    mock_credential_factory_traits::credential_t* mock_credential_factory_traits::m_mock_result_ptr = nullptr;

    mock_credential_factory_traits::credential_t mock_credential_factory_traits::from_win32_credential(CREDENTIALW const* const credential_ptr)
    {
        if (m_mock_result_ptr == nullptr)
            throw invalid_test_setup_exception("mock result not initialized");

        static_cast<void>(credential_ptr);
        return *m_mock_result_ptr;

    }
    void mock_credential_factory_traits::set_credential(credential_t* value)
    {
        m_mock_result_ptr = value;
    }
}
