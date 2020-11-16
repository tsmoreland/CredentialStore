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
package moreland.win32.credentialstore.cli.internal;

import moreland.win32.credentialstore.Guard;
import java.util.List;
import java.util.Optional;

import moreland.win32.credentialstore.CredentialManager;

public final class Win32CredentialExecutor implements CredentialExecutor {

    private CredentialManager credentialManager;

    public Win32CredentialExecutor(CredentialManager credentialManager) {
        Guard.againstNull(credentialManager, "credentialManager");

        this.credentialManager = credentialManager;
        // ...
    }

    @Override
    public Optional<CredentialStoreOperation> getOperation(String name) {
        switch (name.toLowerCase()) {
            case "add":
                return Optional.of(new CredentialStoreOperation(){
                    @Override
                    public boolean process(List<String> args) {
                        return add(args);
                    }
                });
            case "remove":
                return Optional.of(args -> remove(args));
            case "find":
                return Optional.of(args -> find(args));
            case "list":
                return Optional.of(args -> list(args));
            default:
                return Optional.empty();
        }
    }

    @Override
    public boolean add(List<String> args) {
        return false;
    }

    @Override
    public boolean remove(List<String> args) {
        return false;
    }

    @Override
    public boolean find(List<String> args) {
        return false;
    }

    @Override
    public boolean list(List<String> args) {
        return false;
    }
    
}
