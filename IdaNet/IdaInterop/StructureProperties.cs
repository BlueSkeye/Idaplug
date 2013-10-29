using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum StructureProperties : uint
    {
        SF_VAR = 0x00000001, // is variable size structure (varstruct)? a variable size structure is one with the zero size last member If the last member is a varstruct, then the current structure is a varstruct too.
        SF_UNION = 0x00000002, // is a union? varunions are prohibited!
        SF_HASUNI = 0x00000004, // has members of type "union"?
        SF_NOLIST = 0x00000008, // don't include in the chooser list
        SF_TYPLIB = 0x00000010, // the structure comes from type library
        SF_HIDDEN = 0x00000020, // the structure is collapsed
        SF_FRAME = 0x00000040, // the structure is a function frame
    }
}
