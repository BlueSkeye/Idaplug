using System;
using System.Collections.Generic;
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
    internal class Member
    {
        internal Member(IntPtr native)
        {
            NativePointer = native;
            return;
        }

        #region PROPERTIES
        internal bool ByTil
        {
            get { return (0 != (Properties & MemberProperties.MF_BYTIL)); }
        }

        internal bool HasType
        {
            get { return (0 != (Properties & MemberProperties.MF_HASTI)); }
        }

        internal bool HasUnion
        {
            get { return (0 != (Properties & MemberProperties.MF_HASUNI)); }
        }

        internal IntPtr NativePointer { get; private set; }

        internal bool UniMem
        {
            get { return (0 != (Properties & MemberProperties.MF_UNIMEM)); }
        }

        private EffectiveAddress StartOffset
        {
            get { return UniMem ? _StartOffset : 0; }
        }

        /* 0x00 - 0x00 */
        // EffectiveAddress id;             // name(), cmt, rptcmt
        internal EffectiveAddress Id
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x00, 0x00); }
        }

        /* 0x04 - 0x08 */
        // EffectiveAddress soff;            // start offset (for unions - number of the member 0..n)
        private EffectiveAddress _StartOffset
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x08, 0x04); }
        }

        /* 0x08 - 0x10 */
        // EffectiveAddress eoff;            // end offset
        internal EffectiveAddress EndOffset
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x10, 0x08); }
        }
        
        /* 0x0C - 0x18 */
        // flags_t flag;         // type, representation, look strid(id)

        /* 0x10 - 0x1C */
        // uint32 props;         // properties:
        internal MemberProperties Properties
        {
            get { return (MemberProperties)MarshalingUtils.GetInt32(NativePointer, 0x1C, 0x10); }
        }
        #endregion

        #region METHODS
        internal IntPtr GetNative()
        {
            if (IntPtr.Zero == NativePointer) { throw new InvalidOperationException(); }
            return NativePointer;
        }
        #endregion
    }
}
