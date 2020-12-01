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
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

import com.sun.jna.Memory;
import com.sun.jna.WString;

import org.springframework.stereotype.Component;

import moreland.win32.credentialstore.Credential;
import moreland.win32.credentialstore.CredentialFlag;
import moreland.win32.credentialstore.CredentialPersistence;
import moreland.win32.credentialstore.CredentialType;

@Component("credentialConverter")
public final class Win32CredentialConverter implements CredentialConverter {

    private static final String TARGET_NAME_PREFIX = "LegacyGeneric:target=";

    public Win32CredentialConverter() {
        // ... provided for spring to find ...
    }

    @Override
    public Optional<Credential> fromInternalCredential(moreland.win32.credentialstore.structures.Credential source) {
        try {

            final var id = fromNullOrWString(source.targetName).replace(TARGET_NAME_PREFIX, "");
            final var type = CredentialType.fromInteger(source.type);
            var typesWithSecret = List.of(CredentialType.DOMAIN_PASSWORD, CredentialType.GENERIC);

            String secret = "";
            if (typesWithSecret.contains(type) && source.credentialBlobSize > 0) {
                final var byteArray = source.credentialBlob.getByteArray(0, source.credentialBlobSize);
                secret = new String(byteArray, StandardCharsets.UTF_16LE);
            }

            return Optional.of(new Credential(
                id, //fromNullOrWString(source.targetName).replace(TARGET_NAME_PREFIX, ""),
                fromNullOrWString(source.userName),
                secret,
                CredentialFlag.fromInteger(source.flags),
                type,
                CredentialPersistence.fromInteger(source.persist),
                LocalDateTime.now()));

        } catch (IllegalArgumentException | NullPointerException e) {
            return Optional.empty();
        }
    }
    
    @Override
    public Optional<moreland.win32.credentialstore.structures.Credential> toInternalCredential(Credential source) {
        if (source == null) {
            return Optional.empty();
        }

        try {
            var credential = new moreland.win32.credentialstore.structures.Credential(); 

            populateInternalCredential(credential, source);
            return Optional.of(credential);

        } catch (UnsupportedEncodingException e) {
            return Optional.empty();
        }
    }

    public Optional<moreland.win32.credentialstore.structures.Credential.ByReference> toInternalCredentialReference(Credential source) {
        if (source == null) {
            return Optional.empty();
        }

        try {
            var credential = new moreland.win32.credentialstore.structures.Credential.ByReference(); 

            populateInternalCredential(credential, source);
            return Optional.of(credential);

        } catch (UnsupportedEncodingException e) {
            return Optional.empty();
        }
    }

    private static void populateInternalCredential(moreland.win32.credentialstore.structures.Credential destination, Credential source) 
        throws UnsupportedEncodingException {

        destination.targetName = new WString(source.getId());
        destination.userName = new WString(source.getUsername());
        destination.type = source.getType().getValue();
        destination.persist = source.getPersistenceType().getValue();

        var credentialBlob = source.getSecret().getBytes("UTF-16LE");
        var credentialBlobMemory = new Memory(credentialBlob.length);
        credentialBlobMemory.write(0, credentialBlob, 0, credentialBlob.length);

        destination.credentialBlob = credentialBlobMemory;
        destination.credentialBlobSize = (int)credentialBlobMemory.size();

    }

    private static String fromNullOrWString(WString source) {
        return source != null
            ? source.toString()
            : "";
    }
}
