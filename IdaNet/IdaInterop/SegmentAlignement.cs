using System;

namespace IdaNet.IdaInterop
{
    internal enum SegmentAlignement : byte
    {
        saAbs = 0, // Absolute segment.
        saRelByte = 1, // Relocatable, byte aligned.
        saRelWord = 2, // Relocatable, word (2-byte) aligned.
        saRelPara = 3, // Relocatable, paragraph (16-byte) aligned.
        saRelPage = 4, // Relocatable, aligned on 256-byte boundary
        saRelDble = 5, // Relocatable, aligned on a double word (4-byte) boundary.
        saRel4K = 6, // This value is used by the PharLap OMF for page (4K) alignment. It is not supported by LINK.
        saGroup = 7, // Segment group
        saRel32Bytes = 8, // 32 bytes
        saRel64Bytes = 9, // 64 bytes
        saRelQword = 10, // 8 bytes
        saRel128Bytes = 11, // 128 bytes
        saRel512Bytes = 12, // 512 bytes
        saRel1024Bytes = 13, // 1024 bytes
        saRel2048Bytes = 14, // 2048 bytes
    }
}
