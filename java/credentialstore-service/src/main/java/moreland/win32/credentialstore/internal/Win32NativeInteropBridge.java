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

import java.util.List;
import java.util.Set;

import com.sun.jna.LastErrorException;
import com.sun.jna.Pointer;

import moreland.win32.credentialstore.CredentialType;
import moreland.win32.credentialstore.Pair;

final class Win32NativeInteropBridge implements NativeInteropBridge {

    private Advapi32Library advapi;

    public Win32NativeInteropBridge() {
        this(Advapi32Library.INSTANCE);
    }

    /**
     * creates a new instance of the Win32NativeInteropBridge class
     * @param advapi Advapi32 library interface
     * @exception IllegalArgumentException when advapi parameter is null
     */
    public Win32NativeInteropBridge(Advapi32Library advapi) {
        super();
        this.advapi = advapi;

        if (this.advapi == null) {
            throw new IllegalArgumentException("advapi32 library cannot be null");
        }
    }

    @Override
    public int credDelete(String target, int type, int flags) {
        // TODO Auto-generated method stub
        return 0;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public List<NativeCredential> credEnumerate(String filter, Set<EnumerateFlag> flag) {
        // TODO Auto-generated method stub
        return List.of();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public int credFree(Pointer handle) {

        try {
            advapi.CredFreeW(handle);
            return 0;

        } catch (LastErrorException e) {
            return e.getErrorCode();
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Pair<Integer, NativeCredential> credRead(String target, CredentialType type, int reservedFlag) {
        // TODO Auto-generated method stub
        return null;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public int credWrite(NativeCredential credential, PreserveType flags) {
        // TODO Auto-generated method stub
        return 0;
    }

}
