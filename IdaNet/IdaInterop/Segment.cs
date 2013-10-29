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
    /// <summary>This file contains functions that deal with program segmentation.
    /// IDA requires that all program addresses belong to segments (each address
    /// must belong to exactly one segment). Situation when an address doesn't
    /// belong to any segment is allowed as temporary situation only when the user
    /// changes program segmentation. Bytes outside a segment can't be converted to
    /// instructions, have names, comments, etc. Each segment has its start address,
    /// ending address and represents a contiguous range of addresses. There might
    /// be unused holes between segments. Each segment has its unique segment selector.
    /// This selector is used to distinguish the segment from other segments. For 16-bit
    /// programs the selector is equal to the segment base paragraph. For 32-bit programs
    /// there is special array to translate the selectors to the segment base paragraphs.
    /// A selector is a 32/64 bit value. The segment base paragraph determines the offsets
    /// in the segment. If the start address of the segment == (base &lt;&lt; 4) then the
    /// first offset in the segment will be 0. The start address should be higher or equal
    /// to (base &lt;&lt; 4). We will call the offsets in the segment 'virtual addresses'.
    /// So, virtual address of the first byte of segment is
    /// 
    /// (start address of segment - segment base linear address)
    /// 
    /// For IBM PC, virtual address corresponds to offset part of address. For other
    /// processors (Z80, for example), virtual address corresponds to Z80 addresses and
    /// linear addresses are used only internally. For MS Windows programs the segment base
    /// paragraph is 0 and therefore the segment virtual addresses are equal to linear
    /// addresses.</summary>
    internal class Segment : Area
    {
        #region CONSTRUCTORS
        private Segment(IntPtr native)
            : base(AreaControlBlock.Segments, native)
        {
            return;
        }
        #endregion

        #region PROPERTIES
        internal SegmentAlignement Alignement
        {
            get { return (SegmentAlignement)Marshal.ReadByte(NativePointer, 20); }
            set { Marshal.WriteByte(NativePointer, 20, (byte)value); }
        }

        internal uint BackgroundColor
        {
            get { return (uint)Marshal.ReadInt32(NativePointer, 95); }
            set { Marshal.WriteInt32(NativePointer, 95, (int)value); }
        }

        internal SegmentBitness Bitness
        {
            get { return (SegmentBitness)Marshal.ReadByte(NativePointer, 23); }
            set { Marshal.WriteByte(NativePointer, 23, (byte)value); }
        }

        internal int BitsCount
        {
            get { return (1 << ((int)Bitness + 4)); }
        }

        internal int BytesCount
        {
            get { return (BitsCount / 8); }
        }

        internal SegmentFlags Flags
        {
            get { return (SegmentFlags)Marshal.ReadInt16(NativePointer, 24); }
            set { Marshal.WriteInt16(NativePointer, 24, (byte)value); }
        }

        internal SegmentCombination Combination
        {
            get { return (SegmentCombination)Marshal.ReadByte(NativePointer, 21); }
            set { Marshal.WriteByte(NativePointer, 21, (byte)value); }
        }

        internal bool DebugSegment
        {
            get { return 0 != (Flags & SegmentFlags.SFL_DEBUG); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_DEBUG)
                    : (Flags & ~SegmentFlags.SFL_DEBUG);
            }
        }

        // Ephemeral segments are not analyzed automatically
        // (no flirt, no functions unless required, etc)
        // Most likely these segments will be destroyed at the end of the
        // debugging session uness the user changes their status.
        internal bool Ephemeral
        {
            get { return 0 != (Flags & (SegmentFlags.SFL_LOADER | SegmentFlags.SFL_DEBUG)); }
        }

        internal bool FinallyVisible
        {
            get { return (0 != (DatabaseInfo.Current.CommentFlags & CommentFlags.SW_SHHID_SEGM)) || this.Visible; }
        }

        internal bool Hidden
        {
            get { return (0 != (Flags & SegmentFlags.SFL_HIDDEN)); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_HIDDEN)
                    : (Flags & ~SegmentFlags.SFL_HIDDEN);
            }
        }

        internal bool HideType
        {
            get { return 0 != (Flags & SegmentFlags.SFL_HIDETYPE); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_HIDETYPE)
                    : (Flags & ~SegmentFlags.SFL_HIDETYPE);
            }
        }

        internal bool LoaderSegment
        {
            get { return 0 != (Flags & SegmentFlags.SFL_LOADER); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_LOADER)
                    : (Flags & ~SegmentFlags.SFL_LOADER);
            }
        }

        internal string Name
        {
            get { return GetName(get_segm_name); }
            //{
            //    int nativeBufferLength = 16;
            //    IntPtr nativeBuffer = Marshal.AllocCoTaskMem(nativeBufferLength);
            //    SignedSize trueNameLength = get_segm_name(NativePointer, nativeBuffer, nativeBufferLength);

            //    return (-1 == trueNameLength)
            //        ? ""
            //        : Marshal.PtrToStringAnsi(nativeBuffer, (int)trueNameLength);
            //}
            set { throw new NotImplementedException(); }
        }

        internal bool OrgBasePresent
        {
            get { return 0 != (Flags & SegmentFlags.SFL_OBOK); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_OBOK)
                    : (Flags & ~SegmentFlags.SFL_OBOK);
            }
        }

        internal bool OriginPresent
        {
            get { return 0 != (Flags & SegmentFlags.SFL_COMORG); }
            set
            {
                Flags = (value)
                    ? (Flags | SegmentFlags.SFL_COMORG)
                    : (Flags & ~SegmentFlags.SFL_COMORG);
            }
        }

        internal SegmentPermission Permission
        {
            get { return (SegmentPermission)Marshal.ReadByte(NativePointer, 22); }
            set { Marshal.WriteByte(NativePointer, 22, (byte)value); }
        }

        /// <summary>segment selector - should be unique. You can't change this field
        /// after creating the segment. Exception: 16bit OMF files may have several
        /// segments with the same selector, but this is not good (no way to denote a
        /// segment exactly) so it should be fixed in the future.</summary>
        internal SegmentSelector Selector
        {
            get { return (SegmentSelector)Marshal.ReadInt32(NativePointer, 26); }
            set { Marshal.WriteInt32(NativePointer, 26, (int)value); }
        }

        // Type of the segment. The kernel treats differentsegment types differently.
        // Segments marked with '*' contain no instructions or data and are not declared
        // as 'segments' in the disassembly.
        internal SegmentType Type
        {
            get { return (SegmentType)Marshal.ReadByte(NativePointer, 94); }
            set { Marshal.WriteByte(NativePointer, 94, (byte)value); }
        }

        internal string TrueName
        {
            get { return GetName(get_true_segm_name); }
            set { throw new NotImplementedException(); }
        }

        internal bool Use32
        {
            get { return SegmentBitness.Bitness32 <= Bitness; }
        }

        internal bool Use64
        {
            get { return SegmentBitness.Bitness64 == Bitness; }
        }

        internal bool Visible
        {
            get { return !Hidden; }
            set { set_visible_segm(NativePointer, value); }
        }
        #endregion

        #region METHODS
        /// <summary>Used as a segment factory by the segment related area control block.</summary>
        /// <param name="native"></param>
        /// <returns></returns>
        internal static Segment Create(IntPtr native)
        {
            return new Segment(native);
        }

        /// <summary>Enumerate all known segments.</summary>
        /// <returns></returns>
        internal static IEnumerable<Segment> EnumerateSegments()
        {
            uint segmentsCount = AreaControlBlock.Segments.Count;

            for (uint index = 0; index < segmentsCount; index++)
            {
                yield return (Segment)AreaControlBlock.Segments[index];
            }
            yield break;
        }

        /// <summary>Can either retrieve the name or the true name depending on the
        /// retriever delegate value.</summary>
        /// <param name="retriever"></param>
        /// <returns></returns>
        private string GetName(NativeNameRetrievedDelegate retriever)
        {
            int nativeBufferLength = 16;
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(nativeBufferLength);
            SignedSize trueNameLength = retriever(NativePointer, nativeBuffer, nativeBufferLength);

            return (-1 == trueNameLength)
                ? ""
                : Marshal.PtrToStringAnsi(nativeBuffer, (int)trueNameLength);
        }

        /// <summary>Retrieve the segment instance that is hosting the given effective
        /// address.</summary>
        /// <param name="address"></param>
        /// <returns>The segment instance or null if no such segment exist.</returns>
        internal static Segment GetSegment(EffectiveAddress address)
        {
            return (Segment)AreaControlBlock.Segments.GetArea(address);
        }
        #endregion

        #region IDA NATIVE FUNCTIONS
        private delegate SignedSize NativeNameRetrievedDelegate(IntPtr nativePointer, IntPtr nativeBuffer, int nativeBufferLength);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern SignedSize get_segm_name(IntPtr nativeSegment, IntPtr buffer, int bufferSize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern SignedSize get_true_segm_name(IntPtr nativeSegment, IntPtr buffer, int bufferSize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern SignedSize set_visible_segm(IntPtr nativeSegment, bool visible);
        #endregion
    }
}
