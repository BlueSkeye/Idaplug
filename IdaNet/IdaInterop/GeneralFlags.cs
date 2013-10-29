using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum GeneralFlags : byte
    {
        INFFL_LZERO = 0x01,            //  generate leading zeroes in numbers
        INFFL_ALLASM = 0x02,            //  may use constructs not supported by the target assembler
        INFFL_LOADIDC = 0x04,            //  loading an idc file that contains database info
    }
}
