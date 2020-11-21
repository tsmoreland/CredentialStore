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

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.Arrays;
import java.util.stream.Stream;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.MethodSource;

class CredentialPersistenceTests {
    
    @ParameterizedTest(name = "{index} => {0}")
    @MethodSource("credentialPersistenceValueProvider")
    void fromInteger_returnsMatchingPreserveType_IfFound(CredentialPersistence expected) {
        var actual = CredentialPersistence.fromInteger(expected.getValue());
        assertEquals(expected, actual);
    }

    @Test
    void fromInteger_returnsUnknown_IfNotFound() {
        var actual = CredentialPersistence.fromInteger(42);

        assertEquals(CredentialPersistence.UNKNOWN, actual);
    }

    private static Stream<CredentialPersistence> credentialPersistenceValueProvider() {
        return Arrays.stream(CredentialPersistence.class.getEnumConstants());
    }
}
