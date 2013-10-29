using System;
using System.Collections.Generic;
using System.Text;

// Those aliases should be copied in every source file because C# doesn't support
// source code inclusion.
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
    // information abou the target compiler
    internal class CompilerInfo
    {
        internal CompilerInfo(IntPtr native)
        {
            NativePointer = native;
            return;
        }

        internal IntPtr NativePointer { get; private set; }

        #region NATIVE IDA FIELDS
        /* 0x00 */
        // byte id;            // compiler id (see typeinf.hpp, COMP_...)
        internal byte Id
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x00, 0x00); }
        }

        /* 0x01 */
        // byte cm;              // memory model and calling convention (typeinf.hpp, CM_...)
        internal byte MemoryModel
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x01, 0x01); }
        }

        /* 0x02 */
        // byte size_i;         // sizeof(int)
        internal byte SizeofInt
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x02, 0x02); }
        }

        /* 0x03 */
        // byte size_b;         // sizeof(bool)
        internal byte SizeofBool
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x03, 0x03); }
        }

        /* 0x04 */
        // byte size_e;         // sizeof(enum)
        internal byte SizeofEnum
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x04, 0x04); }
        }

        /* 0x05 */
        // byte defalign;       // default alignment for structures
        internal byte DefaultStructureAlignment
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x05, 0x05); }
        }

        /* 0x06 */
        //byte size_s;         // short
        internal byte SizeofShort
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x06, 0x06); }
        }

        /* 0x07 */
        // byte size_l;         // long
        internal byte SizeofLong
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x07, 0x07); }
        }

        /* 0x08 */
        // byte size_ll;        // longlong
        internal byte SizeofLongLong
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x08, 0x08); }
        }

        // NB: size_ldbl is stored separately!
        internal static int NativeSize
        {
            get { return 9; }
        }
        #endregion
    }
}
