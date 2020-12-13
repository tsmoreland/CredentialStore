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

import com.sun.jna.LastErrorException;
import com.sun.jna.Pointer;
import com.sun.jna.WString;
import com.sun.jna.ptr.IntByReference;
import com.sun.jna.ptr.PointerByReference;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import moreland.win32.credentialstore.ExpectedErrorCode;
import moreland.win32.credentialstore.structures.Credential.ByReference;

/**
 * mostly empty implementation of {@code Advapi32Library}
 */
class UnsupportedAdvapi32Library implements Advapi32Library {

    private static final String ERROR_MESSAGE = "Unsupported operating system, all operations will fail with this result in this environment";
    private Logger logger;

    /**
     * Instantiates a new instance of the unsupported logger class
     */
    public UnsupportedAdvapi32Library() {
        logger = LoggerFactory.getLogger(UnsupportedAdvapi32Library.class);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean CredReadW(WString target, int type, int reservedFlag, PointerByReference credential)
            throws LastErrorException {

        logger.error(ERROR_MESSAGE);
        credential.setPointer(Pointer.NULL);
        throw new LastErrorException(ExpectedErrorCode.NOT_SUPPORTED.getValue());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean CredWriteW(ByReference userCredential, int flags) throws LastErrorException {
        logger.error(ERROR_MESSAGE);
        throw new LastErrorException(ExpectedErrorCode.NOT_SUPPORTED.getValue());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean CredFree(Pointer cred) throws LastErrorException {
        logger.error(ERROR_MESSAGE);
        throw new LastErrorException(ExpectedErrorCode.NOT_SUPPORTED.getValue());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean CredDeleteW(WString target, int type, int flags) throws LastErrorException {
        logger.error(ERROR_MESSAGE);
        throw new LastErrorException(ExpectedErrorCode.NOT_SUPPORTED.getValue());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean CredEnumerateW(WString filter, int flag, IntByReference count, PointerByReference credentialsPtr)
            throws LastErrorException {

        logger.error(ERROR_MESSAGE);
        credentialsPtr.setValue(Pointer.NULL);
        throw new LastErrorException(ExpectedErrorCode.NOT_SUPPORTED.getValue());
    }
}
