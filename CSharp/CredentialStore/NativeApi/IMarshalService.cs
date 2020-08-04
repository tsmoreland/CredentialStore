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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// Wrapper around <see cref="Marshal"/>
    /// </summary>
    public interface IMarshalService
    {
        /// <summary>
        /// <inheritdoc cref="Marshal.PtrToStructure(IntPtr, Type)"/>
        /// </summary>
        object? PtrToStructure(IntPtr pointer, Type type);
        /// <summary>
        /// <inheritdoc cref="Marshal.PtrToStructure{T}(IntPtr)"/>
        /// </summary>
        T? PtrToStructure<T>(IntPtr pointer) where T : class;
        /// <summary>
        /// <inheritdoc cref="Marshal.StructureToPtr{T}(T, IntPtr, bool)"/>
        /// </summary>
#if NETSTANDARD2_0
        void StructureToPtr<T>(T structure, IntPtr pointer, bool deleteOld);
#else
        void StructureToPtr<T>([DisallowNull] T structure, IntPtr pointer, bool deleteOld);
#endif

        /// <summary>
        /// <inheritdoc cref="Marshal.PtrToStringUni(IntPtr)"/>
        /// </summary>
        string? PtrToStringUni(IntPtr pointer);
        /// <summary>
        /// <inheritdoc cref="Marshal.GetLastWin32Error()"/>
        /// </summary>
        int GetLastWin32Error();
        /// <summary>
        /// <inheritdoc cref="Marshal.AllocHGlobal(IntPtr)"/>
        /// </summary>
        IntPtr AllocHGlobal(IntPtr pointer);
        /// <summary>
        /// <inheritdoc cref="Marshal.FreeHGlobal(IntPtr)"/>
        /// </summary>
        void FreeHGlobal(IntPtr pointer);

        /// <summary>
        /// <inheritdoc cref="Marshal.ReadIntPtr(IntPtr)"/>
        /// </summary>
        IntPtr ReadIntPtr(IntPtr pointer);
        /// <summary>
        /// <inheritdoc cref="Marshal.StringToCoTaskMemUni(string)"/>
        /// </summary>
        IntPtr StringToCoTaskMemUni(string value);

    }
}
