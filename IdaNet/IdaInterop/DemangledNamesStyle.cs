using System;

namespace IdaNet.IdaInterop
{
    internal enum DemangledNamesStyle : byte
    {
        DEMNAM_CMNT = 0, //   comments
        DEMNAM_NAME = 1, //   regular names
        DEMNAM_NONE = 2, //   don't display
        DEMNAM_GCC3 = 4, // assume gcc3 names (valid for gnu compiler)
        DEMNAM_MASK = 3, // mask for name form
    }
}
