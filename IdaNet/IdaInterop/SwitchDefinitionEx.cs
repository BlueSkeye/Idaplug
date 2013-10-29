using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class SwitchDefinitionEx : SwitchDefinition
    {
        internal SwitchDefinitionEx(IntPtr native)
            : base(native)
        {
            return;
        }

        //switch_info_ex_t() { clear(); }

        //void clear()
        //{
        //memset(this, 0, sizeof(switch_info_ex_t));
        //cb = sizeof(switch_info_ex_t);
        //flags = SWI_EXTENDED;
        //jumps = EffectiveAddress.MaxValue;
        //defjump = EffectiveAddress.MaxValue;
        //startea = EffectiveAddress.MaxValue;
        //regnum = -1;
        //}

        internal void set_expr(int register, DataCrossReferenceType type)
        {
            RegisterNumber = register;
            Type = type;
            return;
        }

        internal bool IsIndirect
        {
            get
            {
                return ((0 != (Flags & SwitchDefinitionFlags.SWI_EXTENDED))
                    && (0 != (FlagsEx & SwitchDefinitionFlagsEx.SWI2_INDIRECT)));
            }
        }

        internal bool IsSubstract
        {
            get
            {
                return ((0 != (Flags & SwitchDefinitionFlags.SWI_EXTENDED))
                    && (0 != (FlagsEx & SwitchDefinitionFlagsEx.SWI2_SUBTRACT)));
            }
        }

        internal bool IsNoLowcase
        {
            get
            {
                return ((0 != (Flags & SwitchDefinitionFlags.SWI_EXTENDED))
                    && (0 != (FlagsEx & SwitchDefinitionFlagsEx.SWI2_HXNOLOWCASE)));
            }
        }

        internal int JumpTableSize
        {
            get { return IsIndirect ? JumpsCount : CasesCount; }
        }

        internal AddressDifference Lowcase
        {
            get { return IsIndirect ? LowcaseIndex : (AddressDifference)LowestCaseValue; }
        }

        /* 0x14 - 0x24 */
        // size_t cb; // sizeof(this)
        internal int StructureSize
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x24, 0x14); }
        }

        /* 0x18 - 0x28 */
        // int flags2;
        internal SwitchDefinitionFlagsEx FlagsEx
        {
            get { return (SwitchDefinitionFlagsEx)MarshalingUtils.GetInt32(NativePointer, 0x28, 0x18); }
        }

        /* 0x1C - 0x2C */
        // int jcases;                   // number of entries in the jump table (SWI2_INDIRECT)
        internal int JumpsCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x2C, 0x1C); }
        }

        /* 0x20 - 0x30 */
        // AddressDifference ind_lowcase;
        internal AddressDifference LowcaseIndex
        {
            get { return MarshalingUtils.GetAddressDifference(NativePointer, 0x30, 0x20); }
        }

        /* 0x24 - 0x38 */
        // EffectiveAddress elbase;                  // element base
        internal EffectiveAddress ElementBase
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x38, 0x24); }
        }

        /* 0x28 - 0x40 */
        // int regnum;                   // the switch expression as a register number of the instruction at 'startea'. -1 means 'unknown'
        internal int RegisterNumber
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x40, 0x28); }
            set { MarshalingUtils.SetInt32(NativePointer, 0x40, 0x28, value); }
        }

        /* 0x2C - 0x48 */
        // char regdtyp;                  // size of the switch expression register as dtyp
        internal DataCrossReferenceType Type
        {
            get { return (DataCrossReferenceType)MarshalingUtils.GetByte(NativePointer, 0x48, 0x2C); }
            set { MarshalingUtils.SetByte(NativePointer, 0x48, 0x2C, (byte)value); }
        }

        /* 0x2D - 0x49 */
        // MemoryChunkSize custom;                // information for custom tables (filled and used by modules)
        internal MemoryChunkSize Custom
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x49, 0x2D); }
        }
    }
}
