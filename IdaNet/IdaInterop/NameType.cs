using System;

namespace IdaNet.IdaInterop
{
    internal enum NameType : byte
    {
        NM_REL_OFF = 0,
        NM_PTR_OFF = 1,
        NM_NAM_OFF = 2,
        NM_REL_EA = 3,
        NM_PTR_EA = 4,
        NM_NAM_EA = 5,
        NM_EA = 6,
        NM_EA4 = 7,
        NM_EA8 = 8,
        NM_SHORT = 9,
        NM_SERIAL = 10,
    }
}
