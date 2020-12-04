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

import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.slf4j.Logger;

import moreland.win32.credentialstore.ErrorToStringService;

@ExtendWith(MockitoExtension.class)
class Win32CriticalCredentialHandleFactoryTests {
    
    @Mock
    private Advapi32Library advapi32;

    @Mock
    private ErrorToStringService errorToStringService;

    @Mock
    private Logger logger;

    @Test
    void constructor_throwsIllegalArgumentException_whenAdvapi32IsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CriticalCredentialHandleFactory((Advapi32Library)null, errorToStringService, logger));
        assertTrue(ex.getMessage().contains("advapi32"));
    }
    
    @Test
    void constructor_throwsIllegalArgumentException_whenErrorToStringServiceIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CriticalCredentialHandleFactory(advapi32, (ErrorToStringService) null, logger));
        assertTrue(ex.getMessage().contains("errorToStringService"));
    }

    @Test
    void constructor_throwsIllegalArgumentException_whenLoggerIsNull() {
        var ex = assertThrows(IllegalArgumentException.class, () -> new Win32CriticalCredentialHandleFactory(advapi32, errorToStringService, (Logger) null));
        assertTrue(ex.getMessage().contains("logger"));
    }
}
