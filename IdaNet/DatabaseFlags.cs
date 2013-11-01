using System;

namespace IdaNet
{
    [Flags()]
    public enum DatabaseFlags : uint
    {
        DeleteUnpackedDatabase = 0x01,
        CompressDatabase = 0x02,
        CreateBackupFile = 0x04,
        TemporaryDatabase = 0x08,
    }
}
