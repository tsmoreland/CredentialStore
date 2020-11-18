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

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import moreland.win32.credentialstore.structures.Credential;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;

import com.sun.jna.WString;
import com.sun.jna.ptr.PointerByReference;

@ExtendWith(MockitoExtension.class)
class Win32CriticalCredentialTests {
    
    @Mock
    private Advapi32Library advapi32;

    @Test
    void isPresent_returnsFalse_WhenConstructedWithNullPointer() {
        try (var handle = new Win32CriticalCredentialHandle(advapi32, (PointerByReference) null)) {
            assertFalse(handle.isPresent());

        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        }
    }

    @Test
    void value_returnsEmpty_WhenConstructedWithNullPointer() {
        try (var handle = new Win32CriticalCredentialHandle(advapi32, (PointerByReference) null)) {
            assertFalse(handle.value().isPresent());

        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        }
    }

    @Test
    void isPresent_returnsFalse_WhenPointerByReferenceContainsNull() {
        var byRef = new PointerByReference();
        try (var handle = new Win32CriticalCredentialHandle(advapi32, byRef)) {
            assertFalse(handle.isPresent());

        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        }
    }

    @Test
    void value_returnsEmpty_WhenPointerByReferenceContainsNull() {
        var byRef = new PointerByReference();
        try (var handle = new Win32CriticalCredentialHandle(advapi32, byRef)) {
            assertFalse(handle.value().isPresent());

        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        }
    }

    @Test
    void close_callsFree_whenIsPresent() {
        var credential = new Credential();
        var ptr = credential.getPointer();
        var byRef = new PointerByReference(ptr);

        try (var handle = new Win32CriticalCredentialHandle(advapi32, byRef)) {
            // ... nothing to do here ...

        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        } 

        verify(advapi32, times(1)).CredFree(ptr);
    }

    @Test
    void value_returnsOptionalWithValue_whenIsPresent() {
        final var credential = new Credential();
        var ptr = credential.getPointer();
        var byRef = new PointerByReference(ptr);

        try (var handle = new Win32CriticalCredentialHandle(advapi32, byRef)) {
            var value = handle.value();
            assertTrue(value.isPresent());
        } catch (Exception e) {
            assertFalse(true, e.getMessage());
        } 
    }

}
