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
    // set of address ranges
    internal class AreaSet
    {
        #region PROPERTIES
        internal IntPtr NativePointer { get; private set; }
        #endregion

        #region IDA NATIVE FIELDS
        /* 0x00 - 0x00 */
        // areavec_t bag;
        internal Vector<Area> bag { get; set; }

        /* 0x0C - 0x10 */
        // area_t *cache;
        internal Area cache { get; set; }

        /* 0x10 - 0x10 */
        internal static int NativeSize
        {
            get
            {
#if __EA64__
                return 0x18;
#else
                return 0x10;
#endif
            }
        }

        // private bool verify();
        #endregion

        // DEFINE_MEMORY_ALLOCATION_FUNCS()
        internal AreaSet(IntPtr nativeSet)
        {
            // cache = null;
            NativePointer = nativeSet;
            return;
        }

        internal AreaSet(Area area)
        {
            // cache = null;
            // if ( !area.IsEmpty ) { bag.push_back(area); }
            return;
        }

        internal AreaSet(AreaSet cloneFrom)
        {
            // cache = null;
            bag = cloneFrom.bag;
            return;
        }

        internal void Swap(AreaSet other)
        {
            // bag.swap(other.bag);
            return;
        }

        internal bool Add(Area area)
        {
            return areaset_t_add(NativePointer, area);
        }

        internal bool Add(EffectiveAddress start, EffectiveAddress end)
        {
            return Add(new Area(start, end));
        }
        
        internal bool Add(AreaSet set)
        {
            return areaset_t_add2(NativePointer, set.NativePointer);
        }

        internal bool Substract(EffectiveAddress ea)
        {
            return Substract(new Area(ea, ea + 1));
        }
        
        internal bool Substract(Area area)
        {
            return areaset_t_sub(NativePointer, area.NativePointer);
        }
        
        internal bool Substract(AreaSet set)
        {
            return areaset_t_sub2(NativePointer, set.NativePointer);
        }

        internal bool HasCommon(Area area, bool strict)
        {
            return areaset_t_has_common(NativePointer, area.NativePointer, strict);
        }

        internal int print(byte[]buf, int bufsize)
        {
            unsafe
            {
                fixed (byte* nativeBuffer = buf)
                {
                    return areaset_t_print(NativePointer, nativeBuffer, bufsize);
                }
            }
        }

        //internal MemoryChunkSize count(); // size in bytes

        internal Area GetArea(int idx)
        {
            return bag[idx];
        }

        internal Area LastArea()
        {
            return bag.back();
        }
        
        internal int nareas()
        {
            return bag.size();
        }

        internal bool empty()
        {
            return bag.empty();
        }

        internal void clear()
        {
            bag.clear();
            cache = null;
        }

        internal bool HasCommon(AreaSet set)
        {
            return areaset_t_has_common2(NativePointer, set.NativePointer);
        }

        internal bool Contains(EffectiveAddress ea)
        {
            return !empty() && (null != find_area(ea));
        }
        
        internal bool Contains(AreaSet set)
        {
            return areaset_t_contains(NativePointer, set.NativePointer);
        }

        internal Area find_area(EffectiveAddress ea)
        {
            return (Area)Area.Marshaler.GetInstance().MarshalNativeToManaged(areaset_t_find_area(NativePointer, ea));
        }

        internal bool Intersect(AreaSet set)
        {
            return areaset_t_intersect(NativePointer, set);
        }

        internal bool is_subset_of(AreaSet set)
        {
            // WARNING parameter was *this
            return set.Contains(this);
        }

        internal bool Equals(AreaSet aset)
        {
            return bag == aset.bag;
        }
        
        // TODO 
        //public bool operator==(const areaset_t &aset) const { return is_equal(aset); }
        //bool operator!=(const areaset_t &aset) const { return !is_equal(aset); }

        // TODO 
        //typedef areavec_t::iterator iterator;
        //typedef areavec_t::const_iterator const_iterator;
        //const_iterator begin(void) const { return bag.begin(); }
        //const_iterator end(void)   const { return bag.end(); }
        //iterator begin(void) { return bag.begin(); }
        //iterator end(void)   { return bag.end(); }
        //const_iterator lower_bound(EffectiveAddress ea) const;
        //const_iterator upper_bound(EffectiveAddress ea) const;
        //const area_t *cached_area(void) const { return cache; }
        //EffectiveAddress next_addr(EffectiveAddress ea) const { return areaset_t_next_addr(this, ea); };
        //EffectiveAddress prev_addr(EffectiveAddress ea) const { return areaset_t_prev_addr(this, ea); };
        //EffectiveAddress next_area(EffectiveAddress ea) const { return areaset_t_next_area(this, ea); };
        //EffectiveAddress prev_area(EffectiveAddress ea) const { return areaset_t_prev_area(this, ea); };
        //int move_chunk(EffectiveAddress from, EffectiveAddress to, MemoryChunkSize size);
        //int check_move_args(EffectiveAddress from, EffectiveAddress to, MemoryChunkSize size);

        //const_iterator _lower_bound(EffectiveAddress ea) const;
        //const_iterator _upper_bound(EffectiveAddress ea) const;

        #region IDA NATIVE FUNCTIONS
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_add([In] IntPtr nativeSet, [In] IntPtr area);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_add([In] IntPtr nativeSet,
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Area.Marshaler))] Area area);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_sub(IntPtr nativeSet, IntPtr area);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_add2(IntPtr nativeSet, IntPtr otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_sub2(IntPtr nativeSet, IntPtr otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_has_common(IntPtr nativeSet, IntPtr area, bool strict);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_has_common2(IntPtr nativeSet, IntPtr otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_contains(IntPtr nativeSet, IntPtr otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static unsafe extern int areaset_t_print(IntPtr nativeSet, byte* buf, int bufsize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_intersect([In] IntPtr nativeSet, [In] IntPtr otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool areaset_t_intersect([In] IntPtr nativeSet,
            [In,MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AreaSet.Marshaler))] AreaSet otherSet);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern IntPtr /*area_t */ areaset_t_find_area(IntPtr nativeSet, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern EffectiveAddress areaset_t_next_addr(IntPtr nativeSet, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern EffectiveAddress areaset_t_prev_addr(IntPtr nativeSet, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern EffectiveAddress areaset_t_next_area(IntPtr nativeSet, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern EffectiveAddress areaset_t_prev_area(IntPtr nativeSet, EffectiveAddress ea);
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
