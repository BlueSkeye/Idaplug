using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum InstructionFeature
    {
        CF_STOP = 0x00001, // Instruction doesn't pass execution to the next instruction
        CF_CALL = 0x00002, // CALL instruction (should make a procedure here)
        CF_CHG1 = 0x00004, // The instruction modifies the first operand
        CF_CHG2 = 0x00008, // The instruction modifies the second operand
        CF_CHG3 = 0x00010, // The instruction modifies the third operand
        CF_CHG4 = 0x00020, // The instruction modifies 4 operand
        CF_CHG5 = 0x00040, // The instruction modifies 5 operand
        CF_CHG6 = 0x00080, // The instruction modifies 6 operand
        CF_USE1 = 0x00100, // The instruction uses value of the first operand
        CF_USE2 = 0x00200, // The instruction uses value of the second operand
        CF_USE3 = 0x00400, // The instruction uses value of the third operand
        CF_USE4 = 0x00800, // The instruction uses value of the 4 operand
        CF_USE5 = 0x01000, // The instruction uses value of the 5 operand
        CF_USE6 = 0x02000, // The instruction uses value of the 6 operand
        CF_JUMP = 0x04000, // The instruction passes execution using indirect jump or call (thus needs additional analysis)
        CF_SHFT = 0x08000, // Bit-shift instruction (shl,shr...)
        CF_HLL =  0x10000, // Instruction may be present in a high level language function.
    }
}
