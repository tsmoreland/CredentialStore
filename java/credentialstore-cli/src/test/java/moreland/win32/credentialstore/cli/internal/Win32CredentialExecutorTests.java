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
package moreland.win32.credentialstore.cli.internal;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.io.PrintStream;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import moreland.win32.credentialstore.Credential;
import moreland.win32.credentialstore.CredentialFlag;
import moreland.win32.credentialstore.CredentialManager;
import moreland.win32.credentialstore.CredentialPersistence;
import moreland.win32.credentialstore.CredentialType;

@ExtendWith(MockitoExtension.class)
class Win32CredentialExecutorTests {

    @Mock
    private CredentialManager credentialManager;

    @Mock
    private PrintStream outputStream;

    @Mock
    private PasswordReaderFacade passwordReaderFacade;

    private Win32CredentialExecutor credentialExecutor;

    @BeforeEach
    void beforeEach() {
        credentialExecutor = new Win32CredentialExecutor(credentialManager, outputStream, passwordReaderFacade);
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenCredentialManagerIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Win32CredentialExecutor((CredentialManager) null, outputStream, passwordReaderFacade));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenOutputStreamIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Win32CredentialExecutor(credentialManager, (PrintStream) null, passwordReaderFacade));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenPasswordReaderFacadeIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Win32CredentialExecutor(credentialManager, outputStream, (PasswordReaderFacade)null));
    }

    @ParameterizedTest
    @ValueSource(strings = { "Add", "add", "AdD", "ADD"})
    void getOperation_returnsIsPresent_whenNameIsAddAnyCase(String name) {
        var operation = credentialExecutor.getOperation(name);
        assertTrue(operation.isPresent());
    }

    @ParameterizedTest
    @ValueSource(strings = { "Remove", "remove", "reMOve", "REMOVE"})
    void getOperation_returnsIsPresent_whenNameIsRemoveAnyCase(String name) {
        var operation = credentialExecutor.getOperation(name);
        assertTrue(operation.isPresent());
    }

    @ParameterizedTest
    @ValueSource(strings = { "List", "list", "LisT", "LIST"})
    void getOperation_returnsIsPresent_whenNameIsListAnyCase(String name) {
        var operation = credentialExecutor.getOperation(name);
        assertTrue(operation.isPresent());
    }

    @ParameterizedTest
    @ValueSource(strings = { "Find", "find", "FIND", "FinD"})
    void getOperation_returnsIsPresent_whenNameIsFindAnyCase(String name) {
        var operation = credentialExecutor.getOperation(name);
        assertTrue(operation.isPresent());
    }

    @ParameterizedTest
    @ValueSource(strings = {"alpha", "bravo", "CHARLIE"})
    void getOperation_returnNotPresent_whenNameIsNotSupported(String name) {
        var operation = credentialExecutor.getOperation(name);
        assertFalse(operation.isPresent());
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void add_printsUsage_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        credentialExecutor.add(arguments);
        verify(outputStream, times(1))
            .println(Win32CredentialExecutor.getHelp("add").orElse("error"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void add_returnsTrue_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        assertTrue(credentialExecutor.add(arguments));
    }

    @ParameterizedTest
    @ValueSource(booleans = { true, false })
    void add_returnsExpected_whenCredentialManagerAddReturnsExpected(boolean expected) {
        when(credentialManager.add(any(Credential.class))).thenReturn(expected);

        var arguments = List.of("generic", "id1", "username1");
        var actual = credentialExecutor.add(arguments);

        assertEquals(expected, actual);
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void remove_printsUsage_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        credentialExecutor.remove(arguments);
        verify(outputStream, times(1))
            .println(Win32CredentialExecutor.getHelp("remove").orElse("error"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void remove_returnsTrue_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        assertTrue(credentialExecutor.remove(arguments));
    }

    @ParameterizedTest
    @ValueSource(booleans = { true, false })
    void remove_returnsExpected_whenDeleteUsingIdAndTypeReturnsExpected(boolean expected) {
        var arguments = List.of("id1", "generic");
        when(credentialManager.delete("id1", CredentialType.GENERIC)).thenReturn(expected);

        var actual = credentialExecutor.remove(arguments);

        assertEquals(expected, actual);
    }

    @ParameterizedTest
    @ValueSource(booleans = { true, false })
    void remove_returnsExpected_whenDeleteUsingIdReturnsExpected(boolean expected) {
        var credentials = List.of(
            new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id2", "username2", "secret2", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id3", "username3", "secret3", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now())
        );
        when(credentialManager.getAll()).thenReturn(credentials);
        when(credentialManager.delete(credentials.get(0))).thenReturn(expected);
        var arguments = List.of("id1");

        assertEquals(expected, credentialExecutor.remove(arguments));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void find_printsUsage_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        credentialExecutor.find(arguments);
        verify(outputStream, times(1))
            .println(Win32CredentialExecutor.getHelp("find").orElse("error"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void find_returnsTrue_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        assertTrue(credentialExecutor.find(arguments));
    }

    @Test
    void find_returnsFalse_whenMatchNotFoundUsingTypeAndId() {
        var arguments = List.of("test-id", "generic");
        when(credentialManager.find("test-id", CredentialType.GENERIC)).thenReturn(Optional.empty());
        assertFalse(credentialExecutor.find(arguments));
    }

    @Test
    void find_returnsFalse_whenMatchNotFoundUsingId() {
        var arguments = List.of("test-id");
        when(credentialManager.find("test-id", true)).thenReturn(List.of());
        assertFalse(credentialExecutor.find(arguments));
    }

    @Test
    void find_returnsTrue_whenMatchFoundUsingIdAndType() {
        var credential = new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now());
        var arguments = List.of("id1", "generic");
        when(credentialManager.find("id1", CredentialType.GENERIC)).thenReturn(Optional.of(credential));
        assertTrue(credentialExecutor.find(arguments));
    }

    @Test
    void find_returnsTrue_whenMatchFoundUsingId() {
        var credentials = List.of(
            new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id2", "username2", "secret2", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id3", "username3", "secret3", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now())
        );

        var arguments = List.of("id1");
        when(credentialManager.find("id1", true)).thenReturn(credentials);

        assertTrue(credentialExecutor.find(arguments));
    }

    @Test
    void find_printsExpectedResult_whenMatchFoundUsingIdAndType() {
        var credential = new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now());
        var arguments = List.of("id1", "generic");
        when(credentialManager.find("id1", CredentialType.GENERIC)).thenReturn(Optional.of(credential));

        credentialExecutor.find(arguments);

        verify(outputStream, times(1)).println(Win32CredentialExecutor.formatOutput(credential));
    }

    @Test
    void find_printsExpectedResult_whenMatchFoundUsingId() {
        var credentials = List.of(
            new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id2", "username2", "secret2", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id3", "username3", "secret3", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now())
        );
        var arguments = List.of("id1");
        when(credentialManager.find("id1", true)).thenReturn(credentials);

        credentialExecutor.find(arguments);

        verify(outputStream, times(1)).println(Win32CredentialExecutor.formatOutput(credentials.get(0)));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void list_printsUsage_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        credentialExecutor.list(arguments);
        verify(outputStream, times(1))
            .println(Win32CredentialExecutor.getHelp("list").orElse("error"));
    }

    @ParameterizedTest
    @ValueSource(strings = {"help", "Help", "HELP"})
    void list_returnsTrue_whenFirstArgumentIsHelp(String argument) {
        var arguments = List.of(argument);
        assertTrue(credentialExecutor.list(arguments));
    }

    @Test
    void list_printsCredentialsSize_whenArgumentsNotHelp() {
        var credentials = List.of(
            new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id2", "username2", "secret2", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id3", "username3", "secret3", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now())
        );
        when(credentialManager.getAll()).thenReturn(credentials);

        credentialExecutor.list(List.of());

        verify(outputStream, times(1))
            .println(String.format("Found %d credentials:", credentials.size()));


    }

    @Test
    void list_printsCredentials_whenArgumentsNotHelp() {
        var credentials = List.of(
            new Credential("id1", "username1", "secret1", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id2", "username2", "secret2", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now()),
            new Credential("id3", "username3", "secret3", CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE, LocalDateTime.now())
        );
        when(credentialManager.getAll()).thenReturn(credentials);
        var expectedOutput = credentials
            .stream()
            .map(Win32CredentialExecutor::formatOutput);

        credentialExecutor.list(List.of());

        expectedOutput
            .forEach(s -> verify(outputStream, times(1)).println(s));
    }

    @Test
    void list_returnsTrue_whenArgumentsAreEmpty() {
        when(credentialManager.getAll()).thenReturn(List.of());
        assertTrue(credentialExecutor.list(List.of()));
    }
}
