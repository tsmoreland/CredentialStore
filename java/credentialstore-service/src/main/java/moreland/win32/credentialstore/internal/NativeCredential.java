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

import com.sun.jna.Pointer;
import com.sun.jna.Structure;
import com.sun.jna.Structure.FieldOrder;

@FieldOrder({"Flags", "Type", "TargetName", "Comment", "LastWritten", "CredentialBlobSize", "CredentialBlob", "Persist", "AttributeCount", "Attributes", "TargetAlias", "UserName"})
class NativeCredential extends Structure {

    /**
     * A bit member that identifies characteristics of the credential. 
     * Undefined bits should be initialized as zero and not otherwise 
     * altered to permit future enhancement.
     * <see cref="CredentialFlag"/> for possible values
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public int Flags = 0;

    /**
     * The type of the credential. This member cannot be changed after 
     * the credential is created. The following values are valid.
     * <see cref="CredentialType"/> for possible values
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public int Type = 0;

    /**
     * The name of the credential. The <see cref="TargetName"/> and 
     * <see cref="Type"/> members uniquely identify the credential. 
     * This member cannot be changed after the credential is created. 
     * Instead, the credential with the old name should be deleted 
     * and the credential with the new name created.
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public String TargetName = "";

    /**
     * A string comment from the user that describes this credential. This 
     * member cannot be longer than 
     * <see cref="Limits.MaximumCredentialStringLength"/> characters.
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public String Comment = "";

    /**
     * The time, in Coordinated Universal Time (Greenwich Mean Time), of 
     * the last modification of the credential. For write operations, the 
     * value of this member is ignored.
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public FileTime LastWritten = new FileTime();

    /**
     * The size, in bytes, of the CredentialBlob member. This member 
     * cannot be larger than <see cref="Limits.MaxCredentialBlobSize"/> bytes
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public int CredentialBlobSize = 0;

    /**
     * Secret data for the credential. The CredentialBlob member can be 
     * both read and written.
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public Pointer CredentialBlob = Pointer.NULL;

    /**
     * Defines the persistence of this credential. This member can be read and written
     * <see cref="CredentialPersistence"/> for possible values
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public int Persist;

    /**
     * The number of application-defined attributes to be associated with 
     * the credential. This member can be read and written. Its value 
     * cannot be greater than CRED_MAX_ATTRIBUTES (64).
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public int AttributeCount = 0;

    /**
     * Application-defined attributes that are associated with the 
     * credential. This member can be read and written.
     */
    @SuppressWarnings({"java:S1104", "java:S116"})
    public CredentialAttribute.ByReference Attributes;

    /**
     * Alias for the <see cref="TargetName"/>  member. This member can be 
     * read and written. It cannot be longer than 
     * <see cref="Limits.MaximumCredentialStringLength"/> characters.
     */
    @SuppressWarnings("java:S1104")
    public String targetAlias = "";

    /**
     * The user name of the account used to connect to <see cref="TargetName"/>.
     * </summary>
     */
    @SuppressWarnings("java:S1104")
    public String userName = "";

    public static class ByReference extends NativeCredential implements Structure.ByReference {
        public ByReference() {
        }

        public ByReference(Pointer memory) {
            super(memory);
        }
    }

    public NativeCredential() {
        super();
    }

    public NativeCredential(Pointer memory) {
        super(memory); 
        read();
    }
}
