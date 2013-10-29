using System;
using System.Collections.Generic;
using System.Text;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum DatabaseMiscFlags : byte
    {
        LFLG_PC_FPP = 0x01, // decode floating point processor instructions?
        LFLG_PC_FLAT = 0x02, // Flat model?
        LFLG_64BIT = 0x04, // 64-bit program?
        LFLG_DBG_NOPATH = 0x08, // do not store input full path in debugger process options
        LFLG_SNAPSHOT = 0x10, // memory snapshot was taken?
        LFLG_IS_DLL = 0x20, // Is dynamic library?
    }
}
