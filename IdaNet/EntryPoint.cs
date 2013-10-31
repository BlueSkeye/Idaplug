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

namespace IdaNet
{
    public class EntryPoint : IDisposable
    {
        #region CONSTRUCTORS
        private EntryPoint()
        {
            return;
        }

        ~EntryPoint()
        {
            Dispose(false);
            return;
        }
        #endregion

        #region PROPERTIES
        /// <summary>Retrieve the effective address of this entry point.</summary>
        public EffectiveAddress At
        {
            get
            {
                lock (this) {
                    AssertNotDisposed();
                    return _at;
                }
            }
            private set { }
        }

        private int Index { get; set; }

        public string Name
        {
            get
            {
                lock(this) {
                    AssertNotDisposed();
                    return _name;
                }
            }
            set {
                lock(this) {
                    AssertNotDisposed();
                    if(!rename_entry(Ordinal, value)) { throw new ApplicationException(); }
                    _name = value;
                    return;
                }
            }
        }

        /// <summary>Retrieve the entry point ordinal value.</summary>
        public MemoryChunkSize Ordinal
        {
            get
            {
                lock(this) {
                    AssertNotDisposed();
                    return _ordinal;
                }
            }
            private set { }
        }

        /// <summary>Get the total number of entry points in the currently disassembled file.</summary>
        public static int TotalCount
        {
            get { return get_entry_qty(); }
        }
        #endregion

        #region METHODS
        private void AssertNotDisposed()
        {
            if (!_disposed) { return; }
            throw new ObjectDisposedException("EntryPoint");
        }

        public void Dispose()
        {
            Dispose(true);
            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                lock(this) {
                    if (!_disposed) {
                        GC.SuppressFinalize(this);
                        _disposed = true;
                    }
                }
            }
        }

        public static IEnumerable<EntryPoint> EnumerateEntryPoints()
        {
            for(int index = 0; index < TotalCount; index ++) { yield return GetEntryPoint(index); }
            yield break;
        }

        /// <summary>Retrieve an instance of this class that matches the one having the given index.
        /// </summary>
        /// <param name="index">Entry point index.</param>
        /// <returns>The EntryPoitn object.</returns>
        public static EntryPoint GetEntryPoint(int index)
        {
            lock(_entriesByIndex) {
                EntryPoint result;

                if(_entriesByIndex.TryGetValue(index, out result)) {
                    result.AssertNotDisposed();
                    return result;
                }
                result = new EntryPoint();
                result.Index = index;
                result.Initialize();
                _entriesByIndex.Add(index, result);
                return result;
            }
        }

        private void Initialize()
        {
            Ordinal = get_entry_ordinal(Index);
            int requiredLength = get_entry_name(Ordinal, IntPtr.Zero, 0);
            if (-1 == requiredLength) { throw new ApplicationException(); }
            IntPtr localBuffer = Marshal.AllocCoTaskMem(requiredLength + 1);
            try {
                requiredLength = get_entry_name(Ordinal, IntPtr.Zero, 0);
                if(-1 == requiredLength) { throw new ApplicationException(); }
                Name = Marshal.PtrToStringAnsi(localBuffer, requiredLength);
            } finally { Marshal.FreeCoTaskMem(localBuffer); }
            return;
        }
        #endregion

        #region FIELDS
        /// <summary>The entry point effective address.</summary>
        private EffectiveAddress _at;
        private bool _disposed;
        /// <summary>A dictionary of entry point objects matched by their index.</summary>
        private static Dictionary<int, EntryPoint> _entriesByIndex = new Dictionary<int, EntryPoint>();
        /// <summary>Entry point name.</summary>
        private string _name;
        /// <summary>Entry point ordinal value.</summary>
        private MemoryChunkSize _ordinal;
        #endregion

        #region IDA NATIVE FUNCTIONS
        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern bool add_entry(MemoryChunkSize ord, EffectiveAddress ea, string name, bool makecode);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern EffectiveAddress get_entry(MemoryChunkSize ordinal);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int get_entry_name(MemoryChunkSize ordinal, IntPtr buffer, uint bufferSize);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern MemoryChunkSize get_entry_ordinal(int index);

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern int get_entry_qty();

        [DllImport(Constants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool rename_entry(MemoryChunkSize ord, string name);
        #endregion
    }
}
