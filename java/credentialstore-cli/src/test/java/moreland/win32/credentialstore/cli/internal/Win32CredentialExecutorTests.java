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

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;

import java.io.PrintStream;
import java.util.List;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import moreland.win32.credentialstore.CredentialManager;

@ExtendWith(MockitoExtension.class)
class Win32CredentialExecutorTests {

    @Mock
    private CredentialManager credentialManager;

    @Mock
    private PrintStream outputStream;

    private Win32CredentialExecutor credentialExecutor;

    @BeforeEach
    void beforeEach() {
        credentialExecutor = new Win32CredentialExecutor(credentialManager, outputStream);
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenCredentialManagerIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Win32CredentialExecutor((CredentialManager) null, outputStream));
    }

    @Test
    void ctor_throwsIllegalArgumentException_whenOutputStreamIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Win32CredentialExecutor(credentialManager, (PrintStream) null));
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

}
