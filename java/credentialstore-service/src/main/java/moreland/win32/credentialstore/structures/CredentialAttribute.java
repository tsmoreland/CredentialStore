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
package moreland.win32.credentialstore.structures;

import com.sun.jna.Structure;
import com.sun.jna.Structure.FieldOrder;

/**
 * The CREDENTIAL_ATTRIBUTE structure contains an application-defined 
 * attribute of the credential. An attribute is a keyword-value pair. 
 * It is up to the application to define the meaning of the attribute.
 */
@FieldOrder({"keyword", "flags", "valueSize", "value"})
public class CredentialAttribute extends Structure {

    /**
     * Name of the application-specific attribute. 
     * Names should be of the form <CompanyName>_<Name>. 
     * This member cannot be longer than CRED_MAX_STRING_LENGTH 
     * (256) characters.
     */
    @SuppressWarnings({"java:S1104"})
    public String keyword = "";

    /**
     * Identifies characteristics of the credential attribute. This member is 
     * reserved and should be originally initialized as zero and not otherwise 
     * altered to permit future enhancement.
     */
    @SuppressWarnings({"java:S1104"})
    public int flags = 0;

    /**
     * Length of Value in bytes. This member cannot be larger 
     * than CRED_MAX_VALUE_SIZE (256).
     */
    @SuppressWarnings({"java:S1104"})
    public int valueSize = 0;

    /**
     * Data associated with the attribute. By convention, if Value is a text 
     * string, then Value should not include the trailing zero character and 
     * should be in UNICODE. 
     * Credentials are expected to be portable. The application should take 
     * care to ensure that the data in value is portable. It is the 
     * responsibility of the application to define the byte-endian and 
     * alignment of the data in Value.
     */
    @SuppressWarnings({"java:S1104"})
    public byte[] value = new byte[256];

    public static class ByReference extends CredentialAttribute implements Structure.ByReference {
    }

}
