using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdaNet.IdaInterop
{
    internal class IntelOperand : Operand
    {
        #region CONSTRUCTORS
        internal IntelOperand(IntPtr native)
            : base(native)
        {
            return;
        }
        #endregion

        // does the operand have a displacement?
        internal bool has_displ
        {
            get
            {
                return (this.Type == OperandType.o_displ)
                    || (   (this.Type == OperandType.o_mem)
                        && this.hasSIB);
            }
        }

        internal bool hasSIB
        {
            get { return 0 != this.SpecialFlag1; }
        }

        // get extended sib base
        internal int sib_base(IntelInstruction owner)
        {
            int @base = this.SpecialFlag2 & 7;
            if (0 != (owner.InstructionPrefix & IntelInstruction.REX_B)) { @base |= 8; }
            return @base;
        }

        // get extended sib index
        internal short sib_index(IntelInstruction owner)
        {
            short index = (short)((this.SpecialFlag2 >> 3) & 7);
            if (0 != (owner.InstructionPrefix & IntelInstruction.REX_X)) { index |= 8; }
            return index;
        }

        internal int sib_scale
        {
            get { return (this.SpecialFlag2 >> 6) & 3; }
        }

        // get the base register of the operand with a displacement
        internal int x86_base(IntelInstruction owner)
        {
            return this.hasSIB ? this.sib_base(owner) : this.Phrase;
        }

        const int INDEX_NONE = 4;       // no index register is present

        // get the index register of the operand with a displacement
        internal int x86_index(IntelInstruction owner)
        {
            return this.hasSIB ? sib_index(owner) : INDEX_NONE;
        }

        // get the scale factor of the operand with a displacement
        internal int x86_scale(IntelInstruction owner)
        {
            return this.hasSIB ? sib_scale : 0;
        }
    }
}
