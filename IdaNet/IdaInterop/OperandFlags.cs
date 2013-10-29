using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum OperandFlags : byte
    {
        OF_NO_BASE_DISP = 0x80, // o_displ: base displacement doesn't exist meaningful only for o_displ type if set, base displacement (x.addr) doesn't exist.
        OF_OUTER_DISP = 0x40, // o_displ: outer displacement exists meaningful only for o_displ type if set, outer displacement (x.value) exists.
        PACK_FORM_DEF = 0x20, // !o_reg + dt_packreal: packed factor defined
        OF_NUMBER = 0x10, // can be output as number only if set, the operand can be converted to a number only
        OF_SHOW = 0x08, // should the operand be displayed? if clear, the operand is hidden and should not be displayed
    }
}
