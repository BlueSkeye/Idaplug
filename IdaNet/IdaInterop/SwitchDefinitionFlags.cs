using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum SwitchDefinitionFlags : ushort
    {
        SWI_SPARSE = 0x01,    // sparse switch ( value table present ) otherwise lowcase present
        SWI_V32 = 0x02,    // 32-bit values in table
        SWI_J32 = 0x04,    // 32-bit jump offsets
        SWI_VSPLIT = 0x08,    // value table is split (only for 32-bit values)
        SWI_DEFAULT = 0x10,    // default case is present
        SWI_END_IN_TBL = 0x20,    // switchend in table (default entry)
        SWI_JMP_INV = 0x40,    // jumptable is inversed (last entry is for first entry in values table)
        SWI_SHIFT_MASK = 0x180,   // use formula (element*shift + elbase) to find jump targets
        
        SWI_ELBASE = 0x200, // elbase is present (if not and shift!=0, endof(jumpea) is used)
        SWI_JSIZE = 0x400, // jump offset expansion bit
        SWI_VSIZE = 0x800, // value table element size expansion bit

        SWI_SEPARATE = 0x1000,  // do not create an array of individual dwords
        SWI_SIGNED = 0x2000,  // jump table entries are signed
        SWI_CUSTOM = 0x4000,  // custom jump table - ph.create_switch_xrefs will be called to create code xrefs for the table. it must return 2. custom jump table must be created by the module
        SWI_EXTENDED = 0x8000,  // this is switch_info_ex_t
    }
}
