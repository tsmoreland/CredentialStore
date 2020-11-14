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

import java.util.Optional;

import com.sun.jna.Pointer;
import com.sun.jna.ptr.PointerByReference;

import moreland.win32.credentialstore.structures.Credential;

public final class Win32CriticalCredentialHandle implements CriticalCredentialHandle {

    private final Advapi32Library advapi32;
    private final Optional<Credential> credential;

    public Win32CriticalCredentialHandle(PointerByReference credentialPtr) {
        this(Advapi32Library.INSTANCE, credentialPtr);
    }

    public Win32CriticalCredentialHandle(Advapi32Library advapi32, PointerByReference credentialPtr) {
        this.advapi32 = advapi32;

        Pointer ptr = credentialPtr != null
            ? credentialPtr.getValue()
            : Pointer.NULL;

        this.credential = !ptr.equals(Pointer.NULL)
            ? Optional.of(new Credential(ptr))
            : Optional.empty();
    }

    public Win32CriticalCredentialHandle(Pointer credentialPtr) {
        this(Advapi32Library.INSTANCE, credentialPtr);
    }

    public Win32CriticalCredentialHandle(Advapi32Library advapi32, Pointer credentialPtr) {
        this.advapi32 = advapi32;

        this.credential = credentialPtr != null
            ? Optional.of(new Credential(credentialPtr))
            : Optional.empty();
    }


    /**
     * {@inheritDoc}
     */
    @Override
    public void close() throws Exception {
        if (credential.isPresent()) {
            advapi32.CredFree(credential.get().getPointer());
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean isPresent() {
        return credential.isPresent();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Optional<Credential> value() {
        return credential;
    }
}
