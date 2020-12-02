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

import java.util.Collections;
import java.util.Map;
import java.util.Optional;

import org.springframework.stereotype.Service;

import moreland.win32.credentialstore.ErrorToStringService;
import moreland.win32.credentialstore.ExpectedErrorCode;

@Service("errorToStringService")
public class Win32ErrorToStringService implements ErrorToStringService {
    
    private static Map<Integer, String> errorMessages;
    
    static {
        Win32ErrorToStringService.errorMessages = Collections.unmodifiableMap(Map.of(
            ExpectedErrorCode.NOT_FOUND.getValue(), String.format("Element not found. %x", ExpectedErrorCode.NOT_FOUND.getValue()),
            ExpectedErrorCode.NO_SUCH_LOGON_SESSION.getValue(), String.format("A specified logon session does not exist. It might already have been terminated. %x", ExpectedErrorCode.NO_SUCH_LOGON_SESSION.getValue()),
            ExpectedErrorCode.INVALID_FLAGS.getValue(), String.format("Invalid Flags. %x", ExpectedErrorCode.INVALID_FLAGS.getValue()),
            ExpectedErrorCode.INVALID_ARGUMENT.getValue(), String.format("Invalid argument %x", ExpectedErrorCode.INVALID_ARGUMENT.getValue()),
            ExpectedErrorCode.NONE.getValue(), ""
        ));
    }

    public Optional<String> getMessageFor(ExpectedErrorCode errorCode) {
        return getMessageFor(errorCode.getValue());
    }

    public Optional<String> getMessageFor(int errorCode) {
        return errorMessages.containsKey(errorCode)
            ? Optional.of(errorMessages.get(errorCode))
            : Optional.empty();
    }
}

