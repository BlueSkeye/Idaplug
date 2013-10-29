using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum AssemblerSyntax
    {
        AS2_BRACE = 0x00000001,        // Use braces for all expressions
        AS2_STRINV = 0x00000002,        // For processors with bytes bigger than 8 bits: invert the meaning of inf.wide_high_byte_first for text strings
        AS2_BYTE1CHAR = 0x00000004,        // One symbol per processor byte Meaningful only for wide byte processors
        AS2_IDEALDSCR = 0x00000008,        // Description of struc/union is in the 'reverse' form (keyword before name) the same as in borland tasm ideal
        AS2_TERSESTR = 0x00000010,        // 'terse' structure initialization form NAME<fld,fld,...> is supported
        AS2_COLONSUF = 0x00000020,        // addresses may have ":xx" suffix this suffix must be ignored when extracting the address under the cursor
    }
}
