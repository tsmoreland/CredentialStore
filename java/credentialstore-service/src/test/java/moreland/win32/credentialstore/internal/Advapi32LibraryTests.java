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

import org.junit.jupiter.api.Test;

import com.sun.jna.LastErrorException;
import com.sun.jna.ptr.PointerByReference;
import com.sun.jna.WString;

import moreland.win32.credentialstore.CredentialType;

class Advapi32LibraryTests {
    
    private final Advapi32Library advapi32 = Advapi32Library.INSTANCE;

    @Test
    void credRead_throwsLastErrorException() {

        assertThrows(LastErrorException.class, 
            () -> {
                try {
                    var type = CredentialType.GENERIC.getValue();
                    var credentialPtr = new PointerByReference();

                    /*
                    if (advapi32.CredReadW(new WString("test"), 1, 0, credentialPtr)) {
                        var credByReference = new Credential(credentialPtr.getValue());
                        System.out.println(String.format("%s %s", credByReference.targetName, credByReference.userName));
                    }

                    if (credentialPtr.getPointer() != null) {
                        advapi32.CredFree(credentialPtr.getPointer());
                    }
                    */

                    advapi32.CredReadW(new WString(""), type, 0, credentialPtr);

                    if (credentialPtr.getPointer() != null) {
                        advapi32.CredFree(credentialPtr.getPointer());
                    }
                } catch (NullPointerException e) {
                    System.out.println(e.getStackTrace().toString());
                    System.out.println(e.getMessage());
                }
            });
    }

}
