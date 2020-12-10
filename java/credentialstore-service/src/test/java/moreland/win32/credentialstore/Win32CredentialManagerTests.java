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

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.util.List;
import java.util.Optional;

import com.sun.jna.LastErrorException;

import org.slf4j.Logger;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import moreland.win32.credentialstore.converters.CredentialConverter;
import moreland.win32.credentialstore.internal.CredentialList;
import moreland.win32.credentialstore.internal.CriticalCredentialHandle;
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
    private ErrorToStringService errorToStringService;

    @Mock
    private Logger logger;

    @Mock
    private CriticalCredentialHandle credentialHandle;
    
    @Mock
    private moreland.win32.credentialstore.structures.Credential nativeCredential;

    @Mock
    private CredentialList credentialsList;

    private List<moreland.win32.credentialstore.structures.Credential> credentials;


    private Win32CredentialManager credentialManager;

    @BeforeEach
    void beforeEach() {
        credentialManager = new Win32CredentialManager(nativeInteropBridge, credentialConverter, errorToStringService, logger);

        credentials = List.of(nativeCredential);
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenNativeInteropBridgeIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager((NativeInteropBridge) null, credentialConverter, errorToStringService, logger));
        assertTrue(ex.getMessage().contains("nativeInteropBridge"));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenCredentialConverterIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager(nativeInteropBridge, (CredentialConverter) null, errorToStringService, logger));
        assertTrue(ex.getMessage().contains("credentialConverter"));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenErrorToStringServiceIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager(nativeInteropBridge, credentialConverter, errorToStringService, (Logger)null));
        assertTrue(ex.getMessage().contains("logger"));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenLoggerIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CredentialManager(nativeInteropBridge, credentialConverter, (ErrorToStringService)null, logger));
        assertTrue(ex.getMessage().contains("errorToStringService"));
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

    @Test
    void find_doesNotThrow_whenFindSucceeds() {
        arrangeUsingCredReadReturnsCred(FindResult.SUCCESS, null);
        assertDoesNotThrow(() -> credentialManager.find("id", CredentialType.GENERIC));
    }

    @Test
    void find_isPresent_whenFindSucceeds() {
        arrangeUsingCredReadReturnsCred(FindResult.SUCCESS, null);
        var actualValue = credentialManager.find("id", CredentialType.GENERIC);
        assertTrue(actualValue.isPresent());
    }

    @Test
    void find_returnsExpectedValue_whenFindSucceeds() {
        arrangeUsingCredReadReturnsCred(FindResult.SUCCESS, null);
        var actualValue = credentialManager.find("id", CredentialType.GENERIC);
        assertEquals(credential, actualValue.get());
    }

    @Test
    void find_doesNotThrow_whenCredReadThrowsLastError42() {
        arrangeUsingCredReadReturnsCred(FindResult.READ_THROWS, new LastErrorException(42));
        assertDoesNotThrow(() -> credentialManager.find("id", CredentialType.GENERIC));
    }

    @Test
    void find_isNotPresent_whenCredReadThrowsLastError42() {
        arrangeUsingCredReadReturnsCred(FindResult.READ_THROWS, new LastErrorException(42));
        var actualValue = credentialManager.find("id", CredentialType.GENERIC);
        assertFalse(actualValue.isPresent());
    }

    @Test
    void find_logsError_whenCredReadThrowsLastErrorInvalidArgument() {
        var e = new LastErrorException(ExpectedErrorCode.INVALID_ARGUMENT.getValue());
        when(errorToStringService.getMessageFor(ExpectedErrorCode.INVALID_ARGUMENT))
            .thenReturn(Optional.of("ERROR_INVALID"));
        arrangeUsingCredReadReturnsCred(FindResult.READ_THROWS, e);
        credentialManager.find("id", CredentialType.GENERIC);

        verify(logger, times(1)).error("ERROR_INVALID", e);
    }

    @ParameterizedTest
    @ValueSource(booleans = {true, false})
    void find_searchAll_returnsOneMatch_whenMatchFound(boolean searchAll) {
        arrangeFilteredCredEnumerateReturns();
        when(credential.getId()).thenReturn("test-id");

        var actualValue = credentialManager.find("test-id", searchAll);

        assertEquals(1,  actualValue.size());
    }

    @ParameterizedTest
    @ValueSource(booleans = {true, false})
    void find_searchAll_returnsExpectedValue_whenMatchFound(boolean searchAll) {
        arrangeFilteredCredEnumerateReturns();
        when(credential.getId()).thenReturn("test-id");

        var actualValue = credentialManager.find("test-id", searchAll);

        assertEquals(credential, actualValue.get(0));
    }

    @ParameterizedTest
    @ValueSource(booleans = {true, false})
    void find_searchAll_returnsEmpty_whenFindFails(boolean searchAll) {
        arrangeFilteredCredEnumerateReturns();
        when(credential.getId()).thenReturn("test-id-not-found");

        var actualValue = credentialManager.find("test-id", searchAll);

        assertEquals(0,  actualValue.size());
    }

    @ParameterizedTest
    @ValueSource(booleans = {true, false})
    void find_searchAll_returnsEmpty_whenCredEnumerateThrows(boolean searchAll) {
        arrangeFilteredCredEnumerateReturns(new LastErrorException(ExpectedErrorCode.INVALID_ARGUMENT.getValue()));
        var actualValue = credentialManager.find("test-id", searchAll);
        assertEquals(0,  actualValue.size());
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

    private static enum FindResult
    {
        SUCCESS,
        CONVERTER_FAILS,
        READ_FAILS,
        READ_THROWS,
    }

    private void arrangeUsingCredReadReturnsCred(FindResult result, LastErrorException e) {
        try {
            switch (result) {
                case SUCCESS:
                    when(nativeInteropBridge.credRead(any(String.class), any(CredentialType.class), any(Integer.class)))
                        .thenReturn(credentialHandle);
                    when(credentialHandle.value())
                        .thenReturn(Optional.of(nativeCredential));
                    when(credentialConverter.fromInternalCredential(nativeCredential))
                        .thenReturn(Optional.of(credential));
                    break;
                case CONVERTER_FAILS:
                    when(nativeInteropBridge.credRead(any(String.class), any(CredentialType.class), any(Integer.class)))
                        .thenReturn(credentialHandle);
                    when(credentialHandle.value())
                        .thenReturn(Optional.of(nativeCredential));
                    when(credentialConverter.fromInternalCredential(nativeCredential))
                        .thenReturn(Optional.empty());
                    break;
                case READ_FAILS:
                    when(nativeInteropBridge.credRead(any(String.class), any(CredentialType.class), any(Integer.class)))
                        .thenReturn(credentialHandle);
                    when(credentialHandle.value())
                        .thenReturn(Optional.empty());
                    when(credentialConverter.fromInternalCredential(nativeCredential))
                        .thenThrow(e); // to force that it's not reachable
                    break;
                case READ_THROWS:
                    when(nativeInteropBridge.credRead(any(String.class), any(CredentialType.class), any(Integer.class)))
                        .thenThrow(e);
                    break;
            }
        } catch (Exception ex) {
            assertFalse(true, ex.getLocalizedMessage());
        }
    }

    private void arrangeFilteredCredEnumerateReturns() {
        arrangeFilteredCredEnumerateReturns(null);
    }
    private void arrangeFilteredCredEnumerateReturns(LastErrorException e) {
        if (e == null) {
            when(nativeInteropBridge.credEnumerate(any(), any()))
                .thenReturn(credentialsList);
            when(credentialsList.stream())    
                .thenReturn(credentials.stream());
            when(credentialConverter.fromInternalCredential(nativeCredential))
                .thenReturn(Optional.of(credential));


        } else {
            when(nativeInteropBridge.credEnumerate(any(), any()))
                .thenThrow(e);
        }

    }

}
