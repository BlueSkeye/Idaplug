using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

namespace IdaNet.IdaInterop
{
    /// <summary>Provides access to entry points. Exported functons are considered
    /// as entry point as well. IDA maintains list of entry points to the program.
    /// Each entry point
    /// - has an address
    /// - has a name
    /// - may have an ordinal number
    /// </summary>
    internal class EntryPoint
    {
        #region CONSTRUCTORS
        private EntryPoint()
        {
            return;
        }
        #endregion

        #region PROPERTIES
        internal EffectiveAddress Address { get; private set; }

        /// <summary>Get number of entry points</summary>
        internal static int Count
        {
            get { return get_entry_qty(); }
        }

        internal string Name { get; private set; }
        #endregion

        #region METHODS
        /// <summary>Add an entry point to the list of entry points.</summary>
        /// <param name="ordinal">ordinal number if ordinal number is equal to
        /// <paramref name="linearAddress"/> then ordinal is not used</param>
        /// <param name="linearAddress">Linear address.</param>
        /// <param name="name">name of entry point. If the specified location
        /// alreadyhas a name, the old name will be appended to the regular
        /// comment. If name == NULL, then the old name will be retained.</param>
        /// <param name="makeCode">should the kernel convert bytes at the entry
        /// point to instruction(s)</param>
        /// <returns>success (currently always true)</returns>
        internal static bool Add(MemoryChunkSize ordinal, EffectiveAddress linearAddress, string name, bool makeCode)
        {
            return add_entry(ordinal, linearAddress, name, makeCode);
        }

        internal static IEnumerable<EntryPoint> EnumerateEntryPoints()
        {
            int upperBound = Count;

            for (int index = 0; index < upperBound; index++)
            {
                MemoryChunkSize ordinal = get_entry_ordinal(index);
                EntryPoint result = new EntryPoint();

                result.Address = get_entry(ordinal);
                IntPtr nativeBuffer = IntPtr.Zero;

                try
                {
                    int requiredSize = get_entry_name(ordinal, IntPtr.Zero, 0);
                    if (-1 == requiredSize) { result.Name = string.Empty; }
                    else
                    {
                        nativeBuffer = Marshal.AllocCoTaskMem(requiredSize);
                        requiredSize = get_entry_name(ordinal, nativeBuffer, (uint)requiredSize);
                        if (0 <= requiredSize) { result.Name = Marshal.PtrToStringAnsi(nativeBuffer, requiredSize); }
                    }
                }
                finally { if (IntPtr.Zero != nativeBuffer) { Marshal.FreeCoTaskMem(nativeBuffer); } }

                yield return result;
            }
            yield break;
        }

        /// <summary>Rename entry point</summary>
        /// <param name="ordinal">ordinal number of the entry point</param>
        /// <param name="newName">name of entry point. If the specified location
        /// already has a name, the old name will be appended to a repeatable
        /// comment.</param>
        /// <returns>true on success, false on failure</returns>
        internal static bool Rename(MemoryChunkSize ordinal, string newName)
        {
            return rename_entry(ordinal, newName);
        }
        #endregion

        #region IDA NATIVE FUNCTIONS
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern bool add_entry(MemoryChunkSize ord, EffectiveAddress ea, string name, bool makecode);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern EffectiveAddress get_entry(MemoryChunkSize ordinal);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int get_entry_name(MemoryChunkSize ordinal, IntPtr buffer, uint bufferSize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern MemoryChunkSize get_entry_ordinal(int index);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern int get_entry_qty();

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool rename_entry(MemoryChunkSize ord, string name);
        #endregion
    }
}
