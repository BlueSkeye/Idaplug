using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum CrossReferencesFlags : byte
    {
        SW_SEGXRF = 0x01,            // show segments in xrefs?
        SW_XRFMRK = 0x02,            // show xref type marks?
        SW_XRFFNC = 0x04,            // show function offsets?
        SW_XRFVAL = 0x08,            // show xref values? (otherwise-"...")
    }
}
