using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using SegmentSelector = System.UInt64;
using SignedSize = System.Int64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using SegmentSelector = System.UInt32;
using SignedSize = System.Int32;
#endif

namespace IdaNet.IdaInterop
{
    internal class Structure
    {
        #region CONSTRUCTORS
        internal Structure(IntPtr native)
        {
            NativePointer = native;
            return;
        }
        #endregion

        #region PROPERTIES
        /* 0x00 - 0x00 */
        // tid_t id;             // name(), cmt, rptcmt

        internal EffectiveAddress Id
        {
            get { return GetIdFast(NativePointer); }
        }

        /* 0x04 - 0x08 */
        // size_t memqty;

        internal int Size
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x04, 0x08); }
        }

        /* 0x08 - 0x0C */
        // member_t *members;    // only defined members are kept there may be holes in the structure the displayer must show the holes too
        internal IntPtr Members
        {
            get { return MarshalingUtils.GetIntPtr(NativePointer, 0x08, 0x0C); }
        }

        /* 0x0C - 0x14 */
        // ushort age;
        internal ushort Age
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x0C, 0x14); }
        }
        
        /* 0x0E - 0x16 */
        // StructureProperties props;         // properties:
        internal StructureProperties Properties
        {
            get { return (StructureProperties)MarshalingUtils.GetUInt(NativePointer, 0x0E, 0x16); }
        }

        /* 0x12 - 0x1A */
        // int32 ordinal;        // corresponding local type ordinal number
        internal int Ordinal
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x12, 0x1A); }
        }

        internal IntPtr NativePointer { get; private set; }
        #endregion

        #region OTHER PROPERTIES
        internal bool FromTypeLib
        {
            get { return (0 != (Properties & StructureProperties.SF_TYPLIB)); }
        }

        internal bool HasUnion
        {
            get { return (0 != (Properties & StructureProperties.SF_HASUNI)); }
        }

        internal bool HasVariableSize
        {
            get { return (0 != (Properties & StructureProperties.SF_VAR)); }
        }

        internal bool IsChoosable
        {
            get { return (0 != (Properties & StructureProperties.SF_NOLIST)); }
        }

        internal bool IsHidden
        {
            get { return (0 != (Properties & StructureProperties.SF_HIDDEN)); }
        }

        internal bool IsUnion
        {
            get { return (0 != (Properties & StructureProperties.SF_UNION)); }
        }
        #endregion

        #region METHODS
        /// <summary>An optimized method that allow for retrieving a structure identifier
        /// without having to instanciate the corresponding mmanaged object.</summary>
        /// <param name="native"></param>
        /// <returns></returns>
        internal static EffectiveAddress GetIdFast(IntPtr native)
        {
            return MarshalingUtils.GetEffectiveAddress(native, 0x00, 0x00);
        }
        #endregion
    }
}
