using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using SegmentSelector = System.UInt64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using SegmentSelector = System.UInt32;
#endif

namespace IdaNet
{
    /// <summary>Interop definitions for invocation of IDA exported functions.</summary>
    internal static class IdaNatives
    {
        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern IntPtr build_loaders_list(/* linput_t* */ IntPtr li);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern void close_linput(IntPtr li);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern void free_loaders_list(/* load_info_t* */ IntPtr list);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool init_loader_options(/* linput_t* */ IntPtr li, /* const load_info_t* */ IntPtr loader);

        // This function is for pure unstructured binary files.
        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool load_binary_file(string filename, /* linput_t* */ IntPtr li,
            LoadFlags _neflags, int fileoff, EffectiveAddress basepara, EffectiveAddress binoff,
            uint nbytes);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool load_nonbinary_file(string filename, /* linput_t* */ IntPtr li,
            string systemDllDirectory, LoadFlags _neflags, /* load_info_t* */ IntPtr loader);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern IntPtr open_linput(string file, bool remote);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool save_database_ex(string outfile, DatabaseFlags flags,
            /* const snapshot_t* */ IntPtr root /* = NULL */, /* const snapshot_t* */ IntPtr attr /* = NULL*/);
    }
}
