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

import java.io.PrintStream;
import java.util.List;
import java.util.Map;
import java.util.Optional;

import moreland.win32.credentialstore.CredentialManager;

public final class Win32CredentialExecutor implements CredentialExecutor {

    private CredentialManager credentialManager;
    private PrintStream outputStream;
    private static Map<String, String> usage;

    static {
        usage = Map.of(
            "add", "Usage: credentialStore.Cli add <type> <target> <username>",
            "remove", "Usage: credentialStore.Cli remove <target> (<type>)",
            "find", "Usage: CredentialStore.Cli find <filter> (<search all, defaults true>)", 
            "list", "Usage: CredentialStore.Cli list"
        );
    }

    public Win32CredentialExecutor(CredentialManager credentialManager, PrintStream outputStream) {
        Guard.againstNull(credentialManager, "credentialManager");
        Guard.againstNull(outputStream, "outputStream");

        this.credentialManager = credentialManager;
        this.outputStream = outputStream;
        // ...
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Optional<CredentialStoreOperation> getOperation(String name) {
        switch (name.toLowerCase()) {
            case "add":
                // long winded approach just to have it once
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

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean add(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get("add"));
            return true;
        }
        return false;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean remove(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get("remove"));
            return true;
        }
        return false;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean find(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get("find"));
            return true;
        }
        return false;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    @SuppressWarnings({"java:S3516"})
    public boolean list(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get("list"));
            return true;
        }

        credentialManager
            .getAll()
            .stream()
            .map(credential -> String.format("%s - %s:%s", credential.getId(), credential.getUsername(), credential.getSecret()))
            .forEach(outputStream::println);
        return true;
    }
    
}
