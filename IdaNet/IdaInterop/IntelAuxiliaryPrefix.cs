using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum IntelAuxiliaryPrefix : ushort
    {
        aux_lock = 0x0001,
        aux_rep = 0x0002,
        aux_repne = 0x0004,
        aux_use32 = 0x0008,  // segment type is 32-bits
        aux_use64 = 0x0010,  // segment type is 64-bits
        aux_large = 0x0020,  // offset field is 32-bit (16-bit is not enough)
        aux_short = 0x0040,  // short (byte) displacement used
        aux_sgpref = 0x0080,  // a segment prefix byte is not used
        aux_oppref = 0x0100,  // operand size prefix byte is not used
        aux_adpref = 0x0200,  // address size prefix byte is not used
        aux_basess = 0x0400,  // SS based instruction
        aux_natop = 0x0800,  // operand size is not overridden by prefix
        aux_natad = 0x1000,  // addressing mode is not overridden by prefix
        aux_fpemu = 0x2000,  // FP emulator instruction
    }
}
