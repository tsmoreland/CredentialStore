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

import java.util.Arrays;
import java.util.Iterator;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import com.sun.jna.Pointer;

import moreland.win32.credentialstore.Guard;
import moreland.win32.credentialstore.structures.Credential;

public class CredentialList implements Iterable<Credential>, AutoCloseable {

    private final Advapi32Library advapi32;
    private final Pointer pointer;
    private List<Credential> credentials = List.of();

    /**
     * instantiates a new instance of the CredentialList class
     * @exception IllegalArgumentException when advapi32 is null
     */
    public CredentialList(Pointer pointer, int count, Advapi32Library advapi32) {
        Guard.againstNull(advapi32, "advapi32");

        this.advapi32 = advapi32;
        this.pointer = pointer;

        if (pointer == null || count <= 0)
            return;

        credentials = Arrays.stream(this.pointer.getPointerArray(0, count))
            .map(Credential::new)
            .collect(Collectors.toList());
    }

    private static class EmptyHolder {
        static final CredentialList instance = new CredentialList(Pointer.NULL, 0, Advapi32Library.INSTANCE);
    }

    /**
     * null object instance
     * @return null object singleton
     */
    public static CredentialList empty() {
        return EmptyHolder.instance;
    }

    /**
     * Returns true if the list contains no elements
     * @return true if this list contains no elements
     */
    public boolean isEmpty() {
        return credentials.isEmpty();
    }

    /**
     * Returns the number of elements in the list
     * @return the number of elements in the list
     */
    public int size() {
        return credentials.size();
    }

    /**
     * Returns a sequential {@code Stream} with this collection as its source.
     * @return a sequential {@code Stream} over the elements in this collection
     */
    public Stream<Credential> stream() {
        return credentials.stream();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Iterator<Credential> iterator() {
        return credentials.iterator();
    }

    /**
     * {@inheritDoc}
     * @throws Exception rethrowsLastErrorException 
     *                   when CredFree throws it
     */
    @Override
    public void close() throws Exception {
        if (pointer != null)
            advapi32.CredFree(pointer);
    }

}
