using System;
using System.Collections.Generic;
using System.Text;

#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using SegmentSelector = System.UInt64;
using SignedSize = System.Int64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using SegmentSelector = System.UInt32;
using SignedSize = System.Int32;
#endif

namespace IdaNet.IdaInterop
{
    internal class SwitchDefinition
    {
        internal SwitchDefinition(IntPtr native)
        {
            NativePointer = native;
            return;
        }

        #region PROPERTIES

        internal int Shift
        {
            get { return ((int)(Flags & SwitchDefinitionFlags.SWI_SHIFT_MASK) >> 7); }
            set
            {
                Flags &= ~SwitchDefinitionFlags.SWI_SHIFT_MASK;
                Flags = (SwitchDefinitionFlags)(ushort)((int)Flags | ((value & 3) << 7));
                return;
            }
        }


        internal int JumpTableElementSize
        {
            get
            {
                // this brain damaged logic is needed for compatibility with old versions
                SwitchDefinitionFlags code = Flags & (SwitchDefinitionFlags.SWI_J32 | SwitchDefinitionFlags.SWI_JSIZE);

                if (code == 0) { return 2; }
                if (code == SwitchDefinitionFlags.SWI_J32) { return 4; }
                if (code == SwitchDefinitionFlags.SWI_JSIZE) { return 1; }
                return 8;
            }
            set
            {
                Flags &= ~(SwitchDefinitionFlags.SWI_J32 | SwitchDefinitionFlags.SWI_JSIZE);

                switch (value)
                {
                    case 1:
                        Flags |= SwitchDefinitionFlags.SWI_JSIZE;
                        return;
                    case 2:
                        return;
                    case 4:
                        Flags |= SwitchDefinitionFlags.SWI_J32;
                        return;
                    case 8:
                        Flags |= SwitchDefinitionFlags.SWI_J32 | SwitchDefinitionFlags.SWI_JSIZE;
                        return;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        internal IntPtr NativePointer { get; private set; }

        internal int ValueTableElementSize
        {
            get
            {
                SwitchDefinitionFlags code = Flags & (SwitchDefinitionFlags.SWI_V32 | SwitchDefinitionFlags.SWI_VSIZE);

                switch (code)
                {
                    case 0:
                        return 2;
                    case SwitchDefinitionFlags.SWI_V32:
                        return 4;
                    case SwitchDefinitionFlags.SWI_VSIZE:
                        return 1;
                    default:
                        return 8;
                }
            }
            set
            {
                Flags &= ~SwitchDefinitionFlags.SWI_V32 | SwitchDefinitionFlags.SWI_VSIZE;

                switch (value)
                {
                    case 1:
                        Flags |= SwitchDefinitionFlags.SWI_VSIZE;
                        return;
                    case 2:
                        return;
                    case 4:
                        Flags |= SwitchDefinitionFlags.SWI_V32;
                        return;
                    case 8:
                        Flags |= SwitchDefinitionFlags.SWI_V32 | SwitchDefinitionFlags.SWI_VSIZE;
                        return;
                    default:
                        throw new ArgumentException();
                }
            }
        }
        #endregion

        #region IDA NATIVE FIELDS
        /* 0x00 - 0x00 */
        // ushort flags;
        internal SwitchDefinitionFlags Flags
        {
            get { return (SwitchDefinitionFlags)MarshalingUtils.GetUShort(NativePointer, 0x00, 0x00); }
            set { MarshalingUtils.SetUShort(NativePointer, 0x00, 0x00, (ushort)value); }
        }

        /* 0x02 - 0x02 */
        // ushort ncases;                // number of cases (excluding default)
        internal ushort CasesCount
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x02, 0x02); }
        }

        /* 0x04 - 0x04 */
        // EffectiveAddress jumps;                   // jump table address
        internal EffectiveAddress Jumps
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x04, 0x04); }
        }

        //union
        //{
        /* 0x08 - 0x0C */
        EffectiveAddress values;                // values table address (SWI_SPARSE)
        /* 0x08 - 0x0C */
        // MemoryChunkSize lowcase;             // the lowest value in cases
        internal MemoryChunkSize LowestCaseValue
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x0C, 0x08); }
        }
        // };

        /* 0x0C - 0x14 */
        // EffectiveAddress defjump;                 // default jump address
        internal EffectiveAddress DefaultJumpAddress
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x0C, 0x14); }
        }

        /* 0x10 - 0x1C */
        // EffectiveAddress startea;                 // start of switch idiom
        internal EffectiveAddress SwitchStart
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x10, 0x1C); }
        }

        internal static int NativeSize
        {
            get
            {
#if __EA64__
                return 0x24;
#else
                return 0x14;
#endif
            }
        }
        #endregion
    }

}
