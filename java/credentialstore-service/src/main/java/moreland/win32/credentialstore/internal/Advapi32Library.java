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

import com.sun.jna.win32.StdCallLibrary;

import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.Platform;

public interface Advapi32Library extends StdCallLibrary {
    Advapi32Library INSTANCE = Platform.isWindows()
        ? (Advapi32Library) Native.load("advapi32", Advapi32Library.class)
        : null;

    /**
     * CreadReadW
     * @param target        unique target identifiy the credential to read
     * @param type          integer value from {@Code moreland.win32.credentialstore.CredentialType}
     * @param reservedFlag  should always be 0
     * @param credentialPtr (output) on success stores the read credential,
     *                      must be freed with CredFreeW
     * @return 0 on success
     */
    @SuppressWarnings("java:S100")
    int CredReadW(String target, int type, int reservedFlag, Pointer credentialPtr);

    /**
     * CredWriteW
     * @param userCredential pointer to credential structure to write
     * @param flags typically 0 but may also be CRED_PRESERVE_CREDENTIAL_BLOB, which
     *              preserves the existing credential blob
     * @return 0 on success
     */
    @SuppressWarnings("java:S100")
    int CredWriteW(Pointer userCredential, int flags);

    /**
     * CredFreeW
     * @param cred pointer to credential to free, if null no action is taken
     * @return 0 on success
     */
    @SuppressWarnings("java:S100")
    int CredFreeW(Pointer cred);

    /**
     * CredDeleteW
     * @param target unique target identifiy the credential to read
     * @param type integer value from {@Code moreland.win32.credentialstore.CredentialType}
     * @param flags Reseved and must be 0
     * @return 0 on success
     */
    @SuppressWarnings("java:S100")
    int CredDeleteW(String target, int type, int flags);

    /**
     * CredEnumerateW
     * @param filter filter string used to limit results
     * @param flag typically 0x1 (CRED_ENUMERATE_ALL_CREDENTIALS)
     * @param count on success the number of returned entries
     * @param credentialsPtr pointer to array of credential objects
     * @return
     */
    @SuppressWarnings("java:S100")
    int CredEnumerateW(String filter, int flag, Pointer count, Pointer credentialsPtr);

    
}
