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
    /// <summary>This specialized class adds processing power to the standard Instruction
    /// class. However no native value is added because the native instance construction
    /// is under control of IDA that knows nothing about an Intel Instruction.</summary>
    internal class IntelInstruction : Instruction
    {
        #region CONSTRUCTORS
        internal IntelInstruction(IntPtr native)
            : base(native)
        {
            return;
        }
        #endregion

        #region PROPERTIES
        internal bool IsJump
        {
            get
            {
                switch ((IntelInstructionCodes)this.Code)
                {
                    case IntelInstructionCodes.NN_ja: // Jump if Above (CF=0 & ZF=0)
                    case IntelInstructionCodes.NN_jae: // Jump if Above or Equal (CF=0)
                    case IntelInstructionCodes.NN_jb: // Jump if Below (CF=1)
                    case IntelInstructionCodes.NN_jbe: // Jump if Below or Equal (CF=1 | ZF=1)
                    case IntelInstructionCodes.NN_jc: // Jump if Carry (CF=1)
                    case IntelInstructionCodes.NN_jcxz: // Jump if CX is 0
                    case IntelInstructionCodes.NN_jecxz: // Jump if ECX is 0
                    case IntelInstructionCodes.NN_jrcxz: // Jump if RCX is 0
                    case IntelInstructionCodes.NN_je: // Jump if Equal (ZF=1)
                    case IntelInstructionCodes.NN_jg: // Jump if Greater (ZF=0 & SF=OF)
                    case IntelInstructionCodes.NN_jge: // Jump if Greater or Equal (SF=OF)
                    case IntelInstructionCodes.NN_jl: // Jump if Less (SF!=OF)
                    case IntelInstructionCodes.NN_jle: // Jump if Less or Equal (ZF=1 | SF!=OF)
                    case IntelInstructionCodes.NN_jna: // Jump if Not Above (CF=1 | ZF=1)
                    case IntelInstructionCodes.NN_jnae: // Jump if Not Above or Equal (CF=1)
                    case IntelInstructionCodes.NN_jnb: // Jump if Not Below (CF=0)
                    case IntelInstructionCodes.NN_jnbe: // Jump if Not Below or Equal (CF=0 & ZF=0)
                    case IntelInstructionCodes.NN_jnc: // Jump if Not Carry (CF=0)
                    case IntelInstructionCodes.NN_jne: // Jump if Not Equal (ZF=0)
                    case IntelInstructionCodes.NN_jng: // Jump if Not Greater (ZF=1 | SF!=OF)
                    case IntelInstructionCodes.NN_jnge: // Jump if Not Greater or Equal (ZF=1)
                    case IntelInstructionCodes.NN_jnl: // Jump if Not Less (SF=OF)
                    case IntelInstructionCodes.NN_jnle: // Jump if Not Less or Equal (ZF=0 & SF=OF)
                    case IntelInstructionCodes.NN_jno: // Jump if Not Overflow (OF=0)
                    case IntelInstructionCodes.NN_jnp: // Jump if Not Parity (PF=0)
                    case IntelInstructionCodes.NN_jns: // Jump if Not Sign (SF=0)
                    case IntelInstructionCodes.NN_jnz: // Jump if Not Zero (ZF=0)
                    case IntelInstructionCodes.NN_jo: // Jump if Overflow (OF=1)
                    case IntelInstructionCodes.NN_jp: // Jump if Parity (PF=1)
                    case IntelInstructionCodes.NN_jpe: // Jump if Parity Even (PF=1)
                    case IntelInstructionCodes.NN_jpo: // Jump if Parity Odd  (PF=0)
                    case IntelInstructionCodes.NN_js: // Jump if Sign (SF=1)
                    case IntelInstructionCodes.NN_jz: // Jump if Zero (ZF=1)
                    case IntelInstructionCodes.NN_jmp: // Jump
                    case IntelInstructionCodes.NN_jmpshort: // Jump Short (not used)
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region METHODS
        internal EffectiveAddress GuessJump()
        {
            if (IntelInstructionCodes.NN_jmp != (IntelInstructionCodes)this.Code)
            {
                throw new InvalidOperationException(this.CanonicMnemonic);
            }
            throw new NotImplementedException();
        }
        #endregion

        // 16-bit mode?
        internal bool mode16()
        {
            return (0 == (this.AuxiliaryPrefix & (ushort)(IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64)));
        }

        // 32-bit mode?
        internal bool mode32()
        {
            return (0 != (this.AuxiliaryPrefix & (ushort)IntelAuxiliaryPrefix.aux_use32));
        }

        // 64-bit mode?
        internal bool mode64()
        {
            return (0 != (this.AuxiliaryPrefix & (ushort)IntelAuxiliaryPrefix.aux_use64));
        }

        // natural address size (no prefixes)?
        internal bool natad()
        {
            return (0 != (this.AuxiliaryPrefix & (ushort)IntelAuxiliaryPrefix.aux_natad));
        }

        // natural operand size (no prefixes)?
        internal bool natop()
        {
            return (0 != (this.AuxiliaryPrefix & (ushort)IntelAuxiliaryPrefix.aux_natop));
        }

        // is current addressing 16-bit?
        internal bool ad16()
        {
            IntelAuxiliaryPrefix p = (IntelAuxiliaryPrefix)this.AuxiliaryPrefix & (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natad);
            return (p == IntelAuxiliaryPrefix.aux_natad) || (p == IntelAuxiliaryPrefix.aux_use32);
        }

        // is current addressing 32-bit?
        internal bool ad32()
        {
            IntelAuxiliaryPrefix p = (IntelAuxiliaryPrefix)this.AuxiliaryPrefix & (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natad);
            return (p == (IntelAuxiliaryPrefix.aux_natad | IntelAuxiliaryPrefix.aux_use32))
                || (0 == p)
                || (p == IntelAuxiliaryPrefix.aux_use64);
        }

        // is current addressing 64-bit?
        internal bool ad64()
        {
            IntelAuxiliaryPrefix p = (IntelAuxiliaryPrefix)this.AuxiliaryPrefix & (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natad);
            return p == (IntelAuxiliaryPrefix.aux_natad | IntelAuxiliaryPrefix.aux_use64);
        }

        // is current operand size 16-bit?
        internal bool op16()
        {
            IntelAuxiliaryPrefix p = (IntelAuxiliaryPrefix)this.AuxiliaryPrefix & (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natop);
            return (p == IntelAuxiliaryPrefix.aux_natop)                                 // 16-bit segment, no prefixes
                || (p == IntelAuxiliaryPrefix.aux_use32)                                 // 32-bit segment, 66h
                || (   (p == IntelAuxiliaryPrefix.aux_use64)
                    && (0 == (this.InstructionPrefix & REX_W)));      // 64-bit segment, 66h, no rex.w
        }

        // is current operand size 32-bit?
        internal bool op32()
        {
            IntelAuxiliaryPrefix p = (IntelAuxiliaryPrefix)this.AuxiliaryPrefix & (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natop);
          return (p == 0)                                         // 16-bit segment, 66h
              || (p == (IntelAuxiliaryPrefix.aux_use32 | IntelAuxiliaryPrefix.aux_natop))                     // 32-bit segment, no prefixes
              || (   (p == (IntelAuxiliaryPrefix.aux_use64 | IntelAuxiliaryPrefix.aux_natop))
                  && (0 == (this.InstructionPrefix & REX_W))); // 64-bit segment, 66h, no rex.w
        }

        //// is current operand size 64-bit?
        //internal bool op64()
        //{
        //  return (   mode64()
        //          && (   (0 != (this.InstructionPrefix & REX_W))
        //              || natop()
        //              && insn_default_opsize_64())); // 64-bit segment, rex.w or insns-64
        //}

        // does the operand have a stack based displacement? (sp or bp based)
        // bool has_stack_displ(Operand x);

        // return addressing width in form of dt_... constant
        internal OperandValueType address_dtyp()
        {
            return ad64()
                ? OperandValueType.dt_qword
                : ad32()
                    ? OperandValueType.dt_dword
                    : OperandValueType.dt_word;
        }

        // return operand width in form of dt_... constant
        internal OperandValueType operand_dtyp()
        {
            return
                //op64()
                //? OperandValueType.dt_qword
                //:
                op32()
                    ? OperandValueType.dt_dword
                    : op16()
                        ? OperandValueType.dt_word
                        : OperandValueType.dt_byte;
        }

        /// <summary>Get a flag that tell whether this instruction is I/O related or not.
        /// </summary>
        internal bool IsIO
        {
            get
            {
                switch ((IntelInstructionCodes)this.Code)
                {
                    case IntelInstructionCodes.NN_ins:
                    case IntelInstructionCodes.NN_outs:
                    case IntelInstructionCodes.NN_out:
                    case IntelInstructionCodes.NN_in:
                        return true;
                    default:
                        return false;
                }
            }
        }

        internal const int REX_W = 8;               // 64-bit operand size
        internal const int REX_R = 4;               // modrm reg field extension
        internal const int REX_X = 2;               // sib index field extension
        internal const int REX_B = 1;               // modrm r/m, sib base, or opcode reg fields extension
    }
}
