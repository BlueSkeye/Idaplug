using System;

namespace IdaNet
{
    public enum KnownFileType
    {
        EXE_old, // MS DOS EXE File
        COM_old,            // MS DOS COM File
        BIN,                // Binary File
        DRV,                // MS DOS Driver
        WIN,                // New Executable (NE)
        HEX,                // Intel Hex Object File
        MEX,                // MOS Technology Hex Object File
        LX,                 // Linear Executable (LX)
        LE,                 // Linear Executable (LE)
        NLM,                // Netware Loadable Module (NLM)
        COFF,               // Common Object File Format (COFF)
        PE,                 // Portable Executable (PE)
        OMF,                // Object Module Format
        SREC,               // R-records
        ZIP,                // ZIP file (this file is never loaded to IDA database)
        OMFLIB,             // Library of OMF Modules
        AR,                 // ar library
        LOADER,             // file is loaded using LOADER DLL
        ELF,                // Executable and Linkable Format (ELF)
        W32RUN,             // Watcom DOS32 Extender (W32RUN)
        AOUT,               // Linux a.out (AOUT)
        PRC,                // PalmPilot program file
        EXE,                // MS DOS EXE File
        COM,                // MS DOS COM File
        AIXAR,              // AIX ar library
        MACHO,              // Max OS X
    }
}
