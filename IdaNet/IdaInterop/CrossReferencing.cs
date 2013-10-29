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
    //      This file contains functions that deal with cross-references.
    //      There are 2 types of xrefs: CODE and DATA references.
    //      All xrefs are kept in the bTree except ordinary execution flow
    //      to the next instrction. Ordinary execution flow to
    //      the next instruction is kept in flags (see bytes.hpp)
    //      Cross-references are automatically sorted.
    // IDA handles the xrefs automatically and may delete an xref
    // added by the user if it does not contain the fl_USER bit
    internal static class CrossReferencing
    {
        // User specified xref This xref will not be deleted by IDA This bit
        // should be combined with the existing xref types (CodeCrossReferenceType & dref_t)
        internal const ushort XREF_USER = 0x20;
        internal const ushort XREF_TAIL = 0x40; // Reference to tail byte in extrn symbols
        internal const ushort XREF_BASE = 0x80; // Reference to the base part of an offset
        internal const ushort XREF_MASK = 0x1F; // Mask to get xref type
        // Reference is past item. This bit may be passed to add_dref() functions but it
        // won't be saved in the database. It will prevent the destruction of eventual
        // alignment directives.
        internal const ushort XREF_PASTEND = 0x100;

        #region IDA NATIVE FUNCTIONS
        // Create a code cross-reference
        //      from    - linear address of referencing instruction
        //      to      - linear address of referenced  instruction
        //      type    - cross-reference type
        // returns: success
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool add_cref(EffectiveAddress from, EffectiveAddress to, CodeCrossReferenceType type);

        // Create a data cross-reference
        //      from    - linear address of referencing instruction or data
        //      to      - linear address of referenced  data
        //      type    - cross-reference type
        // returns: success (may fail if user-defined xref exists from->to)
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool add_dref(EffectiveAddress from, EffectiveAddress to, DataCrossReferenceType type);

        // Delete a code cross-reference
        //      from    - linear address of referencing instruction
        //      to      - linear address of referenced  instruction
        //      expand  - 1: plan to delete the referenced instruction if it has
        //                   no more references.
        //                0:-don't delete the referenced instruction even if no
        //                   more cross-references point to it
        // returns: 1-the referenced instruction will     be deleted
        //          0-the referenced instruction will not be deleted
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int del_cref(EffectiveAddress from, EffectiveAddress to, bool expand);

        // Delete a data cross-reference
        //      from    - linear address of referencing instruction or data
        //      to      - linear address of referenced  data
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void del_dref(EffectiveAddress from, EffectiveAddress to);

        // Get first data referenced from the specified address
        //      from    - linear address of referencing instruction or data
        // returns: linear address of first (lowest) data referenced from
        //            the specified address.
        //            The 'lastXR' variable contains type of the reference
        //          EffectiveAddress.MaxValue if the specified instruction/data doesn't reference
        //            to anything.
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_dref_from(EffectiveAddress from);


        // Get next data referenced from the specified address
        //      from    - linear address of referencing instruction or data
        //      current - linear address of current referenced data
        //                This value is returned by get_first_dref_from() or
        //                previous call to get_next_dref_from() functions.
        // returns: linear address of next data or EffectiveAddress.MaxValue
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_dref_from(EffectiveAddress from,EffectiveAddress current);


        // Get address of instruction/data referencing to the specified data
        //      to      - linear address of referencing instruction or data
        // returns: EffectiveAddress.MaxValue if nobody refers to the specified data
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_dref_to(EffectiveAddress to);


        // Get address of instruction/data referencing to the specified data
        //      to      - linear address of referencing instruction or data
        //      current - current linear address.
        //                This value is returned by get_first_dref_to() or
        //                previous call to get_next_dref_to() functions.
        // returns: EffectiveAddress.MaxValue if nobody refers to the specified data
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_dref_to(EffectiveAddress to,EffectiveAddress current);


        // Get first instruction referenced from the specified instruction
        // If the specified instruction passes execution to the next instruction
        // then the next instruction is returned. Otherwise the lowest referenced
        // address is returned (remember that xrefs are kept sorted!)
        //      from    - linear address of referencing instruction
        // returns: first referenced address.
        //          The 'lastXR' variable contains type of the reference
        // if the specified instruction doesn't reference to other instructions
        // then returns EffectiveAddress.MaxValue.
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_cref_from(EffectiveAddress from);


        // Get next instruction referenced from the specified instruction
        //      from    - linear address of referencing instruction
        //      current - linear address of current referenced instruction
        //                This value is returned by get_first_cref_from() or
        //                previous call to get_next_cref_from() functions.
        // returns: next referenced address or EffectiveAddress.MaxValue.
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_cref_from(EffectiveAddress from,EffectiveAddress current);


        // Get first instruction referencing to the specified instruction
        // If the specified instruction may be executed immediately after its
        // previous instruction
        // then the previous instruction is returned. Otherwise the lowest referencing
        // address is returned (remember that xrefs are kept sorted!)
        //      to      - linear address of referenced instruction
        // returns: linear address of the first referencing instruction or EffectiveAddress.MaxValue.
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_cref_to(EffectiveAddress to);


        // Get next instruction referencing to the specified instruction
        //      to      - linear address of referenced instruction
        //      current - linear address of current referenced instruction
        //                This value is returned by get_first_cref_to() or
        //                previous call to get_next_cref_to() functions.
        // returns: linear address of the next referencing instruction or EffectiveAddress.MaxValue.
        //          The 'lastXR' variable contains type of the reference
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_cref_to(EffectiveAddress from,EffectiveAddress current);

        // The following functions are similar to get_{first|next}_cref_{from|to}
        // functions. The only difference is that they don't take into account
        // ordinary flow of execution. Only jump and call xrefs are returned.
        // (fcref means "far code reference")
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_fcref_from(EffectiveAddress from);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_fcref_from(EffectiveAddress from, EffectiveAddress current);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_first_fcref_to  (EffectiveAddress to);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern EffectiveAddress get_next_fcref_to(EffectiveAddress to, EffectiveAddress current);

        // Helper functions. Should not be called directly!
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool xrefblk_t_first_from(IntPtr /* CrossReferencing.Block */ block, EffectiveAddress from, int flags);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool xrefblk_t_next_from(IntPtr /* CrossReferencing.Block */ block);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool xrefblk_t_first_to(IntPtr /* CrossReferencing.Block */ block, EffectiveAddress to, int flags);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool xrefblk_t_next_to(IntPtr /* CrossReferencing.Block */ block);

        // Create switch table from the switch information
        //      insn_ea - address of the 'indirect jump' instruction
        //      si      - switch information
        // Usually there is no need to call this function directly because the kernel
        // will call it for the result of ph.is_switch()
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal extern static void create_switch_table(EffectiveAddress insn_ea, IntPtr /* SwitchDefinitionEx */ si);

        // Create code xrefs for the switch table
        // This function creates xrefs from the indirect jump.
        //      insn_ea - address of the 'indirect jump' instruction
        //      si      - switch information
        // Usually there is no need to call this function directly because the kernel
        // will call it for switch tables
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal extern static void create_switch_xrefs(EffectiveAddress insn_ea, IntPtr /* SwitchDefinitionEx */si);

        // Get detailed information about the switch table cases
        //      insn_ea - address of the 'indirect jump' instruction
        //      si      - switch information
        //      casevec - vector of case values...
        //      targets - ...and corresponding target addresses
        // Returns: success
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal extern static bool calc_switch_cases(EffectiveAddress insn_ea,
            IntPtr /* SwitchDefinitionEx */ si,
            QVector<QVector<AddressDifference>> casevec,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QVector<EffectiveAddress>.Marshaler), MarshalCookie = "ea")]
            QVector<EffectiveAddress> targets);
        #endregion

        // The following structure and its four member functions
        // represent the easiest way to access cross-references from the given
        // address.
        // For example:
        //
        //      CrossReferencing.Block xb;
        //      for ( bool ok=xb.first_from(ea, XREF_ALL); ok; ok=xb.next_from() )
        //      {
        //         // xb.to - contains the referenced address
        //      }
        //
        // or:
        //
        //      CrossReferencing.Block xb;
        //      for ( bool ok=xb.first_to(ea, XREF_ALL); ok; ok=xb.next_to() )
        //      {
        //         // xb.from - contains the referencing address
        //      }
        //
        // First all code references will be returned, then all data references
        // If you need only code references, stop calling next() as soon as you get a dref
        // If you need only data references, pass XREF_DATA flag to first()
        // You may not modify contents CrossReferencing.Block structure! It is read only.
        // structure to enumerate all xrefs
        internal class Block
        {
            #region CONSTRUCTORS
            internal Block(IntPtr nativeBlock)
            {
                NativePointer = nativeBlock;
                return;
            }
            #endregion

            #region PROPERTIES
            internal IntPtr NativePointer { get; private set; }
            #endregion

            #region METHODS
            // The following functions return: 1-ok, 0-no (more) xrefs
            // They first return code references, then data references.
            // If you need only code references, you need to check 'iscode' after each call.
            // If you need only data references, use XREF_DATA bit.
            // flags may be any combination of the following bits:
            internal const int XREF_ALL = 0x00; // return all references
            internal const int XREF_FAR = 0x01; // don't return ordinary flow xrefs
            internal const int XREF_DATA = 0x02; // return data references only

            // get first reference from...
            internal bool FirstFrom(EffectiveAddress from, int flags)
            {
                return CrossReferencing.xrefblk_t_first_from(NativePointer, from, flags);
            }

            // get next reference from...
            internal bool NextFrom()
            {
                return CrossReferencing.xrefblk_t_next_from(NativePointer);
            }

            // get first reference to...
            internal bool FirstTo(EffectiveAddress to, int flags)
            {
                return CrossReferencing.xrefblk_t_first_to(NativePointer, to, flags);
            }

            // get next reference to....
            internal bool NextTo()
            {
                return CrossReferencing.xrefblk_t_next_to(NativePointer);
            }

            internal bool NextFrom(EffectiveAddress from, EffectiveAddress _to, int flags)
            {
                if (FirstFrom(from, flags))
                {
                    To = _to;
                    return NextFrom();
                }
                return false;
            }

            internal bool NextTo(EffectiveAddress _from, EffectiveAddress to, int flags)
            {
                if (FirstTo(to, flags))
                {
                    From = _from;
                    return NextTo();
                }
                return false;
            }
            #endregion

            #region IDA DEFINED FIELDS
            /* 0x00 - 0x00 */
            // EffectiveAddress from;            // the referencing address - filled by first_to(),next_to()
            internal EffectiveAddress From
            {
                get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x00, 0x00); }
                private set { MarshalingUtils.SetEffectiveAddress(NativePointer, 0x00, 0x00, value); }
            }

            /* 0x04 - 0x08 */
            // EffectiveAddress to;              // the referenced address - filled by first_from(), next_from()
            internal EffectiveAddress To
            {
                get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x08, 0x04); }
                private set { MarshalingUtils.SetEffectiveAddress(NativePointer, 0x08, 0x04, value); }
            }

            /* 0x08 - 0x10 */
            // uchar iscode;         // 1-is code reference; 0-is data reference
            internal bool IsCode
            {
                get { return (0 != MarshalingUtils.GetByte(NativePointer, 0x10, 0x08)); }
            }

            /* 0x09 - 0x11 */
            // uchar type;           // type of the last retured reference (CodeCrossReferenceType & DataCrossReferenceType)
            internal byte Type
            {
                get { return MarshalingUtils.GetByte(NativePointer, 0x11, 0x09); }
            }

            /* 0x0A - 0x12 */
            // uchar user;           // 1-is used defined xref, 0-defined by ida
            internal bool UserDefined
            {
                get { return (0 != MarshalingUtils.GetByte(NativePointer, 0x12, 0x0A)); }
            }

            internal static int NativeSize
            {
                get
                {
#if __EA64__
                    return 0x13;
#else
                    return 0x0B;
#endif
                }
            }
            #endregion
        }
    }
}
