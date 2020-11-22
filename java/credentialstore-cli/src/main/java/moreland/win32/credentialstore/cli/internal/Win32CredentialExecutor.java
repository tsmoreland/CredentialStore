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
import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

import moreland.win32.credentialstore.Credential;
import moreland.win32.credentialstore.CredentialFlag;
import moreland.win32.credentialstore.CredentialManager;
import moreland.win32.credentialstore.CredentialPersistence;
import moreland.win32.credentialstore.CredentialType;

public final class Win32CredentialExecutor implements CredentialExecutor {

    private CredentialManager credentialManager;
    private PrintStream outputStream;
    private static Map<String, String> usage;

    private static final String ADD = "add";
    private static final String REMOVE = "remove";
    private static final String FIND = "find";
    private static final String LIST = "list";


    static {
        usage = Map.of(
            ADD, "Usage: credentialStore.Cli add <type> <target> <username>",
            REMOVE, "Usage: credentialStore.Cli remove <target> (<type>)",
            FIND, "Usage: CredentialStore.Cli find <filter> (<search all, defaults true>)", 
            LIST, "Usage: CredentialStore.Cli list"
        );
    }

    public Win32CredentialExecutor(CredentialManager credentialManager, PrintStream outputStream) {
        Guard.againstNull(credentialManager, "credentialManager");
        Guard.againstNull(outputStream, "outputStream");

        this.credentialManager = credentialManager;
        this.outputStream = outputStream;
        // ...
    }

    private static String formatOutput(Credential credential) {
        return String.format("%s - %s:%s", credential.getId(), credential.getUsername(), credential.getSecret());
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Optional<CredentialStoreOperation> getOperation(String name) {
        switch (name.toLowerCase()) {
            case ADD:
                // long winded approach just to have it once
                return Optional.of(new CredentialStoreOperation(){
                    @Override
                    public boolean process(List<String> args) {
                        return add(args);
                    }
                });
            case REMOVE:
                return Optional.of(args -> remove(args));
            case FIND:
                return Optional.of(args -> find(args));
            case LIST:
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
        if ((!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) || args.size() < 3) {
            outputStream.println(usage.get(ADD));
            return true;
        }

        var type = CredentialType.fromString(args.get(0));
        if (!type.isPresent()) {
            // log not reognized type
            return false;
        }

        var target = args.get(1);
        var username = args.get(2);
        var secret = "secret"; // replace with call to an input handler, an obsucred one

        // maybe filter type for supported ones, ignore any we don't support - everything but generic and domain password

        return credentialManager.add(new Credential(
            target, 
            username, 
            secret, 
            CredentialFlag.NONE, 
            CredentialType.GENERIC, 
            CredentialPersistence.LOCAL_MACHINE,
            LocalDateTime.now()));
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean remove(List<String> args) {
        if (args.isEmpty() || "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get(REMOVE));
            return true;
        }

        final var target = args.get(0);

        if (args.size() > 1) {
            var type = CredentialType.fromString(args.get(1));
            if (!type.isPresent()) {
                // log not reognized type
                return false;
            }
            return credentialManager.delete(target, type.get());
        } else {
            var credential = credentialManager.getAll()
                .stream()
                .filter(c -> c.getId().equals(target))
                .findFirst();
            return credential.isPresent() && credentialManager.delete(credential.get());
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean find(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get(FIND));
            return true;
        }

        final var id = args.get(0);
        if (args.size() > 1) {
            var type = CredentialType.fromString(args.get(1));
            if (!type.isPresent()) {
                // log not reognized type
                return false;
            }

            var credential = credentialManager.find(id, type.get());
            outputStream.println(credential
                .map(Win32CredentialExecutor::formatOutput)
                .orElse(String.format("%s not found.", id)));
            return credential.isPresent();

        } else {
            var credentials = credentialManager.find(id, true);
            credentials
                .stream()
                .forEach(c -> outputStream.println(formatOutput(c)));
            return credentials.size() > 0;
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    @SuppressWarnings({"java:S3516"})
    public boolean list(List<String> args) {
        if (!args.isEmpty() && "help".equalsIgnoreCase(args.get(0))) {
            outputStream.println(usage.get(LIST));
            return true;
        }

        var credentials = credentialManager
            .getAll()
            .stream()
            .map(Win32CredentialExecutor::formatOutput)
            .collect(Collectors.toList());

        outputStream.println(String.format("Found %d credentials:", credentials.size()));
        credentials
            .stream()
            .forEach(outputStream::println);

        return true;
    }
    
}
