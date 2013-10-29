using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    public enum SegmentPermission : byte
    {
        Execute = 1, // Execute
        Write = 2, // Write
        Read = 4, // Read
    }
}
