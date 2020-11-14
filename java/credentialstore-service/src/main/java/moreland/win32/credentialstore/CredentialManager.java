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

import java.util.List;
import java.util.Optional;

/**
 * Win32 Credential Manager (Credential Repository) providing CRUD 
 * operations for Windows Credential Manager 
 */
public interface CredentialManager {

    /**
     * Returns all Credentials from the Users credential set
     */
    List<Credential> getAll();

    /**
     * adds provided credential to Win32 credential manager
     * @param credential credential to add
     * @exception IllegalArgumentException if {@code credential} is null
     */
    void add(Credential credential);

    /// <summary>
    ///  <paramref name="credential"/>
    /// </summary>
    /// <param name="credential">credential to update with values to update</param>
    /// <remarks>
    /// thinly veiled wrapper around <see cref="Add"/> as both do the same write operation,
    /// provided mostly for readability
    /// </remarks>
    /**
     * Updates existing credential with {@code credential}
     * @param credential credential to update
     * @exception IllegalArgumentException if {@code credential} is null
     */
    void update(Credential credential);

    /**
     * deletes a credential from the user's credential set
     * @param credential credential to be removed
     * @exception IllegalArgumentException if {@code credential} is null
     */
    void delete(Credential credential);

    /**
     * deletes a credential from the user's credential set
     * @param id id of item to be deleted
     * @param type type of the item to be deleted
     */
    void delete(String id, CredentialType type);

    /**
     * Finds a credential with the given id value and optionally {@code type}
     * @param id id of the credential to be found
     * @param type type of the matching credential
     * @return optional containing the matching credential if found
     */
    Optional<Credential> find(String id, CredentialType type);

    /**
     * Returns all credentials matching wildcard based {@code filter} 
     * @param filter filter using wildcards
     * @param searchAll if true all credentials are searched
     * @return list of credentials matching filter
     */
    List<Credential> find(String filter, boolean searchAll);
   
}
