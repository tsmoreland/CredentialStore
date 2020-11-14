package moreland.win32.credentialstore.internal;

import com.sun.jna.Pointer;
import com.sun.jna.ptr.PointerByReference;

/**
 * Factory used to construct {@code CriticalCredentialHandle}
 */
public interface CriticalCredentialHandleFactory {

    /**
     * Returns an empty CriticalCredentialHandler, used to represent failure
     */
    CriticalCredentialHandle empty();

    /**
     * {@code CriticalCredentialHandle} builder
     * @param handle critical handle to wrap in {@code CriticalCredentialHandle}
     * @return {@code CriticalCredentialHandle} which may contain a handle 
     * if {@code handle} was valid 
     */
    CriticalCredentialHandle fromPointerByReference(PointerByReference handle);

    /**
     * {@code CriticalCredentialHandle} builder
     * @param handle critical handle to wrap in {@code CriticalCredentialHandle}
     * @return {@code CriticalCredentialHandle} which may contain a handle 
     * if {@code handle} was valid 
     */
    CriticalCredentialHandle fromPointer(Pointer handle);
}
