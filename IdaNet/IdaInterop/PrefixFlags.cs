using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum PrefixFlags : byte
    {
        PREF_SEGADR = 0x01,            // show segment addresses?
        PREF_FNCOFF = 0x02,            // show function offsets?
        PREF_STACK = 0x04,            // show stack pointer?
        PREF_VARMARK = 0x08,            // show asterisk for variable addresses?
    }
}
