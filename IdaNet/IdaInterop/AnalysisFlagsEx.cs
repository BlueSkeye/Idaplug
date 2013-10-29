using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum AnalysisFlagsEx : ushort
    {
        AF2_JUMPTBL = 0x0001,          // Locate and create jump tables
        AF2_DODATA = 0x0002,          // Coagulate data segs at the final pass
        AF2_HFLIRT = 0x0004,          // Automatically hide library functions
        AF2_STKARG = 0x0008,          // Propagate stack argument information
        AF2_REGARG = 0x0010,          // Propagate register argument information
        AF2_CHKUNI = 0x0020,          // Check for unicode strings
        AF2_SIGCMT = 0x0040,          // Append a signature name comment for recognized anonymous library functions
        AF2_SIGMLT = 0x0080,          // Allow recognition of several copies of the same function
        AF2_FTAIL = 0x0100,          // Create function tails
        AF2_DATOFF = 0x0200,          // Automatically convert data to offsets
        AF2_ANORET = 0x0400,          // Perform 'no-return' analysis
        AF2_VERSP = 0x0800,          // Perform full SP-analysis (ph.verify_sp)
        AF2_DOCODE = 0x1000,          // Coagulate code segs at the final pass
        AF2_TRFUNC = 0x2000,          // Truncate functions upon code deletion
        AF2_PURDAT = 0x4000,          // Control flow to data segment is ignored
    }
}
