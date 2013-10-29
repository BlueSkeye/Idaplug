using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum MemberProperties
    {
        MF_OK = 0x00000001,    // is the member ok? (always yes)
        MF_UNIMEM = 0x00000002,    // is a member of a union?
        MF_HASUNI = 0x00000004,    // has members of type "union"?
        MF_BYTIL = 0x00000008,    // the member was created due to the type system
        MF_HASTI = 0x00000010,    // has type information?
    }
}
