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
package moreland.win32.credentialstore.converters;

import java.io.UnsupportedEncodingException;
import java.time.LocalDateTime;
import java.util.Optional;

import com.sun.jna.Memory;
import com.sun.jna.WString;

import moreland.win32.credentialstore.Credential;
import moreland.win32.credentialstore.CredentialFlag;
import moreland.win32.credentialstore.CredentialPersistence;
import moreland.win32.credentialstore.CredentialType;

public final class CredentialConverter {

    private CredentialConverter() {
        // ... hidden constructor ...
    }

    public static Optional<Credential> fromInternalCredential(moreland.win32.credentialstore.structures.Credential source) {
        try {

            var secret = source.credentialBlobSize > 0
                ? new String(source.credentialBlob.getByteArray(0, source.credentialBlobSize) , "UTF-16LE" ) 
                : "";

            return Optional.of(new Credential(
                fromNullOrWString(source.targetName),
                fromNullOrWString(source.userName),
                secret,
                CredentialFlag.fromInteger(source.flags),
                CredentialType.fromInteger(source.type),
                CredentialPersistence.fromInteger(source.persist),
                LocalDateTime.now()));

        } catch (IllegalArgumentException | NullPointerException | UnsupportedEncodingException e) {
            return Optional.empty();
        }
    }
    
    public static Optional<moreland.win32.credentialstore.structures.Credential> toInternalCredential(Credential source) {
        if (source == null) {
            return Optional.empty();
        }

        try {
            var credential = new moreland.win32.credentialstore.structures.Credential(); 

            credential.targetName = new WString(source.getId());
            credential.userName = new WString(source.getUsername());
            credential.type = source.getType().getValue();
            credential.persist = source.getPersistenceType().getValue();

            var credentialBlob = source.getSecret().getBytes("UTF-16LE");
            var credentialBlobMemory = new Memory(credentialBlob.length);
            credentialBlobMemory.write(0, credentialBlob, 0, credentialBlob.length);

            credential.credentialBlob = credentialBlobMemory;
            credential.credentialBlobSize = (int)credentialBlobMemory.size();

            return Optional.of(credential);

        } catch (UnsupportedEncodingException e) {
            return Optional.empty();
        }
    }

    private static String fromNullOrWString(WString source) {
        return source != null
            ? source.toString()
            : "";
    }
}
