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
    // Each function consists of function chunks. At least one function chunk
    // must be present in the function definition - the function entry chunk.
    // Other chunks are called function tails. There may be several of them
    // for a function.
    // A function tail is a continuous range of addresses.
    // It can be used in the definition of one or more functions.
    // One function using the tail is singled out and called the tail owner.
    // This function is considered as 'possessing' the tail.
    // get_func() on a tail address will return the function possessing the tail.
    // You can enumerate the functions using the tail by using func_parent_iterator_t.
    // Each function chunk in the disassembly is represented as an "area" (a range
    // of addresses, see area.hpp for details) with characteristics.
    // A function entry must start with an instruction (code) byte.
    internal class Function : Area
    {
        #region CONSTRUCTORS
        private Function(IntPtr native)
            : base(AreaControlBlock.Functions, native)
        {
            return;
        }
        #endregion

        #region PROPERTIES
        internal bool AnalyzedSP
        {
            get { return (0 != (Flags & FunctionFlags.FUNC_SP_READY)); }
        }

        internal bool DoesReturn
        {
            get { return (0 == (Flags & FunctionFlags.FUNC_NORET)); }
        }

        /* 0x08 - 0x10 */
        // FunctionFlags flags;
        internal FunctionFlags Flags
        {
            get { return (FunctionFlags)MarshalingUtils.GetUShort(NativePointer, 0x10, 0x08); }
        }

        internal bool IsFar
        {
            get { return (0 != (Flags & FunctionFlags.FUNC_FAR)); }
        }

  //union
  //{
  //  struct              // attributes of a function entry chunk
  //  {
  //    //
  //    // Stack frame of the function. It is represented as a structure:
  //    //
  //    //    +----------------------------------------+
  //    //    | function arguments                     |
  //    //    +----------------------------------------+
  //    //    | return address (isn't stored in func_t)|
  //    //    +----------------------------------------+
  //    //    | saved registers (SI,DI,etc)            |
  //    //    +----------------------------------------+ <- typical BP
  //    //    |                                        |  |
  //    //    |                                        |  | fpd
  //    //    |                                        |  |
  //    //    |                                        | <- real BP
  //    //    | local variables                        |
  //    //    |                                        |
  //    //    |                                        |
  //    //    +----------------------------------------+ <- SP
  //    //

        /* 0x0C - 0x18 */
        // MemoryChunkSize frame;        // netnode id of frame structure. use get_frame()
        internal MemoryChunkSize FrameNodeId
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x18, 0x0C); }
        }

        /* 0x10 - 0x20 */
        // MemoryChunkSize localVariablesSize;      // size of local variables part of frame in bytes this value is used as offset for BP (only if FUNC_FRAME is set)
        internal MemoryChunkSize LocalVariablesSize
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x20, 0x10); }
        }

        /* 0x14 - 0x28 */
        // ushort savedRegistersSize;       // size of saved registers in frame
        internal ushort SavedRegistersSize
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x28, 0x14); }
        }

        /* 0x16 - 0x2A */
        // MemoryChunkSize functionArgumentsSize;     // number of bytes purged from the stack upon returning
        internal MemoryChunkSize ArgumentsSize
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x2A, 0x16); }
        }

        /* 0x1A - 0x32 */
        // MemoryChunkSize fpd;         // frame pointer delta (usually 0, i.e. realBP==typicalBP) use update_fpd() to modify it
        internal MemoryChunkSize FramePointerDelta
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x32, 0x1A); }
        }

        /* 0x1E - 0x3A */
        // bgcolor_t color;     // user defined function color
        internal uint Color
        {
            get { return MarshalingUtils.GetUInt(NativePointer, 0x3A, 0x1E); }
        }
        
        // the following fields should not be accessed directly:
        // Stack change points
        /* 0x22 - 0x3E */
        //    ushort pntqty;       // number of SP change points
        /* 0x24 - 0x40 */
        //    stkpnt_t *points;    // array of SP change points use ...stkpnt...() functions to access this array

        // Register variables
        /* 0x28 - 0x48 */
        //    int regvarqty;       // number of register variables (-1-not read in yet) use find_regvar() to read register variables
        /* 0x2C - 0x4C */
        //    regvar_t *regvars;   // array of register variables this array is sorted by: startEA use ...regvar...() functions to access this array

        /* 0x30 - 0x54 */
        //    int llabelqty;       // number of local labels
        /* 0x34 - 0x58 */
        //    llabel_t *llabels;   // local labels this array is sorted by ea use ...llabel...() functions to access this array

        /* 0x38 - 0x60 */
        //    int regargqty;       // number of register arguments
        /* 0x3C - 0x64 */
        //    regarg_t *regargs;   // unsorted array of register arguments use ...regarg...() functions to access this array

        /* 0x40 - 0x6C */
        //    int tailqty;         // number of function tails
        /* 0x44 - 0x70 */
        //    area_t *tails;       // array of tails, sorted by ea use func_tail_iterator_t to access function tails
        /* 0x48 - 0x78 */
        //  };


        //  struct                 // attributes of a function tail chunk
        //  {
        /* 0x0C - 0x18 */
        // EffectiveAddress owner; // the address of the main function possessing this tail
        internal EffectiveAddress Owner
        {
            get { return MarshalingUtils.GetEffectiveAddress(NativePointer, 0x18, 0x0C); }
        }

        /* 0x20 - 0x30 */
        //    int refqty;          // number of referers
        /* 0x24 - 0x34 */
        //    EffectiveAddress *referers;      // array of referers (function start addresses) use func_parent_iterator_t to access the referers
        /* 0x28 - 0x3C */
        //  };
        //};
        #endregion

        #region METHODS
        // Add function frame.
        //      localVariablesSize  - size of function local variables
        //      savedRegistersSize  - size of saved registers
        //      functionArgumentsSize - size of function arguments area which will be purged upon return
        //                This parameter is used for __stdcall and __pascal calling conventions
        //                For other calling conventions please pass 0
        // returns: 1-ok, 0-failed (no function, frame already exists)
        internal bool AddFrame(MemoryChunkSize localVariablesSize, ushort savedRegistersSize,
            MemoryChunkSize functionArgumentsSize)
        {
            return add_frame(NativePointer, localVariablesSize, savedRegistersSize, functionArgumentsSize);
        }

        // Add automatical SP register change point
        // ea    - linear address where SP changes usually this is the end of the instruction which modifies the stack pointer (cmd.ea+cmd.size)
        // delta - difference between old and new values of SP
        internal bool AddStackChangePoint(EffectiveAddress ea, AddressDifference delta)
        {
            return add_auto_stkpnt2(NativePointer, ea, delta);
        }

        // Add user-defined SP register change point
        // ea    - linear address where SP changes usually this is the end of the instruction which modifies the stack pointer (cmd.ea+cmd.size)
        // delta - difference between old and new values of SP
        internal static bool AddUserStackChangePoint(EffectiveAddress ea, AddressDifference delta)
        {
            return add_user_stkpnt(ea, delta);
        }

        internal bool DeleteFrame()
        {
            return del_frame(NativePointer);
        }

        // Add user-defined SP register change point
        // ea    - linear address where SP changes usually this is the end of the instruction which modifies the stack pointer (cmd.ea+cmd.size)
        // delta - difference between old and new values of SP
        internal bool DeleteStackChangePoint(EffectiveAddress ea)
        {
            return del_stkpnt(NativePointer, ea);
        }

        /// <summary>Retriecve descriptor for function hosting the given effective address.
        /// The address doesn't need to be the very the one of the function entry point.</summary>
        /// <param name="address"></param>
        /// <returns>A <see cref="Function"/> instance or a null reference if the given
        /// address doesn't belong to any function.</returns>
        internal static Function GetFunction(EffectiveAddress address)
        {
            IntPtr nativeFunction = get_func(address);

            return (IntPtr.Zero == nativeFunction) ? null : new Function(nativeFunction);
        }

        internal IntPtr GetNative()
        {
            if (IntPtr.Zero == NativePointer) { throw new InvalidOperationException(); }
            return NativePointer;
        }
 
        internal bool SetFrame(MemoryChunkSize localVariablesSize, ushort savedRegistersSize,
            MemoryChunkSize functionArgumentsSize)
        {
            return add_frame(NativePointer, localVariablesSize, savedRegistersSize, functionArgumentsSize);
        }

        #endregion

        #region NATIVE IDA FUNCTIONS
        // Add function frame.
        //      pfn - pointer to function structure
        //      localVariablesSize  - size of function local variables
        //      savedRegistersSize  - size of saved registers
        //      functionArgumentsSize - size of function arguments area which will be purged upon return
        //                This parameter is used for __stdcall and __pascal calling conventions
        //                For other calling conventions please pass 0
        // returns: 1-ok, 0-failed (no function, frame already exists)
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool add_frame(IntPtr nativeFunction, MemoryChunkSize frsize,
            ushort frregs, MemoryChunkSize argsize);

        // Add automatical SP register change point
        //      pfn   - pointer to function. may be NULL.
        //      ea    - linear address where SP changes usually this is the end of the instruction which modifies the stack pointer (cmd.ea+cmd.size)
        //      delta - difference between old and new values of SP
        // returns: 1-ok, 0-failed
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool add_auto_stkpnt2(IntPtr nativeFunction, EffectiveAddress ea, AddressDifference delta);

        // Add user-defined SP register change point
        //      ea    - linear address where SP changes
        //      delta - difference between old and new values of SP
        // returns: 1-ok, 0-failed
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool add_user_stkpnt(EffectiveAddress ea, AddressDifference delta);

        // Delete a function frame
        //      pfn - pointer to function structure
        // returns: 1-ok, 0-failed
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool del_frame(IntPtr nativeFunction);

        // Delete SP register change point
        //      pfn   - pointer to function. may be NULL.
        //      ea    - linear address
        // returns: 1-ok, 0-failed
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool del_stkpnt(IntPtr nativeFunction, EffectiveAddress ea);

        // Get pointer to function frame
        //      pfn - pointer to function structure
        // returns: pointer to function frame
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr /* Structure */ get_frame(IntPtr nativeFunction);

        // Get pointer to function structure by address
        //      ea - any address in a function
        // Returns ptr to a function or NULL
        // This function returns a function entry chunk
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr get_func(EffectiveAddress ea);

        // Set size of function frame
        //      pfn - pointer to function structure
        //      frsize  - size of function local variables
        //      frregs  - size of saved registers
        //      argsize - size of function arguments
        // returns: 1-ok, 0-failed
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool set_frame_size(IntPtr nativeFunction, MemoryChunkSize frsize,
            ushort frregs, MemoryChunkSize argsize);
        #endregion
    }
}
