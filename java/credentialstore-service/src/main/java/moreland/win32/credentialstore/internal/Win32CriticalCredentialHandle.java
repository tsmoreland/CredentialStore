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

import com.sun.jna.LastErrorException;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.PointerByReference;

import org.slf4j.Logger;

import moreland.win32.credentialstore.ErrorToStringService;
import moreland.win32.credentialstore.Guard;
import moreland.win32.credentialstore.structures.Credential;

public final class Win32CriticalCredentialHandle implements CriticalCredentialHandle {

    private final Advapi32Library advapi32;
    private final Optional<Credential> credential;
    private final ErrorToStringService errorToStringService;
    private final Logger logger;

    public Win32CriticalCredentialHandle(Advapi32Library advapi32, PointerByReference credentialPtr,
            ErrorToStringService errorToStringService, Logger logger) throws Exception {

        Guard.againstNull(advapi32, "advapi32");
        this.advapi32 = advapi32;

        Pointer ptr = credentialPtr != null
            ? credentialPtr.getValue()
            : Pointer.NULL;

        this.credential = ptr != null 
            ? Optional.of(new Credential(ptr))
            : Optional.empty();

        try {
            Guard.againstNull(errorToStringService, "errorToStringService");
            Guard.againstNull(logger, "logger");

        } catch (Exception e) {
            close();
            throw e;
        }

        this.errorToStringService = errorToStringService;
        this.logger = logger;

    }

    public Win32CriticalCredentialHandle(Advapi32Library advapi32, Pointer credentialPtr, 
            ErrorToStringService errorToStringService, Logger logger) throws Exception {
        Guard.againstNull(advapi32, "advapi32");
        this.advapi32 = advapi32;
        this.credential = credentialPtr != null
            ? Optional.of(new Credential(credentialPtr))
            : Optional.empty();

        try {
            Guard.againstNull(errorToStringService, "errorToStringService");
            Guard.againstNull(logger, "logger");

        } catch (Exception e) {
            close();
            throw e;
        }

        this.errorToStringService = errorToStringService;
        this.logger = logger;
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

    /**
     * {@inheritDoc}
     */
    @Override
    public void close() throws Exception {
        try {
            if (credential.isPresent()) {
                advapi32.CredFree(credential.get().getPointer());
            }

        } catch (LastErrorException e) {
            if (logger != null && errorToStringService != null)  {
                logger.error(errorToStringService.getMessageFor(e.getErrorCode()).orElse("Unknown error occurred."), e);
            }
        }
    }

}
