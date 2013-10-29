using System;

namespace IdaNet.IdaInterop
{
    internal enum SegmentType : byte
    {
        SEG_NORM = 0,       // Unknown type, no assumptions
        SEG_XTRN = 1,       // * segment with 'extern' definitions no instructions are allowed
        SEG_CODE = 2,       // code segment
        SEG_DATA = 3,       // data segment
        SEG_IMP = 4,       // java: implementation segment
        SEG_GRP = 6,       // * group of segments
        SEG_NULL = 7,       // zero-length segment
        SEG_UNDF = 8,       // undefined segment type (not used)
        SEG_BSS = 9,       // uninitialized segment
        SEG_ABSSYM = 10,       // * segment with definitions of absolute symbols
        SEG_COMM = 11,       // * segment with communal definitions
        SEG_IMEM = 12,       // internal processor memory & sfr (8051)
    }
}
