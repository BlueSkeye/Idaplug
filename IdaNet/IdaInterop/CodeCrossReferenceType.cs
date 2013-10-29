using System;

namespace IdaNet.IdaInterop
{
    internal enum CodeCrossReferenceType
    {
        fl_U, // unknown -- for compatibility with old versions. Should not be used anymore.
        fl_CF = 16, // Call Far This xref creates a function at the referenced location
        fl_CN, // Call Near This xref creates a function at the referenced location
        fl_JF, // Jump Far
        fl_JN, // Jump Near
        fl_USobsolete, // User specified (obsolete)
        fl_F, // Ordinary flow: used to specify execution flow to the next instruction.
    }
}
