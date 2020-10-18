/*
 * Copyright © 2020 Terry Moreland
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

package moreland.win32.credentialstore;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.MethodSource;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.time.temporal.TemporalAmount;
import java.util.stream.Stream;

import static org.junit.jupiter.api.Assertions.*;

class CredentialTests {

    String id;
    String username;
    String secret;
    CredentialFlag flag;
    CredentialType type;
    CredentialPersistence persistence;
    LocalDateTime lastUpdated;

    /**
     * Execute before each test, no particular need for this mostly here for the sake of being here
     */
    @BeforeEach
    void beforeEach() {
        id = "target-identifier";
        username = "test-user";
        secret = "super-secret-text";
        type = CredentialType.GENERIC;
        flag = CredentialFlag.NONE;
        persistence = CredentialPersistence.LOCAL_MACHINE;
        lastUpdated = LocalDateTime.of(2000, 1, 1, 0, 0, 0);
    }

    /**
     * As with BeforeEach this doesn't serve any purpose just here for the sake of being here
     */
    @AfterEach
    void afterEach() {
    }

    @Test
    void constructor_throwsIllegalArgumentException_whenIdIsNull() {
        assertThrows(IllegalArgumentException.class, () -> new Credential(null, username, secret, flag, type, persistence, lastUpdated));
    }
    @Test
    void constructor_doesNotThrowIllegalArgumentException_whenUserNameIsNull() {
        assertDoesNotThrow(() -> new Credential(id, null, secret, flag, type, persistence, lastUpdated));
    }
    @Test
    void constructor_doesNotThrowException_whenCharacteristicsAreNone() {
        assertDoesNotThrow(() -> new Credential(id, username, secret, CredentialFlag.NONE, type, persistence, lastUpdated));
    }
    @Test
    void constructor_throwsIllegalArgumentException_whenTypeIsUnknown() {
        assertThrows(IllegalArgumentException.class, () -> new Credential(id, username, secret, flag, CredentialType.UNKNOWN, persistence, lastUpdated));
    }
    @Test
    void constructor_throwIllegalArgumentException_whenPersistentTypeIsUnknown() {
        assertThrows(IllegalArgumentException.class, () -> new Credential(id, username, secret, flag, type, CredentialPersistence.UNKNOWN, lastUpdated));
    }


    @ParameterizedTest(name = "{index} => flag={0} type={1} persistence={2}")
    @MethodSource("parametersProvider")
    void equals_returns_true_whenIdAndUsernameAndCredentialTypeAndPersistenceTypeEqual(CredentialTestCaseParameter parameter) {

        var first = new Credential(id, username, "secret1", parameter.Flag, parameter.Type, parameter.Persistence, lastUpdated);
        var second = new Credential(id, username, "secret2", CredentialFlag.NONE, parameter.Type, parameter.Persistence, lastUpdated.plus(1, ChronoUnit.DAYS));

        assertEquals(first, second);
    }

    private static Stream<CredentialTestCaseParameter> parametersProvider() {
        return Stream.of(
                CredentialTestCaseParameter.of(CredentialFlag.NONE, CredentialType.GENERIC, CredentialPersistence.LOCAL_MACHINE),
                CredentialTestCaseParameter.of(CredentialFlag.PROMPT_NOW, CredentialType.DOMAIN_PASSWORD, CredentialPersistence.ENTERPRISE),
                CredentialTestCaseParameter.of(CredentialFlag.NONE, CredentialType.GENERIC_CERTIFICATE, CredentialPersistence.SESSION));

    }

}

