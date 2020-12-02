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

import java.util.Optional;

import com.sun.jna.LastErrorException;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.IntByReference;
import com.sun.jna.ptr.PointerByReference;

import org.springframework.stereotype.Component;

import com.sun.jna.WString;

import moreland.win32.credentialstore.CredentialType;
import moreland.win32.credentialstore.Guard;
import moreland.win32.credentialstore.structures.Credential;

@Component("nativeInteropBridge")
public final class Win32NativeInteropBridge implements NativeInteropBridge {

    private Advapi32Library advapi32;
    private CriticalCredentialHandleFactory criticalCredentialHandleFactory;

    public Win32NativeInteropBridge() {
        this(Advapi32Library.INSTANCE, new Win32CriticalCredentialHandleFactory(Advapi32Library.INSTANCE));
    }

    /**
     * creates a new instance of the Win32NativeInteropBridge class
     * 
     * @param advapi Advapi32 library interface
     * @exception IllegalArgumentException when advapi parameter is null
     */
    public Win32NativeInteropBridge(Advapi32Library advapi32, CriticalCredentialHandleFactory criticalCredentialHandleFactory) {
        super();

        Guard.againstNull(advapi32, "advapi32");
        Guard.againstNull(criticalCredentialHandleFactory, "criticalCredentialHandleFactory");

        this.advapi32 = advapi32;
        this.criticalCredentialHandleFactory = criticalCredentialHandleFactory;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean credDelete(String target, int type, int flags) throws LastErrorException {
        synchronized(advapi32) {
            return advapi32.CredDeleteW(new WString(target), type, flags);
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CredentialList credEnumerate(Optional<String> filter, EnumerateFlag flag) throws LastErrorException {

        var count = new IntByReference();
        var credentialsPtr = new PointerByReference();

        synchronized(advapi32) {
            return advapi32.CredEnumerateW(filter.map(WString::new).orElse(null), flag.getValue(), count, credentialsPtr)
                ? new CredentialList(credentialsPtr.getValue(), count.getValue(), advapi32)
                : CredentialList.empty();
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean credFree(Pointer handle) throws LastErrorException {
        synchronized(advapi32) {
            return advapi32.CredFree(handle);
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CriticalCredentialHandle credRead(String target, CredentialType type, int reservedFlag) throws LastErrorException {

        var credentialPtr = new PointerByReference();
        synchronized(advapi32) {
            if (!advapi32.CredReadW(new WString(target), type.getValue(), reservedFlag, credentialPtr))
                return criticalCredentialHandleFactory.empty();
        }
        return criticalCredentialHandleFactory.fromPointerByReference(credentialPtr);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public boolean credWrite(Credential.ByReference credential, PreserveType flags) throws LastErrorException {
        synchronized(advapi32) {
            return advapi32.CredWriteW(credential, flags.getValue());
        }
    }

}
