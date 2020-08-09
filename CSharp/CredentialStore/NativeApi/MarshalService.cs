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
#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.InteropServices;

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    /// <summary>
    /// <inheritdoc cref="IMarshalService"/>
    /// </summary>
    internal sealed class MarshalService : IMarshalService
    {
        /// <summary>
        /// <inheritdoc cref="IMarshalService.PtrToStructure(IntPtr, Type)"/>
        /// </summary>
        public object? PtrToStructure(IntPtr pointer, Type type) => pointer != IntPtr.Zero
            ? Marshal.PtrToStructure(pointer, type)
            : null;

        /// <summary>
        /// <inheritdoc cref="IMarshalService.PtrToStructure{T}(IntPtr)"/>
        /// </summary>
        public T? PtrToStructure<T>(IntPtr pointer) where T : class =>
            pointer != IntPtr.Zero 
                ? Marshal.PtrToStructure<T>(pointer) 
                : null;
        /// <summary>
        /// <inheritdoc cref="IMarshalService.StructureToPtr{T}(T, IntPtr, bool)"/>
        /// </summary>
#if NETSTANDARD2_0
        public void StructureToPtr<T>(T structure, IntPtr pointer, bool deleteOld)
#else
        public void StructureToPtr<T>([DisallowNull] T structure, IntPtr pointer, bool deleteOld)
#endif
        {
            Marshal.StructureToPtr(structure, pointer, deleteOld);
        }


        /// <summary>
        /// <inheritdoc cref="IMarshalService.PtrToStringUni(IntPtr)"/>
        /// </summary>
        public string? PtrToStringUni(IntPtr pointer) =>
            Marshal.PtrToStringUni(pointer);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.GetLastWin32Error()"/>
        /// </summary>
        public int GetLastWin32Error() => Marshal.GetLastWin32Error();

        /// <summary>
        /// <inheritdoc cref="Marshal.AllocHGlobal(int)"/>
        /// </summary>
        public IntPtr AllocHGlobal(int size) =>
            Marshal.AllocHGlobal(size);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.FreeHGlobal(IntPtr)"/>
        /// </summary>
        public void FreeHGlobal(IntPtr pointer) =>
            Marshal.FreeHGlobal(pointer);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.ReadIntPtr(IntPtr)"/>
        /// </summary>
        public IntPtr ReadIntPtr(IntPtr pointer) =>
            Marshal.ReadIntPtr(pointer);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.StringToCoTaskMemUni(string)"/>
        /// </summary>
        public IntPtr StringToCoTaskMemUni(string value) =>
            Marshal.StringToCoTaskMemUni(value);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.SizeOf(Type)"/>
        /// </summary>
        public int SizeOf(Type type) =>
            Marshal.SizeOf(type);

        /// <summary>
        /// <inheritdoc cref="IMarshalService.SizeOf{T}(T)"/>
        /// </summary>
        public int SizeOf<T>(T value) =>
            Marshal.SizeOf(value);
    }

}
