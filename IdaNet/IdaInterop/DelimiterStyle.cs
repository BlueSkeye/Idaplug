using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum DelimiterStyle : byte
    {
        LMT_THIN = 0x01,            //  thin borders
        LMT_THICK = 0x02,            //  thick borders
        LMT_EMPTY = 0x04,            //  empty lines at the end of basic blocks
    }
}
