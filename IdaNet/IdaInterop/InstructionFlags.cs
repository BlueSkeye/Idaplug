using System;

namespace IdaNet.IdaInterop
{
    internal enum InstructionFlags
    {
        INSN_MACRO = 0x01, // macro instruction
        INSN_MODMAC = 0x02, // macros: may modify the database to make room for the macro insn
    }
}
