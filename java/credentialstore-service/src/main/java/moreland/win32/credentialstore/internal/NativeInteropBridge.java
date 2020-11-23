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

import moreland.win32.credentialstore.CredentialType;

import moreland.win32.credentialstore.structures.Credential;

public interface NativeInteropBridge {
    
    /**
     * Deletes a credential from the users credential set
     * @param target
     * @param type Type of the credential to delete. Must be one of the
     *             {@code} CredentialType defined types. For a list of the
     *             defined types, see the Type member of the {@code Credential} 
     *             structure.
     * @param flags Reserved and must be zero.
     * @exception LastErrorException if operation fails
     */
    boolean credDelete(String target, int type, int flags) throws LastErrorException;

    /**
     * Returns a list of credentials matching the provided filter
     * @param filter null-terminated string that contains the filter for
     *               the returned credentials. Only credentials with a TargetName
     *               matching the filter will be returned. The filter specifies a name
     *               prefix followed by an asterisk. For instance, the filter "FRED*"
     *               will return all credentials with a TargetName beginning with the
     *               string "FRED".
     *               If null is specified, all credentials will be returned.
     * @param flag Set of {@code EnumerateFlag}
     * @return collection of matching credentials
     * @exception LastErrorException if operation fails
     */
    CredentialList credEnumerate(Optional<String> filter, EnumerateFlag flag) throws LastErrorException;

    /**
     * Frees the buffer allocated by any of the credentials api functions.
     * @param handle Pointer to the buffer to be freed.
     * @exception LastErrorException if operation fails
     */
    boolean credFree(Pointer handle) throws LastErrorException;

    /**
     * Returns {@code NativeCredential} containing the matching
     * credential if found; otherwise, null
     * @param target null-terminated string that contains the name 
     *               credential to read.
     * @param type Type of the credential to read. Type must be one of the
     *             of {@code CredentialType} type
     * @param reservedFlag currently reserved and must be 0
     * @return the matching {@code Credential}
     * @exception LastErrorException on error, including not found
     */
    CriticalCredentialHandle credRead(String target, CredentialType type, int reservedFlag) throws LastErrorException;

    /**
     * Creates or updates a credential
     * @param credential credential structure to be written.
     * @param flags Flags that control the function's operation.
     * @exception LastErrorException if operation fails
     */
    boolean credWrite(Credential.ByReference credential, PreserveType flags) throws LastErrorException;

}
