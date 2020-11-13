package moreland.win32.credentialstore.structures;

import com.sun.jna.Structure;
import com.sun.jna.Structure.FieldOrder;

/**
 * @see <a href=
 *      "https://docs.microsoft.com/en-us/windows/win32/api/minwinbase/ns-minwinbase-filetime"></a>
 */
@FieldOrder({"lowDateTime", "highDateTime"})
public final class FileTime extends Structure {
    
    /**
     * The Low-order part of the file time.
     */
    @SuppressWarnings("java:S1104")
    public int lowDateTime = 0;
    /**
     * The high-order part of the file time.
     */
    @SuppressWarnings("java:S1104")
    public int highDateTime = 0;
}
