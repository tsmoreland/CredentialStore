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
package moreland.win32.credentialstore.internal;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import com.sun.jna.Pointer;

@ExtendWith(MockitoExtension.class)
class CredentialsListTests {

    @Mock
    private Advapi32Library advapi32;

    @Mock
    private Pointer pointer;

    @Test
    void ctor_throwsIllegalArgumentException_whenAdvapi32IsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new CredentialList(Pointer.NULL, 0, (Advapi32Library) null));
        assertTrue(ex.getMessage().contains("advapi32"));
    }
    
    @Test
    void isEmpty_returnsTrue_whenPointerIsNull() throws Exception {
        try (var list = new CredentialList(Pointer.NULL, 0, advapi32)) {
            assertTrue(list.isEmpty());
        }
    }

    @Test
    void isEmpty_returnsTrue_whenCountIsZero() throws Exception {
        try (var list = new CredentialList(pointer, 0, advapi32)) {
            assertTrue(list.isEmpty());
        }
    }

    @Test
    void isEmpty_returnsTrue_whenCountIsLessThanZero() throws Exception {
        try (var list = new CredentialList(pointer, -1, advapi32)) {
            assertTrue(list.isEmpty());
        }
    }

    @Test
    void isEmpty_returnsFalse_whenCountGreaterThanZero() {
        when(pointer.getPointerArray(0, 1)).thenReturn(new Pointer[] { null });
        try (var list = new CredentialList(pointer, 1, advapi32)) {
            assertEquals(1, list.size());;
        } catch (Exception e) {
            System.out.println(e.getMessage());

        }
    }


    @Test
    void close_classCredFree_whenPointerIsNotNull() throws Exception {
        try (var list = new CredentialList(pointer, 0, advapi32)) {
            // ... nothing to do ...
        }

        verify(advapi32, times(1)).CredFree(pointer);
    }
}
