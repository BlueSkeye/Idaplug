using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    // Operand of an instruction. This structure is filled by the analyzer.
    // Upon entrance to the analyzer, some fields of this structure are initialized:
    //      type    - o_void
    //      offb    - 0
    //      offo    - 0
    //      flags   - OF_SHOW
    /* Total size is 0x18 - 0x24 */
    internal class Operand
    {
        #region CONSTRUCTORS
        internal Operand(IntPtr nativeOperand)
        {
            NativePointer = nativeOperand;
            return;
        }
        #endregion

        #region PROPERTIES
        internal bool IsImmediate(MemoryChunkSize candidate)
        {
            return (Type == OperandType.o_imm) && (candidate == Value);
        }

        internal bool IsRegister(int candidate)
        {
            return (Type == OperandType.o_reg) && (candidate == RegistersCount);
        }

        internal IntPtr NativePointer { get; private set; }

        internal bool Showed
        {
            get { return (0 != (Flags & OperandFlags.OF_SHOW)); }
            set { Flags = (value) ? (Flags | OperandFlags.OF_SHOW) : (Flags & ~OperandFlags.OF_SHOW); }
        }
        #endregion

        #region METHODS
        internal void Dump()
        {
            Interactivity.Message("Op {0} type={1} flags={2} value type={3}\r\n",
                OperandIndex, Type, Flags, ValueType);
            InteropConstants.Dump(NativePointer, 0x40);
            return;
        }

        internal IntPtr GetNative()
        {
            if (IntPtr.Zero == NativePointer) { throw new InvalidOperationException(); }
            return NativePointer;
        }
        #endregion

        #region IDA NATIVE FIELDS
        // Number of operand. Initialized once at the start of work.
        // You have no right to change its value.
        /* 0x00 - 0x00 */
        // byte n;              // number of operand (0,1,2)
        internal byte OperandIndex
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x00, 0x00); }
        }

        // Type of operand. See above for explanations
        /* 0x01 - 0x01 */
        // optype_t type; // type of operand
        internal OperandType Type
        {
            get { return (OperandType)MarshalingUtils.GetByte(NativePointer, 0x01, 0x01); }
        }

        // Offset of operand value from the instruction start.
        // Of course this field is meaningful only for certain types of operands.
        // Leave it equal to zero if the operand has no offset.
        // This offset should point to the 'interesting' part of operand.
        // For example, it may point to the address of a function in
        //      call func
        // or it may point to bytes holding '5' in
        //      mov  ax, [bx+5]
        // Usually bytes pointed to this offset are relocated (have fixup information)
        /* 0x02 - 0x02 */
        // char offb; // offset of operand relative to instruction start 0 - unknown
        internal byte Offset
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x02, 0x02); }
        }

        // The same as above. Some operands have 2 numeric values used to
        // form operand. 'offo' is used for the second part of operand if it exists.
        // Currently this field is used only for outer offsets of Motorla processors.
        // Leave it equal to zero if the operand has no offset
        /* 0x03 - 0x03 */
        // char offo; // offset of operand relative to instruction start 0 - unknown
        internal byte Offset2
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x03, 0x03); }
        }
        
        // Some characteristics of operand
        /* 0x04 - 0x04 */
        // uchar         flags;
        internal OperandFlags Flags
        {
            get { return (OperandFlags)MarshalingUtils.GetByte(NativePointer, 0x04, 0x04); }
            set { MarshalingUtils.WriteByte(NativePointer, 0x04, 0x04, (byte)value); }
        }

        // Type of operand value. Usually first 9 types are used.
        // This is the type of the operand itself, not the size of the addressing mode.
        // for example, byte ptr [epb+32_bit_offset]  will have dt_byte type.
        /* 0x05 - 0x05 */
        // char          dtyp;
        internal OperandValueType ValueType
        {
            get { return (OperandValueType)MarshalingUtils.GetByte(NativePointer, 0x05, 0x05); }
        }

        // The following unions keep other information about the operand
        //union
        //{
        /* 0x06 - 0x06 */
        // uint16 reg;                 // number of register (o_reg)
        internal ushort RegistersCount
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x06, 0x06); }
        }

        /* 0x06 - 0x06 */
        // uint16 phrase;              // number of register phrase (o_phrase,o_displ) you yourself define numbers of phrases as you like
        internal ushort Phrase
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x06, 0x06); }
        }
        //};

        //  Next 12 bytes are used by mc68k for some float types
        // VALUE
        /* 0x08 - 0x08 */
        //union
        //{
        /* 0x08 - 0x08 */
            //MemoryChunkSize value;
            // 1) operand value (o_imm)
            // 2) outer displacement (o_displ+OF_OUTER_DISP) integer values should be in IDA's (little-endian) order
            // when using ieee_realcvt, floating point values should be in the processor's native byte order dt_double
            // values take up 8 bytes (value and addr fields for 32-bit modules) NB: in case a dt_dword/dt_qword
            // immediate is forced to float by user, the kernel converts it to processor's native order before calling
            // FP conversion routines
            internal MemoryChunkSize Value
            {
                get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x08, 0x08); }
            }

            //struct
            //{                    // this structure is defined for
            //    uint16 low;             // your convenience only
            //    uint16 high;
            //} value_shorts;
        //};
        /* 0x0C - 0x10 */
        // VIRTUAL ADDRESS (OFFSET WITHIN THE SEGMENT)
        //union
        //{
            /* 0x0C - 0x10 */
            // EffectiveAddress addr; // virtual address pointed or used by the operand (o_mem,o_displ,o_far,o_near)

            //struct
            //{                    // this structure is defined for
            //    uint16 low;             // your convenience only
            //    uint16 high;
            //} addr_shorts;
        //};
        /* 0x10 - 0x18 */
        // IDP SPECIFIC INFORMATION
        //union
        //{
            /* 0x10 - 0x18 */
            // EffectiveAddress specval;               // This field may be used as you want.
            //struct
            //{                    // this structure is defined for your convenience only
            //    uint16 low;             // IBM PC: segment register number (o_mem,o_far,o_near)
            //    uint16 high;            // IBM PC: segment selector value  (o_mem,o_far,o_near)
            //} specval_shorts;
        //};
        /* 0x14 - 0x20 */

        // The following fields are used only in idp modules
        // You may use them as you want to store additional information about
        // the operand
        /* 0x14 - 0x20 */
        // char specflag1;
        internal byte SpecialFlag1
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x14, 0x20); }
        }

        /* 0x15 - 0x21 */
        // char          specflag2;
        internal byte SpecialFlag2
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x15, 0x21); }
        }

        /* 0x16 - 0x22 */
        // char          specflag3;
        internal byte SpecialFlag3
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x16, 0x22); }
        }

        /* 0x17 - 0x23 */
        // char          specflag4;
        internal byte SpecialFlag4
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x17, 0x23); }
        }
        #endregion
    }
}
