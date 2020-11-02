package moreland.win32.credentialstore;

/**
 * Win32 Exception Wrapper
 */
public class Win32Exception extends Exception {

    /**
     * generated serial Version UID
     */
    private static final long serialVersionUID = 3105181476167821311L;
    private final int errorCode;

    public Win32Exception(int errorCode) {
        super();

        this.errorCode = errorCode;
    }

    /**
     * Win32 errorcode, the value represents an unsigned integer or DWORD value
     * @see <a href="https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes">System Error Codes</a>
     * @return Win32 error code 
     */
    public int getErrorCode() {
        return errorCode;
    }
    
}
