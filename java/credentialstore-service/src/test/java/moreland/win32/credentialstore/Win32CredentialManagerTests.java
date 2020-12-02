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
package moreland.win32.credentialstore;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import java.util.Optional;

import com.sun.jna.LastErrorException;

import org.slf4j.Logger;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import moreland.win32.credentialstore.converters.CredentialConverter;
import moreland.win32.credentialstore.internal.NativeInteropBridge;
import moreland.win32.credentialstore.internal.PreserveType;
import moreland.win32.credentialstore.structures.Credential.ByReference;

@ExtendWith(MockitoExtension.class)
class Win32CredentialManagerTests {

    @FunctionalInterface
    private interface ConsumerPredicate {
        boolean process(Credential credential);
    }

    @Mock
    private NativeInteropBridge nativeInteropBridge;

    @Mock
    private CredentialConverter credentialConverter;

    @Mock
    private Credential credential;

    @Mock
    private Logger logger;

    private Win32CredentialManager credentialManager;

    @BeforeEach
    void beforeEach() {
        credentialManager = new Win32CredentialManager(nativeInteropBridge, credentialConverter, logger);
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenNativeInteropBridgeIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager((NativeInteropBridge) null, credentialConverter, logger));
        assertTrue(ex.getMessage().contains("nativeInteropBridge"));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenCredentialConverterIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager(nativeInteropBridge, (CredentialConverter) null, logger));
        assertTrue(ex.getMessage().contains("credentialConverter"));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenLoggerIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager(nativeInteropBridge, credentialConverter, (Logger)null));
        assertTrue(ex.getMessage().contains("logger"));
    }

    @Test
    void add_returnsFalse_whenCredentialConverterReturnsEmpty() {
        assertFalse(arrangeAndActUsingCredentialConverterReturnsEmpty(credentialManager::add));
    }

    @Test
    void add_returnsFalse_whenCredWriteReturnsFalse() {
        assertFalse(arrangeAndActUsingCredWriteReturnsFalse(credentialManager::add));
    }

    @Test
    void add_returnsFalse_whenCredWriteThrowsLastErrorException() {
        assertFalse(arrangeAndActUsingCredWriteThrowsLastErrorException(credentialManager::add));
    }

    @Test
    void add_returnsTrue_whenCredWriteReturnsTrue() {
        assertTrue(arrangeAndActUsingCredWriteReturnsTrue(credentialManager::add));
    }

    @Test
    void update_returnsFalse_whenCredentialConverterReturnsEmpty() {
        assertFalse(arrangeAndActUsingCredentialConverterReturnsEmpty(credentialManager::update));
    }

    @Test
    void update_returnsFalse_whenCredWriteReturnsFalse() {
        assertFalse(arrangeAndActUsingCredWriteReturnsFalse(credentialManager::update));
    }

    @Test
    void update_returnsFalse_whenCredWriteThrowsLastErrorException() {
        assertFalse(arrangeAndActUsingCredWriteThrowsLastErrorException(credentialManager::update));
    }

    @Test
    void update_returnsTrue_whenCredWriteReturnsTrue() {
        assertTrue(arrangeAndActUsingCredWriteReturnsTrue(credentialManager::update));
    }

    private boolean arrangeAndActUsingCredentialConverterReturnsEmpty(ConsumerPredicate consumerPredicate) {
        when(credentialConverter.toInternalCredentialReference(any(Credential.class))).thenReturn(Optional.empty());
        return consumerPredicate.process(credential);
    }

    private boolean arrangeAndActUsingCredWriteReturnsFalse(ConsumerPredicate consumerPredicate) {
        when(credentialConverter.toInternalCredentialReference(any(Credential.class)))
            .thenReturn(Optional.of(new moreland.win32.credentialstore.structures.Credential.ByReference()));
        when(nativeInteropBridge.credWrite(any(ByReference.class), any(PreserveType.class))).thenReturn(false);
        return consumerPredicate.process(credential);
    }
    private boolean arrangeAndActUsingCredWriteThrowsLastErrorException(ConsumerPredicate consumerPredicate) {
        when(credentialConverter.toInternalCredentialReference(any(Credential.class)))
            .thenReturn(Optional.of(new moreland.win32.credentialstore.structures.Credential.ByReference()));
        when(nativeInteropBridge.credWrite(any(ByReference.class), any(PreserveType.class)))
            .thenThrow(new LastErrorException(42));
        return consumerPredicate.process(credential);
    }
    private boolean arrangeAndActUsingCredWriteReturnsTrue(ConsumerPredicate consumerPredicate) {
        when(credentialConverter.toInternalCredentialReference(any(Credential.class)))
            .thenReturn(Optional.of(new moreland.win32.credentialstore.structures.Credential.ByReference()));
        when(nativeInteropBridge.credWrite(any(ByReference.class), any(PreserveType.class))).thenReturn(true);
        return consumerPredicate.process(credential);
    }
}
