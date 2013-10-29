using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum ProcessorFeatures : uint
    {
        PR_SEGS = 0x000001,// has segment registers?
        PR_USE32 = 0x000002,// supports 32-bit addressing?
        PR_DEFSEG32 = 0x000004,// segments are 32-bit by default
        PR_RNAMESOK = 0x000008,// allow to user register names for location names
        //PR_DB2CSEG = 0x0010,// .byte directive in code segments should define even number of bytes (used by AVR processor)
        PR_ADJSEGS = 0x000020,// IDA may adjust segments moving their starting/ending addresses.
        PR_DEFNUM = 0x0000C0,// default number representation:
        PRN_HEX = 0x000000,// = hex
        PRN_OCT = 0x000040,// = octal
        PRN_DEC = 0x000080,// = decimal
        PRN_BIN = 0x0000C0,// = binary
        PR_WORD_INS = 0x000100,// instruction codes are grouped 2bytes in binrary line prefix
        PR_NOCHANGE = 0x000200,// The user can't change segments and code/data attributes (display only)
        PR_ASSEMBLE = 0x000400,// Module has a built-in assembler and understands IDP_ASSEMBLE
        PR_ALIGN = 0x000800,// All data items should be aligned properly
        PR_TYPEINFO = 0x001000,// the processor module supports type information callbacks ALL OF THEM SHOULD BE IMPLEMENTED! (the ones >= decorate_name)
        PR_USE64 = 0x002000,// supports 64-bit addressing?
        PR_SGROTHER = 0x004000,// the segment registers don't contain the segment selectors, something else
        PR_STACK_UP = 0x008000,// the stack grows up
        PR_BINMEM = 0x010000,// the processor module provides correctsegmentation for binary files(i.e. it creates additional segments) The kernel will not ask the user to specify the RAM/ROM sizes
        PR_SEGTRANS = 0x020000,// the processor module supports the segment translation feature (it means it calculates the code addresses using the codeSeg() function)
        PR_CHK_XREF = 0x040000,// don't allow near xrefs between segments with different bases
        PR_NO_SEGMOVE = 0x080000,// the processor module doesn't support move_segm() (i.e. the user can't move segments)
        PR_FULL_HIFXP = 0x100000,// REF_VHIGH operand value contains full operand not only the high bits. Meaningful if ph.high_fixup_bits
        PR_USE_ARG_TYPES = 0x200000,// use ph.use_arg_types callback
        PR_SCALE_STKVARS = 0x400000,// use ph.get_stkvar_scale callback
        PR_DELAYED = 0x800000,// has delayed jumps and calls
        PR_ALIGN_INSN = 0x1000000,// allow ida to create alignment instructions arbirtrarily. Since these instructions might lead to other wrong instructions and spoil the listing, IDA does not create them by default anymore
        PR_PURGING = 0x2000000,// there are calling conventions which may purge bytes from the stack
        PR_CNDINSNS = 0x4000000,// has conditional instructions
        PR_USE_TBYTE = 0x8000000,// BTMT_SPECFLT means _TBYTE type
        PR_DEFSEG64 = 0x10000000,// segments are 64-bit by default
    }
}
