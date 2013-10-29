using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum SegmentFlags : ushort
    {
        SFL_COMORG = 0x01, // IDP dependent field (IBM PC: if set, ORG directive is not commented out)
        SFL_OBOK = 0x02, // orgbase is present? (IDP dependent field)
        SFL_HIDDEN = 0x04, // is the segment hidden?
        SFL_DEBUG = 0x08, // is the segment created for the debugger? such segments are temporary and do not have permanent flags
        SFL_LOADER = 0x10, // is the segment created by the loader?
        SFL_HIDETYPE = 0x20, // hide segment type (do not print it in the listing)
    }
}
