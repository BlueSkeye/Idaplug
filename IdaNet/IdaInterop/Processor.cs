using System;
using System.Collections.Generic;
using System.IO;
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
    //      This structure describes a processor module (IDP)
    //      An IDP file may have only one such structure called LPH.
    //      The kernel will copy it to 'ph' structure and use 'ph'.
    internal class Processor
    {
        #region CONSTRUCTORS
        internal Processor(IntPtr nativeProcessor)
        {
            NativePointer = nativeProcessor;
            return;
        }
        #endregion

        #region PROPERTIES
        internal IntPtr NativePointer { get; private set; }

        internal static Processor Current
        {
            get
            {
                if (null == _currentProcessor)
                {
                    _currentProcessor = new Processor(InteropConstants.GetExportedData(ExportedDataIdentifier.ExportedCurrentProcessor));
                    _currentProcessor.Dump();
                }
                return _currentProcessor;
            }
        }

        internal bool HasSegmentRegisters
        {
            get { return (0 != (Features & ProcessorFeatures.PR_SEGS)); }
        }

        internal bool Use32
        {
            get { return (0 != (Features & (ProcessorFeatures.PR_USE64|ProcessorFeatures.PR_USE32))); }
        }

        internal bool Use64
        {
            get { return (0 != (Features & ProcessorFeatures.PR_USE64)); }
        }

        internal bool HasTypeInfo
        {
            get { return (0 != (Features & ProcessorFeatures.PR_TYPEINFO)); }
        }

        internal bool StackGrowsUp
        {
            get { return (0 != (Features & ProcessorFeatures.PR_STACK_UP)); }
        }

        internal bool UseTriByte
        {
            get { return (0 != (Features & ProcessorFeatures.PR_USE_TBYTE)); }
        }

        internal int get_segm_bitness
        {
            
            get
            {
                return (0 != (Features & ProcessorFeatures.PR_DEFSEG64))
                    ? 2
                    : (0 != (Features & ProcessorFeatures.PR_DEFSEG32))
                        ? 1
                        : 0;
            }
        }

        // Number of 8bit bytes required to hold one byte of the target processor
        // for code segments
        internal int CodeSegmentByteSize
        {
            get { return (CodeSegmentBitsPerByte + 7) / 8; }
        }

        // for other segments
        internal int DataSegmentByteSize
        {
            get { return (DataSegmentBitsPerByte + 7) / 8; }
        }

        // Get the stack variable scaling factor
        // Useful for processors who refer to the stack with implicit scaling factor.
        // TMS320C55 for example: SP(#1) really refers to (SP+2)
        internal int get_stkvar_scale()
        {
            return (0 != (Features & ProcessorFeatures.PR_SCALE_STKVARS))
                ? Notify(idp_notify.get_stkvar_scale_factor)
                : 1;
        }
        
        internal bool IsCanonicInstruction(ushort itype)
        {
            return (itype >= FirstInstructionCode) && (itype < LastInstructionCode);
        }
        #endregion

        #region METHODS
        internal void Dump()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string processorName in EnumerateLongProcessorNames())
            {
                if (0 != builder.Length) { builder.Append(" | "); }
                builder.Append(processorName);
            }
            Interactivity.Message("Processor version {0} id {1} : {2}\r\n",
                Version, ProcessorId, builder.ToString());
            Interactivity.Message("Instructions from {0} to {1}\r\n",
                FirstInstructionCode, LastInstructionCode);
            return;
        }
        #endregion

        #region MANAGED FIELDS
        private InstructionDescriptor[] _instructionDescriptors;

        // Custom instruction codes defined by processor extension plugins must be greater than or equal to this:
        internal const uint CUSTOM_CMD_ITYPE = 0x8000;

        // use_regarg_type (see below) uses this bit in the return value to indicate that the register value has been spoiled
        internal const uint REG_SPOIL = 0x80000000;
        
        internal const int REAL_ERROR_FORMAT = -1; // not supported format for current .idp
        internal const int REAL_ERROR_RANGE = -2; // number too big (small) for store (mem NOT modifyed)
        internal const int REAL_ERROR_BADDATA = -3; // illegal real data for load (IEEE data not filled)

        internal const int OP_FP_BASED = 0x00000000; // operand is FP based
        internal const int OP_SP_BASED = 0x00000001; // operand is SP based
        internal const int OP_SP_ADD = 0x00000000; // operand value is added to the pointer
        internal const int OP_SP_SUB = 0x00000002; // operand value is substracted from the pointer
        #endregion

        #region IDA NATIVE FIELDS
        /* 0x00 - 0x00 */
        // int version; // Expected kernel version, should be IDP_INTERFACE_VERSION
        internal int Version
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x00, 0x00); }
        }
        
        /* 0x04 - 0x04 */
        // int id;                       // IDP id
        internal ProcessorId ProcessorId
        {
            get { return (ProcessorId)MarshalingUtils.GetInt32(NativePointer, 0x04, 0x04); }
        }
        
        /* 0x08 - 0x08 */
        // uint32 flag;                  // Processor features
        internal ProcessorFeatures Features
        {
            get { return (ProcessorFeatures)MarshalingUtils.GetInt32(NativePointer, 0x08, 0x08); }
        }
        
        /* 0x0C - 0x0C */
        // int cnbits; // Number of bits in a byte for code segments (usually 8) IDA supports values up to 32 bits
        internal int CodeSegmentBitsPerByte
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x0C, 0x0C); }
        }
        
        /* 0x10 - 0x10 */
        // int dnbits; // Number of bits in a byte for non-code segments (usually 8) IDA supports values up to 32 bits
        internal int DataSegmentBitsPerByte
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x10, 0x10); }
        }

        // IDP module may support several compatible processors. The following arrays define processor names:
        /* 0x14 - 0x14 */
        // const char **psnames; // short processor names (NULL terminated) Each name should be shorter than 9 characters
        private string[] _shortProcessorNames;

        internal IEnumerable<string> EnumerateShortProcessorNames()
        {
            if (null == _shortProcessorNames)
            {
                _shortProcessorNames = MarshalingUtils.GetStringsArray(NativePointer, 0x14, 0x14);
            }
            foreach (string processorName in _shortProcessorNames)
            {
                yield return processorName;
            }
            yield break;
        }

        /* 0x18 - 0x18 */
        // const char **plnames; // long processor names (NULL terminated) No restriction on name lengths.
        private string[] _longProcessorNames;

        internal IEnumerable<string> EnumerateLongProcessorNames()
        {
            if (null == _longProcessorNames)
            {
                _longProcessorNames = MarshalingUtils.GetStringsArray(NativePointer, 0x18, 0x18);
            }
            foreach (string processorName in _longProcessorNames)
            {
                yield return processorName;
            }
            yield break;
        }

        /* 0x1C - 0x1C */
        // asm_t **assemblers; // pointer to array of target assembler definitions. You may change this array when current processor is changed. (NULL terminated)
        private AssemblerDescriptor[] _assemblers;

        internal IEnumerable<AssemblerDescriptor> EnumerateAssemblers()
        {
            if (null == _assemblers)
            {
                IntPtr arrayBase = MarshalingUtils.GetIntPtr(NativePointer, 0x1C, 0x1C);
                IntPtr itemAddress;
                ushort itemOffset = 0;
                List<AssemblerDescriptor> assemblers = new List<AssemblerDescriptor>();

                do
                {
                    itemAddress = MarshalingUtils.GetIntPtr(arrayBase, itemOffset, itemOffset);
                    itemOffset += 4;
                    if (IntPtr.Zero != itemAddress) { assemblers.Add(new AssemblerDescriptor(itemAddress)); }
                } while (IntPtr.Zero != itemAddress);
                _assemblers = assemblers.ToArray();
            }
            foreach (AssemblerDescriptor descriptor in _assemblers)
            {
                yield return descriptor;
            }
            yield break;
        }

        // Subtypes in callback custom_fixup
        // WARN : Had definition of enum cust_fix here.

        // Callback function. IDP module can take appropriate actions when some events occurs in the kernel.
        // WARN : Had definition for enum idp_notify here.

        /* 0x20 - 0x20 */
        // int (__stdcall* notify)(idp_notify msgid, ...); // Various notifications for the idp
        internal delegate int NotifyDelegate(idp_notify what, params IntPtr[] args);
        private NotifyDelegate _notify;

        internal NotifyDelegate Notify
        {
            get
            {
                if (null == _notify)
                {
                    _notify = MarshalingUtils.GetFunctionPointer<NotifyDelegate>(NativePointer, 0x20, 0x20);
                }
                return _notify;
            }
        }

        // The following functions generate portions of the disassembled text.
        /* 0x24 - 0x24 */
        // void  (__stdcall* header)(void);                // function to produce start of disassembled text
        internal delegate void MakeHeaderDelegate();
        private MakeHeaderDelegate _makeHeader;

        // function to produce start of disassembled text
        internal void MakeHeader()
        {
            if (null == _makeHeader)
            {
                _makeHeader = MarshalingUtils.GetFunctionPointer<MakeHeaderDelegate>(NativePointer, 0x24, 0x24);
            }
            _makeHeader();
            return;
        }
        
        /* 0x28 - 0x28 */
        // void  (__stdcall* footer)(void);                // function to produce end of disassembled text
        internal delegate void MakeFooterDelegate();
        private MakeFooterDelegate _makeFooter;

        // function to produce end of disassembled text
        internal void MakeFooter()
        {
            if (null == _makeFooter)
            {
                _makeFooter = MarshalingUtils.GetFunctionPointer<MakeFooterDelegate>(NativePointer, 0x28, 0x28);
            }
            _makeFooter();
            return;
        }

        /* 0x2C - 0x2C */
        // void  (__stdcall* segstart)(EffectiveAddress ea);          // function to produce start of segment
        internal delegate void MakeSegmentStartDelegate(EffectiveAddress ea);
        private MakeSegmentStartDelegate _makeSegmentStart;

        // function to produce start of segment
        internal void MakeSegmentStart(EffectiveAddress ea)
        {
            if (null == _makeSegmentStart)
            {
                _makeSegmentStart = MarshalingUtils.GetFunctionPointer<MakeSegmentStartDelegate>(NativePointer, 0x2C, 0x2C);
            }
            _makeSegmentStart(ea);
            return;
        }

        /* 0x30 - 0x30 */
        // void  (__stdcall* segend)  (EffectiveAddress ea);          // function to produce end of segment
        internal delegate void MakeSegmentEndDelegate(EffectiveAddress ea);
        private MakeSegmentEndDelegate _makeSegmentEnd;

        // function to produce end of segment
        internal void MakeSegmentEnd(EffectiveAddress ea)
        {
            if (null == _makeSegmentEnd)
            {
                _makeSegmentEnd = MarshalingUtils.GetFunctionPointer<MakeSegmentEndDelegate>(NativePointer, 0x30, 0x30);
            }
            _makeSegmentEnd(ea);
            return;
        }

        /* 0x34 - 0x34 */
        // void  (__stdcall* assumes) (EffectiveAddress ea);          // function to produce assume directives when segment register value changes if your processor has no segment registers, you may define it as NULL
        internal delegate void MakeAssumesDelegate(EffectiveAddress ea);
        private MakeAssumesDelegate _makeAssumes;

        internal void MakeAssumes(EffectiveAddress ea)
        {
            if (null == _makeAssumes)
            {
                _makeAssumes = MarshalingUtils.GetFunctionPointer<MakeAssumesDelegate>(NativePointer, 0x34, 0x34);
            }
            _makeAssumes(ea);
            return;
        }

        // Analyze one instruction and fill 'cmd' structure.
        // cmd.ea contains address of instruction to analyze.
        // Return length of the instruction in bytes, 0 if instruction can't be decoded.
        // This function shouldn't change the database, flags or anything else.
        // All these actions should be performed only by u_emu() function.
        /* 0x38 - 0x38 */
        // int   (__stdcall* u_ana)(void);
        internal delegate int AnalyzeInstructionDelegate();
        private AnalyzeInstructionDelegate _analyzeInstruction;

        internal int AnalyzeInstruction()
        {
            if (null == _analyzeInstruction)
            {
                _analyzeInstruction = MarshalingUtils.GetFunctionPointer<AnalyzeInstructionDelegate>(NativePointer, 0x38, 0x38);
            }
            return _analyzeInstruction();
        }

        // Emulate instruction, create cross-references, plan to analyze
        // subsequent instructions, modify flags etc. Upon entrance to this function
        // all information about the instruction is in 'cmd' structure.
        // If zero is returned, the kernel will delete the instruction.
        /* 0x3C - 0x3C */
        // int   (__stdcall* u_emu)   (void);
        internal delegate int EmulateInstructionDelegate();
        private EmulateInstructionDelegate _emulateInstruction;

        internal int EmulateInstruction()
        {
            if (null == _emulateInstruction)
            {
                _emulateInstruction = MarshalingUtils.GetFunctionPointer<EmulateInstructionDelegate>(NativePointer, 0x3C, 0x3C);
            }
            return _emulateInstruction();
        }

        // Generate text representation of an instruction in 'cmd' structure.
        // This function shouldn't change the database, flags or anything else.
        // All these actions should be performed only by u_emu() function.
        /* 0x40 - 0x40 */
        // void  (__stdcall* u_out)   (void);
        internal delegate void OutputInstructionTextDelegate();
        private OutputInstructionTextDelegate _outputInstructionText;

        internal void OutputInstructionText()
        {
            if (null == _outputInstructionText)
            {
                _outputInstructionText = MarshalingUtils.GetFunctionPointer<OutputInstructionTextDelegate>(NativePointer, 0x40, 0x40);
            }
            _outputInstructionText();
            return;
        }

        // Generate text representation of an instructon operand.
        // This function shouldn't change the database, flags or anything else.
        // All these actions should be performed only by u_emu() function.
        // The output text is placed in the output buffer initialized with init_output_buffer()
        // This function uses out_...() functions from ua.hpp to generate the operand text
        // Returns: 1-ok, 0-operand is hidden.
        /* 0x44 - 0x44 */
        // bool  (__stdcall* u_outop) (op_t &op);
        internal delegate bool OutputInstructionOperandTextDelegate(Operand op);
        private OutputInstructionOperandTextDelegate _outputInstructionOperandText;

        internal bool OutputInstructionOperandTex(Operand op)
        {
            if (null == _outputInstructionOperandText)
            {
                _outputInstructionOperandText = MarshalingUtils.GetFunctionPointer<OutputInstructionOperandTextDelegate>(NativePointer, 0x44, 0x44);
            }
            return _outputInstructionOperandText(op);
        }

        // Generate text represenation of data items
        // This function MAY change the database and create cross-references, etc.
        /* 0x48 - 0x48 */
        // void  (__stdcall* d_out)   (EffectiveAddress ea);          // disassemble data
        internal delegate void OutputDataTextDelegate(EffectiveAddress ea);
        private OutputDataTextDelegate _outputDataText;

        internal void OutputDataText(EffectiveAddress ea)
        {
            if (null == _outputDataText)
            {
                _outputDataText = MarshalingUtils.GetFunctionPointer<OutputDataTextDelegate>(NativePointer, 0x48, 0x48);
            }
            _outputDataText(ea);
            return;
        }

        // Compare instruction operands.
        // Returns 1-equal,0-not equal operands.
        // This pointer may be NULL.
        /* 0x4C - 0x4C */
        // bool  (__stdcall* cmp_opnd)(const op_t &op1, const op_t &op2);
        internal delegate bool CompareInstructionOperandDelegate(Operand op1, Operand op2);
        private CompareInstructionOperandDelegate _compareInstructionOperand;

        internal bool CompareInstructionOperand(Operand op1, Operand op2)
        {
            if (null == _compareInstructionOperand)
            {
                _compareInstructionOperand = MarshalingUtils.GetFunctionPointer<CompareInstructionOperandDelegate>(NativePointer, 0x4C, 0x4C);
            }
            return _compareInstructionOperand(op1, op2);
        }

        // Can the operand have a type as offset, segment, decimal, etc.
        // (for example, a register AX can't have a type, meaning that the user can't
        // change its representation. see bytes.hpp for information about types and flags)
        // This pointer may be NULL.
        /* 0x50 - 0x50 */
        // bool  (__stdcall* can_have_type)(op_t &op);
        internal delegate bool CanHaveTypeDelegate(Operand op);
        private CanHaveTypeDelegate _canHaveType;

        internal bool CanHaveType(Operand op)
        {
            if (null == _canHaveType)
            {
                _canHaveType = MarshalingUtils.GetFunctionPointer<CanHaveTypeDelegate>(NativePointer, 0x50, 0x50);
            }
            return _canHaveType(op);
        }

        //      Processor register information:
        /* 0x54 - 0x54 */
        // int   regsNum;                        // number of registers
        internal int RegistersCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x54, 0x54); }
        }

        /* 0x58 - 0x58 */
        // const char **regNames;                // their names
        private string[] _registers;

        internal IEnumerable<string> EnumerateRegisterNames()
        {
            if (null == _registers)
            {
                _registers = MarshalingUtils.GetStringsArray(NativePointer, 0x58, 0x58);
            }
            foreach (string registerName in _registers)
            {
                yield return registerName;
            }
            yield break;
        }

        // The following pointers should be NULL:
        /* 0x5C - 0x5C */
        // AbstractRegister *(__stdcall* getreg)(int regnum); // Get register value. If specified, will be used in the determining predefined comment based on the register value
        internal delegate IntPtr GetRegisterDelegate(int registerNumber);
        private GetRegisterDelegate _getRegister;

        internal GetRegisterDelegate GetRegister
        {
            get
            {
                if (null == _getRegister)
                {
                    _getRegister = MarshalingUtils.GetFunctionPointer<GetRegisterDelegate>(NativePointer, 0x5C, 0x5C);
                }
                return _getRegister;
            }
        }

        /* 0x60 - 0x60 */
        // int   rFiles;                         // number of register files
        internal int RegisterFilesCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x60, 0x60); }
        }

        /* 0x64 - 0x64 */
        // const char **rFnames;                 // register names for files
        private string[] _registerFileNames;

        internal IEnumerable<string> EnumerateRegisterFileNames()
        {
            if (null == _registerFileNames)
            {
                _registerFileNames = MarshalingUtils.GetStringsArray(NativePointer, 0x64, 0x64);
            }
            foreach (string registerName in _registerFileNames)
            {
                yield return registerName;
            }
            yield break;
        }

        /* 0x68 - 0x68 */
        // rginfo *rFdescs;                      // description of registers

        /* 0x6C - 0x6C */
        // WorkReg *CPUregs;                     // pointer to CPU registers

        // Segment register information (use virtual CS and DS registers if your
        // processor doesn't have segment registers):
        /* 0x70 - 0x70 */
        // int   regFirstSreg;                   // number of first segment register
        internal int FirstSegmentRegisterCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x70, 0x70); }
        }

        /* 0x74 - 0x74 */
        // int   regLastSreg;                    // number of last segment register
        internal int LastSegmentRegisterCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x74, 0x74); }
        }

        /* 0x78 - 0x78 */
        // int   segreg_size;                    // size of a segment register in bytes
        internal int SegmentRegisterByteSize
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x78, 0x78); }
        }

        // You should define 2 virtual segment registers for CS and DS.
        // Let's call them rVcs and rVds.
        /* 0x7C - 0x7C */
        // int   regCodeSreg;                    // number of CS register
        internal int CodeSegmentRegistersCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x7C, 0x7C); }
        }

        /* 0x80 - 0x80 */
        // int   regDataSreg;                    // number of DS register
        internal int DataSegmentRegistersCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x80, 0x80); }
        }

        //      Empirics
        /* 0x84 - 0x84 */
        // const bytes_t *codestart; // Array of typical code start sequences This array is used when a new file is loaded to find the beginnings of code sequences. This array is terminated with a zero length item.
        private byte[][] _codeStart;

        internal IEnumerable<byte[]> EnumerateCodeStartSequences()
        {
            if (null == _codeStart)
            {
                IntPtr arrayBase = MarshalingUtils.GetIntPtr(NativePointer, 0x84, 0x84);
                ushort itemOffset = 0;
                int itemLength;
                List<byte[]> sequences = new List<byte[]>();

                do
                {
                    itemLength = MarshalingUtils.GetByte(arrayBase, itemOffset, itemOffset);
                    itemOffset++;
                    if (0 < itemLength)
                    {
                        IntPtr sequenceAddress = MarshalingUtils.GetIntPtr(arrayBase, itemOffset, itemOffset);
                        itemOffset += 4;
                        sequences.Add(MarshalingUtils.GetBytes(sequenceAddress, itemOffset, itemOffset, itemLength));
                    }
                } while (0 != itemLength);
                _codeStart = sequences.ToArray();
            }
            foreach (byte[] sequence in _codeStart) { yield return sequence; }
            yield break;
        }
        
        /* 0x88 - 0x88 */
        // const bytes_t *retcodes; // Array of 'return' instruction opcodes This array is used to determine form of autogenerated locret_... labels. The last item of it should be { 0, NULL } This array may be NULL Better way of handling return instructions is to define the is_ret_insn callback in the notify() function
        private byte[] _returnOpcodes;

        internal IEnumerable<byte> EnumerateReturnOpCodes()
        {
            if (null == _returnOpcodes)
            {
                IntPtr arrayBase = MarshalingUtils.GetIntPtr(NativePointer, 0x88, 0x88);
                _returnOpcodes = MarshalingUtils.GetNullTerminatedBytes(arrayBase);
            }
            foreach (byte opCode in _returnOpcodes) { yield return opCode; }
            yield break;
        }

        //      Instruction set
        /* 0x8C - 0x8C */
        //int   instruc_start;                  // icode of the first instruction
        internal int FirstInstructionCode
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x8C, 0x8C); }
        }

        /* 0x90 - 0x90 */
        // int   instruc_end;                    // icode of the last instruction + 1
        internal int LastInstructionCode
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x90, 0x90); }
        }

        /* 0x94 - 0x94 */
        // instruc_t *instruc;                   // Array of instructions
        internal InstructionDescriptor GetInstructionDescriptor(int index)
        {
            if (null == _instructionDescriptors)
            {
                // On first call we capture instruction descriptors.
#if __EA64__
                IntPtr arrayBase = Marshal.ReadIntPtr(NativePointer, 0x94);
#else
                IntPtr arrayBase = Marshal.ReadIntPtr(NativePointer, 0x94);
#endif
                List<InstructionDescriptor> descriptors = new List<InstructionDescriptor>();
                int instructionsCount = LastInstructionCode - FirstInstructionCode;

                for (int instructionIndex = 0; instructionIndex < instructionsCount; instructionIndex++)
                {
                    descriptors.Add(new InstructionDescriptor(arrayBase, instructionIndex * InstructionDescriptor.NativeSize));
                }
                _instructionDescriptors = descriptors.ToArray();
            }
            return _instructionDescriptors[index];
        }

        // is indirect far jump or call instruction?
        // meaningful only if the processor has 'near' and 'far' reference types
        /* 0x98 - 0x98 */
        // int   (__stdcall* is_far_jump)(int icode);
        internal delegate int IsFarJumpDelegate(int instructionCode);
        private IsFarJumpDelegate _isFarJump;

        internal bool IsFarJump(int instructionCode)
        {
            if (null == _isFarJump)
            {
                _isFarJump = MarshalingUtils.GetFunctionPointer<IsFarJumpDelegate>(NativePointer, 0x98, 0x98);
            }
            return (0 != _isFarJump(instructionCode));
        }

        //      Translation function for offsets
        //      Currently used in the offset display functions
        //      to calculate the referenced address
        /* 0x9C - 0x9C */
        // EffectiveAddress (__stdcall* translate)(EffectiveAddress base, AddressDifference offset);
        internal delegate EffectiveAddress TranslateDelegate(EffectiveAddress baseAddress, AddressDifference offset);
        private TranslateDelegate _translate;

        internal EffectiveAddress Translate(EffectiveAddress baseAddress, AddressDifference offset)
        {
            if (null == _translate)
            {
                _translate = MarshalingUtils.GetFunctionPointer<TranslateDelegate>(NativePointer, 0x9C, 0x9C);
            }
            return _translate(baseAddress, offset);
        }

        //      Size of long double (tbyte) for this processor
        //      (meaningful only if ash.a_tbyte != NULL)
        /* 0xA0 - 0xA0 */
        // size_t tbyte_size;
        internal int LongDoubleBytesCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0xA0, 0xA0); }
        }

        // Floating point -> IEEE conversion function
        // error codes returned by this function (load/store):
        /* 0xA4 - 0xA4 */
        // int (__stdcall* realcvt)(void *m, uint16 *e, uint16 swt);

        // Number of digits in floating numbers after the decimal point.
        // If an element of this array equals 0, then the corresponding
        // floating point data is not used for the processor.
        // This array is used to align numbers in the output.
        //      real_width[0] - number of digits for short floats (only PDP-11 has them)
        //      real_width[1] - number of digits for "float"
        //      real_width[2] - number of digits for "double"
        //      real_width[3] - number of digits for "long double"
        // Example: IBM PC module has { 0,7,15,19 }
        /* 0xA8 - 0xA8 */
        // char real_width[4];

        // Find 'switch' idiom
        // fills 'si' structure with information and returns 1
        // returns 0 if switch is not found.
        // input: 'cmd' structure is correct. this function may use and modify 'cmd' structure it will be called for instructions marked with CF_JUMP
        /* 0xAC - 0xAC */
        // bool (__stdcall* is_switch)(switch_info_ex_t *si);
        internal delegate bool IsSwitchDelegate(EffectiveAddress baseAddress);
        private IsSwitchDelegate _isSwitch;

        internal bool IsSwitch(EffectiveAddress baseAddress)
        {
            if (null == _isSwitch)
            {
                _isSwitch = MarshalingUtils.GetFunctionPointer<IsSwitchDelegate>(NativePointer, 0xAC, 0xAC);
            }
            return _isSwitch(baseAddress);
        }

        //  Generate map file. If this pointer is NULL, the kernel itself
        //  will create the map file.
        //  This function returns number of lines in output file.
        //  0 - empty file, -1 - write error
        /* 0xB0 - 0xB0 */
        // int32 (__stdcall* gen_map_file)(FILE *fp);
        internal delegate int GenerateMapFileDelegate(IntPtr filePointer);
        private GenerateMapFileDelegate _generateMapFile;

        internal int GenerateMapFile(FileStream into)
        {
            if (null == _generateMapFile)
            {
                _generateMapFile = MarshalingUtils.GetFunctionPointer<GenerateMapFileDelegate>(NativePointer, 0xB0, 0xB0);
            }
            return _generateMapFile(into.Handle);
        }

        //  Extract address from a string. Returns EffectiveAddress.MaxValue if can't extract.
        //  Returns EffectiveAddress.MaxValue-1 if kernel should use standard algorithm.
        /* 0xB4 - 0xB4 */
        // EffectiveAddress (__stdcall* extract_address)(EffectiveAddress ea,const char *string,int x);

        //  Check whether the operand is relative to stack pointer or frame pointer.
        //  This function is used to determine how to output a stack variable
        //  This function may be absent. If it is absent, then all operands
        //  are sp based by default.
        //  Define this function only if some stack references use frame pointer
        //  instead of stack pointer.
        //  returns flags:
        /* 0xB8 - 0xB8 */
        // int (__stdcall* is_sp_based)(const op_t &x);
        internal delegate int IsSPBasedDelegate(IntPtr nativeOperand);
        private IsSPBasedDelegate _isSPBased;

        internal int GenerateMapFile(Operand operand)
        {
            if (null == _isSPBased)
            {
                _isSPBased = MarshalingUtils.GetFunctionPointer<IsSPBasedDelegate>(NativePointer, 0xB8, 0xB8);
            }
            return _isSPBased(operand.GetNative());
        }

        //  Create a function frame for a newly created function.
        //  Set up frame size, its attributes etc.
        //  This function may be absent.
        /* 0xBC - 0xBC */
        // bool (__stdcall* create_func_frame)(func_t *pfn);
        internal delegate bool CreateFunctionFrameDelegate(IntPtr nativeFunction);
        private CreateFunctionFrameDelegate _createFunctionFrame;

        internal bool CreateFunctionFrame(Function function)
        {
            if (null == _createFunctionFrame)
            {
                _createFunctionFrame = MarshalingUtils.GetFunctionPointer<CreateFunctionFrameDelegate>(NativePointer, 0xBC, 0xBC);
            }
            return _createFunctionFrame(function.GetNative());
        }

        // Get size of function return address in bytes
        //      pfn - pointer to function structure, can't be NULL
        // If this function is absent, the kernel will assume
        //      4 bytes for 32-bit function
        //      2 bytes otherwise
        /* 0xC0 - 0xC0 */
        // int (__stdcall* get_frame_retsize)(func_t *pfn);
        internal delegate int GetFrameReturnSizeDelegate(IntPtr nativeFunction);
        private GetFrameReturnSizeDelegate _getFrameReturnSize;

        internal int GetFrameReturnSize(Function function)
        {
            if (null == _getFrameReturnSize)
            {
                _getFrameReturnSize = MarshalingUtils.GetFunctionPointer<GetFrameReturnSizeDelegate>(NativePointer, 0xC0, 0xC0);
            }
            return _getFrameReturnSize(function.GetNative());
        }

        //  Generate stack variable definition line
        //  If this function is NULL, then the kernel will create this line itself.
        //  Default line is varname = type ptr value
        //  where 'type' is one of byte,word,dword,qword,tbyte
        /* 0xC4 - 0xC4 */
        // void (__stdcall* gen_stkvar_def)(char *buf, size_t bufsize, const member_t *mptr, AddressDifference v);
        internal delegate void GenerateStackVariableDefinitionDelegate(IntPtr buffer, int bufferSize, IntPtr nativeMember, AddressDifference v);
        private GenerateStackVariableDefinitionDelegate _generateStackVariableDefinition;

        internal string GetFrameReturnSize(Member member, AddressDifference v)
        {
            if (null == _generateStackVariableDefinition)
            {
                _generateStackVariableDefinition = MarshalingUtils.GetFunctionPointer<GenerateStackVariableDefinitionDelegate>(NativePointer, 0xC4, 0xC4);
            }
            int nativeBufferSize = 256;
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(nativeBufferSize);

            try
            {
                _generateStackVariableDefinition(nativeBuffer, nativeBufferSize, member.GetNative(), v);
                return Marshal.PtrToStringAnsi(nativeBuffer);
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        // Generate text representation of an item in a special segment
        // i.e. absolute symbols, externs, communal definitions etc.
        // returns: 1-overflow, 0-ok
        /* 0xC8 - 0xC8 */
        // bool (__stdcall* u_outspec)(EffectiveAddress ea,uchar segtype);
        internal delegate bool GenerateSpecialSegmentItemTextDelegate(EffectiveAddress ea, byte segmentType);
        private GenerateSpecialSegmentItemTextDelegate _generateSpecialSegmentItemText;

        internal bool GenerateSpecialSegmentItemText(EffectiveAddress ea, byte segmentType)
        {
            if (null == _generateSpecialSegmentItemText)
            {
                _generateSpecialSegmentItemText = MarshalingUtils.GetFunctionPointer<GenerateSpecialSegmentItemTextDelegate>(NativePointer, 0xC8, 0xC8);
            }

            return !_generateSpecialSegmentItemText(ea, segmentType);
        }

        // Icode of return instruction. It is ok to give any of possible return instructions
        /* 0xCC - 0xCC */
        // int icode_return;
        internal int ReturnInstructionCode
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x158, 0xCC); }
        }

        // Set IDP-specific option (see below)
        /* 0xD0 - 0xD0 */
        // set_options_t *set_idp_options;

        // Is the instruction created only for alignment purposes?
        // returns: number of bytes in the instruction
        /* 0xD4 - 0xD4 */
        // int (__stdcall* is_align_insn)(EffectiveAddress ea);
        internal delegate int IsAlignmentInstructionDelegate(EffectiveAddress ea);
        private IsAlignmentInstructionDelegate _isAlignmentInstruction;

        internal int GenerateSpecialSegmentItemText(EffectiveAddress ea)
        {
            if (null == _isAlignmentInstruction)
            {
                _isAlignmentInstruction = MarshalingUtils.GetFunctionPointer<IsAlignmentInstructionDelegate>(NativePointer, 0xD4, 0xD4);
            }

            return _isAlignmentInstruction(ea);
        }

        // Micro virtual machine description
        // If NULL, IDP doesn't support microcodes.
        /* 0xD8 - 0xD8 */
        // mvm_t *mvm;

        //      If the FIXUP_VHIGH and FIXUP_VLOW fixup types are supported
        //      then the number of bits in the HIGH part. For example,
        //      SPARC will have here 22 because it has HIGH22 and LOW10 relocations.
        //      See also: the description of PR_FULL_HIFXP bit
        /* 0xDC - 0xDC */
        // int high_fixup_bits;
        internal int HighFixupBitsCount
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x158, 0xCC); }
        }
        #endregion

        #region MANAGED FIELDS
        private static Processor _currentProcessor;
        #endregion
    }
}
