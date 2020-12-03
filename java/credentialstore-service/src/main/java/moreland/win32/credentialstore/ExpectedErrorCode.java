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

import java.util.Arrays;
import java.util.Optional;

/**
 * Expected WIN32 error codes
 * @param value expected integer value
 */
public enum ExpectedErrorCode {
    NONE(0),
    NOT_FOUND(0x00000490),
    NO_SUCH_LOGON_SESSION(0x000005200),
    INVALID_FLAGS(0x000003eC),
    INVALID_ARGUMENT(87);

    private final int value;
    public int getValue() {
        return value;
    }

    public static Optional<ExpectedErrorCode> fromInteger(int value) {
        return Arrays.stream(ExpectedErrorCode.class.getEnumConstants())
            .filter(e -> e.getValue() == value)
            .findFirst();
    }

    ExpectedErrorCode(int value) {
        this.value = value;
    }
}
