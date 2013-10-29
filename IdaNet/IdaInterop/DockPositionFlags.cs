using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum DockPositionFlags
    {
        DP_LEFT = 0x0001,
        DP_TOP = 0x0002,
        DP_RIGHT = 0x0004,
        DP_BOTTOM = 0x0008,
        DP_INSIDE = 0x0010,
        DP_BEFORE = 0x0020,
        DP_RAW = 0x0040,
        DP_FLOATING = 0x0080,
    }
}
