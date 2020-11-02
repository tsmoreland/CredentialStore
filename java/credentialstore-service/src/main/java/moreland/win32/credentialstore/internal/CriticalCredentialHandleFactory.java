package moreland.win32.credentialstore.internal;

import com.sun.jna.Pointer;

/**
 * Factory used to construct {@code CriticalCredentialHandle}
 */
public interface CriticalCredentialHandleFactory {
    /**
     * {@code CriticalCredentialHandle} builder
     * @param handle critical handle to wrap in {@code CriticalCredentialHandle}
     * @return {@code CriticalCredentialHandle} which may contain a handle 
     * if {@code handle} was valid 
     */
    CriticalCredentialHandle build(Pointer handle);
}
