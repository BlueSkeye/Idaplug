using System;

namespace IdaNet.IdaInterop
{
    public enum SegmentCombination : byte
    {
        scPriv = 0,    // Private. Do not combine with any other program segment.
        scGroup = 1,    // Segment group
        scPub = 2,    // Public. Combine by appending at an offset that meets the alignment requirement.
        scPub2 = 4,    // As defined by Microsoft, same as C=2 (public).
        scStack = 5,    // Stack. Combine as for C=2. This combine type forces byte alignment.
        scCommon = 6,    // Common. Combine by overlay using maximum size.
        scPub3 = 7,    // As defined by Microsoft, same as C=2 (public).
    }
}
