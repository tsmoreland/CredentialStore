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

import com.sun.jna.LastErrorException;

import moreland.win32.credentialstore.converters.CredentialConverter;
import moreland.win32.credentialstore.internal.NativeInteropBridge;
import moreland.win32.credentialstore.internal.PreserveType;

public final class Win32CredentialManager implements CredentialManager {

    private NativeInteropBridge nativeInteropBridge;

    Win32CredentialManager(NativeInteropBridge nativeInteropBridge) {
		if (nativeInteropBridge == null) {
            throw new IllegalArgumentException("nativeInteropBridge is null");
        }

        this.nativeInteropBridge = nativeInteropBridge;
    }

    @Override
    public List<Credential> getAll() {
        return List.of();
    }

    private boolean addOrUpdate(Credential credential, PreserveType preserveType) {
        var win32Credential = CredentialConverter.toInternalCredentialReference(credential);
        if (!win32Credential.isPresent()) {
            return false;
        }

        try {
            nativeInteropBridge.credWrite(win32Credential.get(), preserveType);
            return true;
            
        } catch (LastErrorException e) {
            return false;
        }
    }

    @Override
    public boolean add(Credential credential) {
        return addOrUpdate(credential, PreserveType.NONE);
    }

    @Override
    public boolean update(Credential credential) {
        return addOrUpdate(credential, PreserveType.PRESERVE_CREDENTIAL_BLOB);
    }

    @Override
    public boolean delete(Credential credential) {
        if (credential == null) {
            throw new IllegalArgumentException("credential is null");
        }

        return delete(credential.getId(), credential.getType());
    }

    @Override
    public boolean delete(String id, CredentialType type) {
        try {
            return nativeInteropBridge.credDelete(id, type.getValue(), 0);

        } catch (LastErrorException e) {
            return false;
        }
    }

    @Override
    public Optional<Credential> find(String id, CredentialType type) {

        try (var win33Credential = nativeInteropBridge.credRead(id, type, 0)) {

            return win33Credential
                .value()
                .flatMap(CredentialConverter::fromInternalCredential);

        } catch (Exception e) {
            return Optional.empty();
        }
    }

    @Override
    public List<Credential> find(String filter, boolean searchAll) {
        return List.of();
    }
    
}