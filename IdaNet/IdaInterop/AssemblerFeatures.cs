using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum AssemblerFeatures : uint
    {
        AS_OFFST = 0x00000001, // offsets are 'offset xxx' ?
        AS_COLON = 0x00000002, // create colons after data names ?
        AS_UDATA = 0x00000004, // can use '?' in data directives

        AS_2CHRE = 0x00000008, // double char constants are: "xy
        AS_NCHRE = 0x00000010, // char constants are: 'x
        AS_N2CHR = 0x00000020, // can't have 2 byte char consts

        // ASCII directives:
        AS_1TEXT = 0x00000040, //   1 text per line, no bytes
        AS_NHIAS = 0x00000080, //   no characters with high bit
        AS_NCMAS = 0x00000100, //   no commas in ascii directives

        AS_HEXFM = 0x00000E00, // format of hex numbers:
        ASH_HEXF0 = 0x00000000, //   34h
        ASH_HEXF1 = 0x00000200, //   h'34
        ASH_HEXF2 = 0x00000400, //   34
        ASH_HEXF3 = 0x00000600, //   0x34
        ASH_HEXF4 = 0x00000800, //   $34
        ASH_HEXF5 = 0x00000A00, //   <^R   > (radix)
        AS_DECFM = 0x00003000, // format of dec numbers:
        ASD_DECF0 = 0x00000000, //   34
        ASD_DECF1 = 0x00001000, //   #34
        ASD_DECF2 = 0x00002000, //   34.
        ASD_DECF3 = 0x00003000, //   .34
        AS_OCTFM = 0x0001C000, // format of octal numbers:
        ASO_OCTF0 = 0x00000000, //   123o
        ASO_OCTF1 = 0x00004000, //   0123
        ASO_OCTF2 = 0x00008000, //   123
        ASO_OCTF3 = 0x0000C000, //   @123
        ASO_OCTF4 = 0x00010000, //   o'123
        ASO_OCTF5 = 0x00014000, //   123q
        ASO_OCTF6 = 0x00018000, //   ~123
        AS_BINFM = 0x000E0000, // format of binary numbers:
        ASB_BINF0 = 0x00000000, //   010101b
        ASB_BINF1 = 0x00020000, //   ^B010101
        ASB_BINF2 = 0x00040000, //   %010101
        ASB_BINF3 = 0x00060000, //   0b1010101
        ASB_BINF4 = 0x00080000, //   b'1010101
        ASB_BINF5 = 0x000A0000, //   b'1010101'

        AS_UNEQU = 0x00100000, // replace undefined data items with EQU (for ANTA's A80)
        AS_ONEDUP = 0x00200000, // One array definition per line
        AS_NOXRF = 0x00400000, // Disable xrefs during the output file generation
        AS_XTRNTYPE = 0x00800000, // Assembler understands type of extrn symbols as ":type" suffix
        AS_RELSUP = 0x01000000, // Checkarg: 'and','or','xor' operations with addresses are possible
        AS_LALIGN = 0x02000000, // Labels at "align" keyword are supported.
        AS_NOCODECLN = 0x04000000, // don't create colons after code names
        AS_NOTAB = 0x08000000, // Disable tabulation symbols during the output file generation
        AS_NOSPACE = 0x10000000, // No spaces in expressions
        AS_ALIGN2 = 0x20000000, // .align directive expects an exponent rather than a power of 2 (.align 5 means to align at 32byte boundary)
        AS_ASCIIC = 0x40000000, // ascii directive accepts C-like escape sequences (\n,\x01 and similar)
        AS_ASCIIZ = 0x80000000, // ascii directive inserts implicit zero byte at the end
    }
}
