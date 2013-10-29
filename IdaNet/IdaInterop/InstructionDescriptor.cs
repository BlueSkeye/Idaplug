using System;
using System.Runtime.InteropServices;

namespace IdaNet.IdaInterop
{
    // IDA uses internal representation of processor instructions.
    // Definition of all internal instructions are kept in special arrays.
    // One of such arrays describes instruction names are features.
    internal class InstructionDescriptor
    {
        #region CONSTRUCTORS
        internal InstructionDescriptor(IntPtr at, int offset)
        {
            Name = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(at, offset));
            Feature = (InstructionFeature)Marshal.ReadInt32(at, offset + 4);
            return;
        }
        #endregion

        #region PROPERTIES
        /* 0x00 - 0x00 */
        // const char *name;
        internal string Name { get; private set; }

        /* 0x04 - 0x04 */
        // uint32 feature;
        internal InstructionFeature Feature { get; private set; }

        internal static int NativeSize
        {
            get { return 8; }
        }
        #endregion
    }
}
