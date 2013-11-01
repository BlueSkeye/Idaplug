using System;

namespace IdaNet
{
    [Flags()]
    public enum LoadFlags : ushort
    {
        CreateSegments = 0x0001,
        LoadResources = 0x0002,
        RenameEntries = 0x0004,
        ManualLoad = 0x0008,
        FillSegmentGaps = 0x0010,
        CreateImportSegment = 0x0020,
        // Osboleted
        // DontAlignSegments = 0x0040,
        FirstLoadedFile = 0x0080,
        LoadAsCodeSgment = 0x0100,
        ReloadFile = 0x0200,
        AutocreateFlatGroup = 0x0400,
        CreateMiniDatabase = 0x0800,
        DisplayAdditionalLoaderOptionsDialog = 0x1000,
        LoadAllSegmentsSilently = 0x2000,
    }
}
