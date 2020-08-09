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

namespace Moreland.Security.Win32.CredentialStore.NativeApi
{
    internal sealed class PointerMath : IPointerMath
    {
        private readonly IMarshalService _marshalService;
        private readonly ILoggerAdapter _logger;

        public PointerMath(IMarshalService marshalService, ILoggerAdapter logger)
        {
            _marshalService = marshalService ?? throw new ArgumentNullException(nameof(marshalService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T? GetStructureAtOffsetOrNull<T>(IntPtr arrayPointer, int offset) where T : class
        {
            if (arrayPointer == IntPtr.Zero)
                throw new ArgumentException("invalid pointer", nameof(arrayPointer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var pointerToOffset = IntPtr.Add(arrayPointer, offset);
            var pointerToValue = _marshalService.ReadIntPtr(pointerToOffset);
            if (pointerToValue != IntPtr.Zero)
                return _marshalService.PtrToStructure<T>(pointerToValue);

            _logger.Error($"(null) found at offset {offset}");
            return null;
        }
    }
}
