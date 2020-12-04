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

import org.slf4j.Logger;
import org.springframework.stereotype.Component;

import moreland.win32.credentialstore.ErrorToStringService;
import moreland.win32.credentialstore.Guard;

@Component("criticalCredentialHandleFactory")
public class Win32CriticalCredentialHandleFactory implements CriticalCredentialHandleFactory {

    private final Advapi32Library advapi32;
    private final ErrorToStringService errorToStringService;
    private final Logger logger;

    public Win32CriticalCredentialHandleFactory(Advapi32Library advapi32, ErrorToStringService errorToStringService,
            Logger logger) {
        Guard.againstNull(advapi32, "advapi32");
        Guard.againstNull(errorToStringService, "errorToStringService");
        Guard.againstNull(logger, "logger");

        this.advapi32 = advapi32;
        this.errorToStringService = errorToStringService;
        this.logger = logger;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Optional<CriticalCredentialHandle> fromPointerByReference(PointerByReference handle) {
        try {
            return Optional.of(new Win32CriticalCredentialHandle(advapi32, handle, errorToStringService, logger));
        } catch (Exception e) {
            return Optional.empty();
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Optional<CriticalCredentialHandle> fromPointer(Pointer pointer) {
        try {
            return Optional.of(new Win32CriticalCredentialHandle(advapi32, pointer, errorToStringService, logger));
        } catch (Exception e) {
            return Optional.empty();
        }

    }
}
