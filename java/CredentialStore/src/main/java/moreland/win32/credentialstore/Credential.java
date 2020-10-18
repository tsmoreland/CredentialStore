/*
 * Copyright © 2020 Terry Moreland
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

package moreland.win32.credentialstore;

import java.time.LocalDateTime;
import java.util.Objects;

/**
 * Credential Entity as populated by Win32 api
 */
public final class Credential {

    private final String id;
    private final String username;
    private final String secret;
    private final CredentialFlag characteristics;
    private final CredentialType type;
    private final CredentialPersistence persistenceType;
    private final LocalDateTime lastUpdated;

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
        if (!isNullOrEmpty(invalidArgumentName)) {
            throw new IllegalArgumentException(invalidArgumentName);
        }
    }

    /**
     * Unique Identifier, this represents the target name of the credential
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


    private static boolean isNullOrEmpty(String string) {
        return string == null || string.trim().isEmpty();
    }
    private String getInvalidArgumentNameOrEmpty() {
        if (isNullOrEmpty(id))
            return "id";
        if (type == CredentialType.UNKNOWN)
            return "type";
        return persistenceType == CredentialPersistence.UNKNOWN
                ? "PersistenceType"
                : "";
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean equals(Object o) {
        if (this == o)
            return true;
        if (o == null || getClass() != o.getClass())
            return false;

        Credential that = (Credential) o;

        return id.equals(that.id) &&
                username.equals(that.username) &&
                type == that.type &&
                persistenceType == that.persistenceType;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public int hashCode() {
        return Objects.hash(id, username, type, persistenceType);
    }
}
