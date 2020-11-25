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

import java.util.List;
import java.util.Optional;

/**
 * Command Line interface request executor
 */
public interface CredentialExecutor {

    /**
     * Fetches a functional interface to one of the supported operations
     * @param name
     * @return Optional of matching {@code CredentialStoreOperation} if found;
     *         otherwise, empty
     */
    Optional<CredentialStoreOperation> getOperation(final String name);

    /**
     * Adds a new credential using the supplied arguments
     * @param args either type id username with password being prompted
     *             or help which will detail the required arguments
     * @return true if credential was added or help was requested;
     *         otherwise false
     */
    boolean add(final List<String> args);

    /**
     * Deletes a credential matching the supplied arguments
     * @param args 2 item list containing either id and credential type  
     * @return true if credential exists and is removed, or doesn't exist
     *         or using help; otherwise, false
     */
    boolean remove(final List<String> args);

    /**
     * Searches for credentials match a given filter
     * @param args args containing filter, and optional second argument,
     *             if second argument is not 'all' then search all flag
     *             is not set when calling native api
     * @return true if at least one match was found
     */
    boolean find(final List<String> args);

    /**
     * prints out all credentials for the current user
     * @param args none
     * @return true
     */
    boolean list(final List<String> args);
   

}
