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

#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <wincred.h>
#include <credential.h>
#include <credential_store_export.h>

namespace win32::credential_store
{
    struct WIN32_CREDENTIAL_STORE_EXPORT credential_factory_traits final
    {
        using credential_t = credential<wchar_t>;

        [[nodiscard]] static credential_t from_win32_credential(CREDENTIALW const * const credential_ptr);

    private:
        static credential_type to_credential_type(DWORD const type);
        static persistence_type to_persistence_type(DWORD const type);
        static std::wstring get_secret(CREDENTIALW const*const credential_ptr);
        static wchar_t const* value_or_empty(wchar_t const* const value);
    };
    
}
