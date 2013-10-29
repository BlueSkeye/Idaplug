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
    // Structure to hold information about an instruction. This structure is
    // filled by the analysis step of IDP and used by the emulation and
    // conversion to text steps. The kernel uses this structure too.
    // All structure fields except cs, ip, ea, Operand.n, Operand.flags
    // are initialized to zero by the kernel. The rest should be filled
    // by ana().
    internal class Instruction
    {
        #region CLASS INITIALIZER
        static Instruction()
        {
            return;
        }
        #endregion

        #region CONSTRUCTORS
        internal Instruction(IntPtr nativeInstruction)
        {
            NativePointer = nativeInstruction;
            return;
        }
        #endregion

        #region PROPERTIES
        internal int CanonicCode
        {
            get { return Code - Processor.Current.FirstInstructionCode; }
        }

        internal InstructionFeature CanonicFeature
        {
            get
            {
                InstructionDescriptor descriptor = Descriptor;

                return (null == descriptor) ? 0 : descriptor.Feature;
            }
        }

        internal string CanonicMnemonic
        {
            get
            {
                InstructionDescriptor descriptor = Descriptor;

                return (null == descriptor) ? null : descriptor.Name;
            }
        }

        internal InstructionDescriptor Descriptor
        {
            get { return IsCanonic ? Processor.Current.GetInstructionDescriptor(CanonicCode) : null; }
        }

        internal bool IsCanonic
        {
            get { return Processor.Current.IsCanonicInstruction(Code); }
        }

        internal bool IsMacro
        {
            get { return (0 != (Flags & InstructionFlags.INSN_MACRO)); }
        }

        internal static Instruction LastAnalyzed { get; private set; }

        internal IntPtr NativePointer { get; private set; }
        #endregion

        #region METHODS
        internal void Dump()
        {
            Interactivity.Message("Inst : CS = {0:X08}, VA = {1:X08}, LA = {2:X08}, Size = {3}, Flags = {4}, Macro = {5}\r\n",
                SegmentBase, VirtualAddress, LinearAddress, Size, Flags, IsMacro);
            if (!IsCanonic) { Interactivity.Message("Not a canonic instuction\r\n"); }
            else
            {
                Interactivity.Message("Canonic : code {0}, feature {1}, menmonic {2}\r\n",
                    CanonicCode, CanonicFeature, CanonicMnemonic);
            }
            IntelInstruction intel = this as IntelInstruction;

            if (null != intel)
            {
            }
            // InteropConstants.Dump(NativePointer, 0x10);
            return;
        }

        internal static Instruction Get(EffectiveAddress at)
        {
            if (null == LastAnalyzed)
            {
                IntPtr nativeAddress =
                    InteropConstants.GetExportedData(ExportedDataIdentifier.ExportedCommandId);
                // TODO : Should implement a factory
                LastAnalyzed = new IntelInstruction(nativeAddress);
                Interactivity.Message(
                    "Last analyzed instruction pinned at 0x{0:X08} / 0x{0:X016}\r\n",
                    nativeAddress.ToInt32(), nativeAddress.ToInt64());
            }
            if (0 == decode_insn(at))
            {
                if (0 != create_insn(at)) { Interactivity.Message("Successfully created instruction at 0x{0:X08}\r\n", at); }
                else { Interactivity.Message("Failed to create instruction at 0x{0:X08}\r\n", at); }
            }
            else { Interactivity.Message("Already existing instruction found at 0x{0:X08}\r\n", at); }
            return LastAnalyzed;
        }
        #endregion

        #region IDA NATIVE FIELDS
        // Current segment base paragraph. Initialized by the kernel.
        /* 0x00 - 0x00 */
        // EffectiveAddress cs; // segment base (in paragraphs)
        internal EffectiveAddress SegmentBase
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x00, 0x00); }
        }

        // Virtual address of the instruction (address within the segment) Initialized by the kernel.
        /* 0x04 - 0x08 */
        // EffectiveAddress ip; // offset in the segment
        internal EffectiveAddress VirtualAddress
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x08, 0x04); }
        }

        // Linear address of the instruction. Initialized by the kernel.
        /* 0x08 - 0x10 */
        // EffectiveAddress ea; // instruction start addresses
        internal EffectiveAddress LinearAddress
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x10, 0x08); }
        }

        // Internal code of instruction. IDP should define its own instruction
        // codes. These codes are usually defined in ins.hpp. The array of instruction
        // names and features (ins.cpp) is accessed using this code.
        /* 0x0C - 0x18 */
        // ushort itype; // instruction code (see ins.hpp) only for canonical insns (not user defined!):
        internal ushort Code
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x18, 0x0C); }
        }
        
        // Size of instruction in bytes.
        // The analyzer should put here the actual size of the instruction.
        /* 0x0E - 0x1A */
        // ushort size; // instruction size in bytes
        internal ushort Size
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x1A, 0x0E); }
        }

        // Additinal information about the instruction.
        // You may use these field as you want.
        //union
        //{
            /* 0x10 - 0x1C */
            // ushort auxpref;             // processor dependent field
            internal ushort AuxiliaryPrefix
            {
                get { return MarshalingUtils.GetUShort(NativePointer, 0x1C, 0x10); }
            }

            //struct
            //{
            /* 0x10 - 0x22 */
            // byte low;
            /* 0x11 - 0x23 */
            // byte high;
            //} auxpref_chars;
        // };
        /* 0x12 - 0x1E */
        // byte segpref;                 // processor dependent field
        internal byte SegmentPrefix
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x1E, 0x12); }
        }

        /* 0x13 - 0x1F */
        // byte insnpref;                // processor dependent field
        internal byte InstructionPrefix
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x1F, 0x13); }
        }

        // Information about instruction operands.
        /* 0x14 - 0x20 */
        //#define UA_MAXOP        6
        //op_t Operands[UA_MAXOP];
        //#define Op1 Operands[0]
        //#define Op2 Operands[1]
        //#define Op3 Operands[2]
        //#define Op4 Operands[3]
        //#define Op5 Operands[4]
        //#define Op6 Operands[5]
        internal Operand this[int index]
        {
            get
            {
                IntPtr nativeOperand;
#if __EA64__
                nativeOperand = new IntPtr(NativePointer.ToInt64() + 0x20 + (index * 0x24));
#else
                nativeOperand = new IntPtr(NativePointer.ToInt64() + 0x14 + (index * 0x18));
#endif
                return (IntPtr.Zero == nativeOperand) ? null : new Operand(nativeOperand);
            }
        }

        /* 0xA4 - 0xF8 */
        // byte flags;                   // instruction flags
        internal InstructionFlags Flags
        {
            get { return (InstructionFlags)MarshalingUtils.GetByte(NativePointer, 0xF8, 0xA4); }
        }

        // TODO : Shouldn't we despite pragma pack round up to wor alignement ?
        internal static int NativeSize
        {
            get
            {
#if __EA64__
                return 0xF9;
#else
                return 0xA5;
#endif
            }
        }
        #endregion

        #region IDA NATIVE FUNCTIONS
        // Create an instruction at the specified address
        //      ea - linear address
        // This function checks if an instruction is present at the specified address
        // and will try to create one if there is none. It will fail if there is
        // a data item or other items hindering the creation of the new instruction.
        // This function will also fill the 'cmd' structure.
        // Returns the length of the instruction or 0
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int create_insn(EffectiveAddress ea);

        // Analyze the specified address and fill 'cmd'
        //      ea - linear address
        // This function does not modify the database
        // It just tries to intepret the specified address as an instruction and fills
        // the 'cmd' structure with the results.
        // Returns the length of the (possible) instruction or 0
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int decode_insn(EffectiveAddress ea);
        #endregion

        #region MANAGED FIELDS
        private static IntPtr _lastAnalyzed = IntPtr.Zero;
        #endregion
    }
}
