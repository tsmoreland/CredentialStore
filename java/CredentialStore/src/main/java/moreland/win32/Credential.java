/*
 * Copyright © 2020 Terry Moreland
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

package moreland.win32;

import java.time.LocalDateTime;

public class Credential {

    private String id;
    private String username;
    private String secret;
    private CredentialFlag characteristics;
    private CredentialType type;
    private CredentialPersistence persistenceType;
    private LocalDateTime lastUpdated;

    /**
     * Instantiates a new instance of the @see Credential class populated with the provided values
     * @param id unique id, maps to Target Name within Native Api
     * @param username username
     * @param secret secret to be stored securely by Native API
     * @param characteristics typically @see CredentialFlag.None, see Win32 CREDENTIAL type for more detail
     * @param type @see CredentialType
     * @param persistenceType @see CredentialPersistence
     * @param lastUpdated last updated value, not used for newly saved values
     * @throws IllegalArgumentException if id is null or empty, or either type or persistenceType are UNKNOWN
     */
    public Credential(String id, String username, String secret,
                      CredentialFlag characteristics, CredentialType type,
                      CredentialPersistence persistenceType,
                      LocalDateTime lastUpdated) {
        this.id = id;
        this.username = username;
        this.secret = secret;
        this.characteristics = characteristics;
        this.type = type;
        this.persistenceType = persistenceType;
        this.lastUpdated = lastUpdated;

        var invalidArgumentName = getInvalidArgumentNameOrEmpty();
        if (!isNullorEmpty(invalidArgumentName)) {
            throw new IllegalArgumentException(invalidArgumentName);
        }
    }

    /**
     * Unqiue Identifier, this represents the target name of the credential
     */
    public String getId() {
        return id;
    }

    /**
     * The user name of the account used to connect to @see getId
     */
    public String getUsername() {
        return username;
    }

    /**
     * Secret (password)
     */
    public String getSecret() {
        return secret;
    }

    /**
     * @see CredentialFlag
     */
    public CredentialFlag getCharacteristics() {
        return characteristics;
    }

    /**
     * @see CredentialFlag
     */
    public CredentialType getType() {
        return type;
    }

    /**
     * @see CredentialPersistence
     */
    public CredentialPersistence getPersistenceType() {
        return persistenceType;
    }

    /**
     * Last Updated time, only valid for reads
     */
    public LocalDateTime getLastUpdated() {
        return lastUpdated;
    }


    private static boolean isNullorEmpty(String string) {
        return string == null || string.trim().isEmpty();
    }
    private String getInvalidArgumentNameOrEmpty() {
        if (isNullorEmpty(id))
            return "id";
        if (type == CredentialType.UNKNOWN)
            return "type";
        return persistenceType == CredentialPersistence.UNKNOWN
                ? "PersistenceType"
                : "";
    }
}
