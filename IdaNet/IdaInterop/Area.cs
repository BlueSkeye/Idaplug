using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

using IdaNet.CppCompatibility;

namespace IdaNet.IdaInterop
{
    /// <summary>This class </summary>
    internal class Area : IComparable<Area>, IMarshalable
    {
        #region CONSTRUCTORS
        internal Area()
        {
            return;
        }

        internal Area(AreaControlBlock owner, IntPtr native)
        {
            this._owner = owner;
            NativePointer = native;
            return;
        }

        internal Area(AreaControlBlock owner, IntPtr native, EffectiveAddress startEA, EffectiveAddress endEA)
            : this(owner, native)
        {
            return;
        }

        internal Area(EffectiveAddress startEA, EffectiveAddress endEA)
        {
            StartEA = startEA;
            EndEA = endEA;
            return;
        }
        #endregion

        #region PROPERTIES
        internal EffectiveAddress EndEA
        {
            get { return (EffectiveAddress)Marshal.ReadInt64(NativePointer, 4); }
            set { Marshal.WriteInt64(NativePointer, 4, (long)value); }
        }

        internal bool IsEmpty
        {
            get { return StartEA >= EndEA; }
        }

        internal IntPtr NativePointer { get; private set; }

        /// <summary>Get or set the regular comment for this area. Setting the comment
        /// to a null or empty string will delete the comment.</summary>
        internal string RegularComment
        {
            get { return GetComment(false); }
            set { SetComment(value ?? "", false); }
        }

        /// <summary>Get or set the repeatable comment for this area. Setting the comment
        /// to a null or empty string will delete the comment.</summary>
        internal string RepeatableComment
        {
            get { return GetComment(true); }
            set { SetComment(value ?? "", true); }
        }

        internal MemoryChunkSize Size
        {
            get { return EndEA - StartEA; }
        }

        internal EffectiveAddress StartEA
        {
            get { return (EffectiveAddress)Marshal.ReadInt64(NativePointer, 0); }
            set { Marshal.WriteInt64(NativePointer, 0, (long)value); }
        }
        #endregion

        #region METHODS
        internal void Clear()
        {
            StartEA = 0;
            EndEA = 0;
            return;
        }

        public int CompareTo(Area other)
        {
            return (StartEA > other.StartEA)
                ? 1
                : StartEA < other.StartEA
                    ? -1
                    : 0;
        }

        internal bool Contains(EffectiveAddress ea)
        {
            return (StartEA <= ea) && (EndEA > ea);
        }

        internal void Extend(EffectiveAddress ea)
        {
            if (StartEA > ea) { StartEA = ea; }
            if (EndEA < ea) { EndEA = ea; }
            return;
        }

        /// <summary>Get area regular or reapatable comment.</summary>
        /// <param name="repeatable">true if the repeatble comment is requested.</param>
        /// <returns></returns>
        private string GetComment(bool repeatable)
        {
            IntPtr nativeComment = IntPtr.Zero;

            try
            {
                nativeComment = areacb_t_get_area_cmt(_owner.NativePointer, this.NativePointer, repeatable);
                return Marshal.PtrToStringAnsi(nativeComment);
            }
            finally { if (IntPtr.Zero != nativeComment) { MemoryManagement.qfree(nativeComment); } }
        }

        internal void Intersect(Area other)
        {
            if (StartEA < other.StartEA) { StartEA = other.StartEA; }
            if (EndEA > other.EndEA) { EndEA = other.EndEA; }
            if (EndEA < StartEA) { EndEA = StartEA; }
            return;
        }
        
        internal bool Overlaps(Area other)
        {
            return (other.StartEA < EndEA) && (StartEA < other.EndEA);
        }

        /// <summary>Set area regular or reapatable comment.</summary>
        /// <param name="comment">comment string, may be multiline (with '\n') maximal size is 4096 bytes.</param>
        /// <param name="repeatable">true if the repeatble comment is to be set.</param>
        /// <returns></returns>
        private void SetComment(string comment, bool repeatable)
        {
            IntPtr nativeComment = IntPtr.Zero;

            try
            {
                nativeComment = Marshal.StringToCoTaskMemAnsi(comment);
                areacb_t_set_area_cmt(_owner.NativePointer, this.NativePointer, nativeComment, repeatable);
            }
            finally { if (IntPtr.Zero != nativeComment) { Marshal.FreeCoTaskMem(nativeComment); } }
        }

        // Update segment information. You must call this function after modification
        // of segment characteristics. Note that not all fields of segment structure
        // may be modified directly, there are special functions to modify some fields.
        // returns: 1-ok, 0-failure
        internal bool Update()
        {
            return _owner.Update(this);
        }
        #endregion

        #region NATIVE MEMBERS
        // 0x00 EffectiveAddress startEA;
        // 0x04 EffectiveAddress endEA;                  // endEA excluded
        #endregion

        public static bool operator ==(Area thisOne, Area other)
        {
            return thisOne.compare(other) == 0;
        }

        public static bool operator !=(Area thisOne, Area other)
        {
            return thisOne.compare(other) != 0;
        }
        
        public static bool operator > (Area thisOne, Area other)
        {
            return thisOne.compare(other) > 0;
        }

        public static bool operator <(Area thisOne, Area other)
        {
            return thisOne.compare(other) < 0;
        }

        private int compare(Area other)
        {
            return this.StartEA > other.StartEA ? 1 : this.StartEA < other.StartEA ? -1 : 0;
        }

        internal int print(byte[] buf, int bufsize)
        {
            return area_t_print(NativePointer, buf, bufsize);
        }

        #region MANAGED MEBERS
        protected AreaControlBlock _owner;
        #endregion

        #region IDA NATIVE FUNCTIONS
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr areacb_t_get_area_cmt(IntPtr nativeAreaControlBlock, IntPtr nativeArea, bool repeatable);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool areacb_t_set_area_cmt(IntPtr nativeAreaControlBlock, IntPtr nativeArea, IntPtr comment, bool repeatable);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int area_t_print(IntPtr /* area_t* */ cb, byte[] buf, int bufsize);
        #endregion

        internal class Marshaler : ICustomMarshaler
        {
            #region CONSTRUCTORS
            private Marshaler()
            {
                return;
            }
            #endregion

            #region PROPERTIES
            public static ICustomMarshaler Singleton
            {
                get { return _singleton; }
            }
            #endregion

            #region METHODS
            public static ICustomMarshaler GetInstance()
            {
                return Singleton;
            }

            public void CleanUpManagedData(object ManagedObj)
            {
                throw new NotImplementedException();
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                throw new NotImplementedException();
            }

            public int GetNativeDataSize()
            {
                throw new NotImplementedException();
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                throw new NotImplementedException();
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region FIELDS
            private static readonly Marshaler _singleton = new Marshaler();
            #endregion
        }
    }
}
