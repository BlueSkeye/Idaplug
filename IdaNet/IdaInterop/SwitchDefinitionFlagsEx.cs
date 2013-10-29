using System;

namespace IdaNet.IdaInterop
{
    internal enum SwitchDefinitionFlagsEx
    {
        SWI2_INDIRECT = 0x0001, // value table elements are used as indexes into the jump table
        SWI2_SUBTRACT = 0x0002, // table values are subtracted from the elbase instead of being addded
        SWI2_HXNOLOWCASE = 0x0004, // lowcase value should not be used by the decompiler (internal flag)
    }
}
