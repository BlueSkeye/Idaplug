using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum CommentFlags : byte
    {
        SW_RPTCMT = 0x01,            //   show repeatable comments?
        SW_ALLCMT = 0x02,            //   comment all lines?
        SW_NOCMT = 0x04,            //   no comments at all
        SW_LINNUM = 0x08,            //   show source line numbers
        SW_TESTMODE = 0x10,           //   testida.idc is running
        SW_SHHID_ITEM = 0x20,            //   show hidden instructions
        SW_SHHID_FUNC = 0x40,            //   show hidden functions
        SW_SHHID_SEGM = 0x80,            //   show hidden segments
    }
}
