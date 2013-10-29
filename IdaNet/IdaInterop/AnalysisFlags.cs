using System;

namespace IdaNet.IdaInterop
{
    internal enum AnalysisFlags : ushort
    {
        AF_FIXUP = 0x0001, // Create offsets and segments using fixup info
        AF_MARKCODE = 0x0002, // Mark typical code sequences as code
        AF_UNK = 0x0004, // Delete instructions with no xrefs
        AF_CODE = 0x0008, // Trace execution flow
        AF_PROC = 0x0010, // Create functions if call is present
        AF_USED = 0x0020, // Analyze and create all xrefs
        AF_FLIRT = 0x0040, // Use flirt signatures
        AF_PROCPTR = 0x0080, // Create function if data xref data->code32 exists
        AF_JFUNC = 0x0100, // Rename jump functions as j_...
        AF_NULLSUB = 0x0200, // Rename empty functions as nullsub_...
        AF_LVAR = 0x0400, // Create stack variables
        AF_TRACE = 0x0800, // Trace stack pointer
        AF_ASCII = 0x1000, // Create ascii string if data xref exists
        AF_IMMOFF = 0x2000, // Convert 32bit instruction operand to offset
        AF_DREFOFF = 0x4000, // Create offset if data xref to seg32 exists
        AF_FINAL = 0x8000, // Final pass of analysis
    }
}
