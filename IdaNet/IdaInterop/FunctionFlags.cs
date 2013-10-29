using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum FunctionFlags : ushort
    {
        FUNC_NORET = 0x0001,     // Function doesn't return
        FUNC_FAR = 0x0002,     // Far function
        FUNC_LIB = 0x0004,     // Library function
        FUNC_STATICDEF = 0x0008, // Static function
        FUNC_FRAME = 0x0010, // Function uses frame pointer (BP)
        FUNC_USERFAR = 0x0020, // User has specified far-ness of the function
        FUNC_HIDDEN = 0x0040, // A hidden function chunk
        FUNC_THUNK = 0x0080, // Thunk (jump) function
        FUNC_BOTTOMBP = 0x0100, // BP points to the bottom of the stack frame
        FUNC_NORET_PENDING = 0x00200, // Function 'non-return' analysis must be performed. This flag is verified upon func_does_return()
        FUNC_SP_READY = 0x0400, // SP-analysis has been performed If this flag is on, the stack change points should not be not modified anymore. Currently this analysis is performed only for PC
        FUNC_PURGED_OK = 0x4000, // 'functionArgumentsSize' field has been validated. If this bit is clear and 'functionArgumentsSize' is 0, then we do not known the real number of bytes removed from the stack. This bit is handled by the processor module.
        FUNC_TAIL = 0x8000, // This is a function tail. Other bits must be clear (except FUNC_HIDDEN)
    }
}
