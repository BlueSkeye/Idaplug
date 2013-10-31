using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// Those aliases should be copied in every source file because C# doesn't support
// source code inclusion.
#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using NodeIndex = System.UInt64;
using SegmentSelector = System.UInt64;
using SignedSize = System.Int64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using NodeIndex = System.UInt32;
using SegmentSelector = System.UInt32;
using SignedSize = System.Int32;
#endif

namespace IdaNet
{
    internal static class Constants
    {
#if __EA64__
        internal const string IdaDllName = "ida64.wll";
#else
        internal const string IdaDllName = "ida.wll";
#endif
    }
}
