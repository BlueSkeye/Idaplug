using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    // The database parameters
    // This structure is kept in the ida database
    // It contains the essential parameters for the current program
    internal class DatabaseInfo
    {
        #region CONSTRUCTORS
        private DatabaseInfo(IntPtr native)
        {
            NativePointer = native;
            DatabaseInfo.Current = this;
            return;
        }
        #endregion

        #region PROPERTIES
        // size of input file core image used to create executable file
        internal MemoryChunkSize CoreFileSize
        {
            get { return GetMemoryChunkSize(0x11, 0x11); }
        }

        internal static DatabaseInfo Current { get; private set; }

        internal DemangledNamesStyle Demangling
        {
            get { return ((DemangledNamesStyle)Marshal.ReadByte(NativePointer, 0x0E) & DemangledNamesStyle.DEMNAM_MASK); }
            set { Marshal.WriteByte(NativePointer, 0x0E, (byte)value); }
        }

        internal FileType FileType
        {
            get { return (FileType)Marshal.ReadInt16(NativePointer, 0x0F); }
            set { Marshal.WriteInt16(NativePointer, 0x0F, (short)value); }
        }

        internal bool Is32Bits
        {
            get { return (0 != (MiscFlags & DatabaseMiscFlags.LFLG_PC_FLAT)); }
        }

        internal bool Is64Bits
        {
            get { return (0 != (MiscFlags & DatabaseMiscFlags.LFLG_64BIT)); }
        }

        internal bool IsDll
        {
            get { return (0 != (MiscFlags & DatabaseMiscFlags.LFLG_IS_DLL)); }
        }

        // is unstructured input file?
        internal bool IsLikeBinary
        {
            get
            {
                switch (FileType)
                {
                    case IdaInterop.FileType.f_BIN:
                    case IdaInterop.FileType.f_HEX:
                    case IdaInterop.FileType.f_MEX:
                    case IdaInterop.FileType.f_SREC:
                        return true;
                    default:
                        return false;
                }
            }
        }

        internal bool IsSnapshot
        {
            get { return (0 != (MiscFlags & DatabaseMiscFlags.LFLG_SNAPSHOT)); }
        }

        internal bool LoadingIDC
        {
            get { return (0 != (GeneralFlags & GeneralFlags.INFFL_LOADIDC)); } 
        }

        internal DatabaseMiscFlags MiscFlags
        {
            get { return (DatabaseMiscFlags)Marshal.ReadByte(NativePointer, 0x0D); }
            set { Marshal.WriteByte(NativePointer, 0x0D, (byte)value); }
        }

        internal IntPtr NativePointer { get; private set; }

        internal string ProcName
        {
            get { return Marshal.PtrToStringAnsi(InteropConstants.Combine(NativePointer, 0x05), 8); }
            set
            {
                if (null == value) { throw new ArgumentNullException(); }
                if (8 < value.Length) { value = value.Substring(0, 8); }
                byte[] localBuffer = ASCIIEncoding.ASCII.GetBytes(value);
                Marshal.Copy(localBuffer, 0, InteropConstants.Combine(NativePointer, 0x05), localBuffer.Length);
                if (8 > localBuffer.Length)
                {
                    Marshal.WriteByte(NativePointer, 0x05 + localBuffer.Length, 0);
                }
                return;
            }
        }

        internal bool UseAllAsm
        {
            get { return (0 != (GeneralFlags & GeneralFlags.INFFL_ALLASM)); }
        }

        // Version of database
        internal ushort Version
        {
            get { return GetUShort(0x03, 0x03); }
        }
        #endregion

        #region METHODS
        private bool GetBool(ushort ea64Offset, ushort ea32Offset)
        {
            return MarshalingUtils.GetBool(NativePointer, ea64Offset, ea32Offset);
        }

        private EffectiveAddress GetEffectiveAddress(ushort ea64Offset, ushort ea32Offset)
        {
            return MarshalingUtils.GetEffectiveAddress(NativePointer, ea64Offset, ea32Offset);
        }

        private MemoryChunkSize GetMemoryChunkSize(ushort ea64Offset, ushort ea32Offset)
        {
            return MarshalingUtils.GetMemoryChunkSize(NativePointer, ea64Offset, ea32Offset);
        }

        private SegmentSelector GetSegmentSelector(ushort ea64Offset, ushort ea32Offset)
        {
            return MarshalingUtils.GetSegmentSelector(NativePointer, ea64Offset, ea32Offset);
        }

        private ushort GetUShort(ushort ea64Offset, ushort ea32Offset)
        {
            return MarshalingUtils.GetUShort(NativePointer, ea64Offset, ea32Offset);
        }
        #endregion

        #region NATIVE FIELDS
        /* 0x00 - 0x00 */
        // private char[] tag; // [3];                 // 'IDA'
        /* 0x03 - 0x03 */
        // private ushort version; // Version of database
        /* 0x05 - 0x05 */
        // private char[] procName; // [8];            // Name of current processor
        /* 0x0D - 0x0D */
        // private DatabaseMiscFlags lflags; // Misc. flags
        /* 0x0E - 0x0E */
        // private DemangledNamesStyle demnames; // display demangled names as:
        /* 0x0F - 0x0F */
        // private FileType filetype;
        /* 0x11 - 0x11 */
        // private MemoryChunkSize fcoresiz; // size of input file core image used to create executable file
        /* 0x15 - 0x19 */
        // MemoryChunkSize corestart;              // start of pages to load (offset in the file)

        // start of pages to load (offset in the file)
        internal MemoryChunkSize CoreLoadablePagesFileOffset
        {
            get { return GetMemoryChunkSize(0x19, 0x15); }
        }

        /* 0x19 - 0x21 */
        // ushort ostype;                 // OS type the program is for bit definitions in libfuncs.hpp

        // OS type the program is for bit definitions in libfuncs.hpp
        internal ushort OSType
        {
            get { return GetUShort(0x21, 0x19); }
        }

        /* 0x1B - 0x23 */
        // ushort apptype;                // Application type bit definitions in libfuncs.hpp

        // Application type bit definitions in libfuncs.hpp
        internal ushort ApplicationType
        {
            get { return GetUShort(0x23, 0x18); }
        }

        /* 0x1D - 0x25 */
        // EffectiveAddress startSP;    // SP register value at the start of program execution

        // SP register value at the start of program execution
        internal EffectiveAddress StartSPValue
        {
            get { return GetEffectiveAddress(0x25, 0x10); }
        }

        /* 0x21 - 0x2D */
        // AnalysisFlags af;                     // Analysis flags:

        // Analysis flags:
        internal AnalysisFlags AnalysisFlags
        {
            get
            {
#if __EA64__
                return (AnalysisFlags)Marshal.ReadInt16(NativePointer, 0x2D);
#else
                return (AnalysisFlags)Marshal.ReadInt16(NativePointer, 0x21);
#endif
            }
        }

        /* 0x23 - 0x2F */
        // EffectiveAddress startIP; // IP register value at the start of program execution

        // IP register value at the start of program execution
        internal EffectiveAddress StartIPValue
        {
            get { return GetEffectiveAddress(0x2F, 0x23); }
        }

        /* 0x27 - 0x37 */
        // EffectiveAddress beginEA;                // Linear address of program entry point

        // Linear address of program entry point
        internal EffectiveAddress ProgramEntryPointLinearAddress
        {
            get { return GetEffectiveAddress(0x37, 0x27); }
        }

        /* 0x2B - 0x3F */
        // EffectiveAddress minEA;                  // current limits of program

        internal EffectiveAddress MinLinearAddress
        {
            get { return GetEffectiveAddress(0x37, 0x2B); }
        }

        /* 0x2F - 0x47 */
        // EffectiveAddress maxEA;                  // maxEA is excluded

        internal EffectiveAddress MaxLinearAddress
        {
            get { return GetEffectiveAddress(0x47, 0x2F); }
        }

        /* 0x33 - 0x4F */
        // EffectiveAddress ominEA;                 // original minEA (is set after loading the input file)

        // original minEA (is set after loading the input file)
        internal EffectiveAddress OriginalMinLinearAddress
        {
            get { return GetEffectiveAddress(0x4F, 0x33); }
        }

        /* 0x37 - 0x57 */
        // EffectiveAddress omaxEA;                 // original maxEA (is set after loading the input file)

        // original maxEA (is set after loading the input file)
        internal EffectiveAddress OriginalMaxLinearAddress
        {
            get { return GetEffectiveAddress(0x57, 0x37); }
        }

        /* 0x3B - 0x5F */
        // EffectiveAddress lowoff;                 // Low  limit for offsets (used in calculation of 'void' operands)

        // Low  limit for offsets (used in calculation of 'void' operands)
        internal EffectiveAddress LowOffsetsLimit
        {
            get { return GetEffectiveAddress(0x38, 0x5F); }
        }

        /* 0x3F - 0x67 */
        // EffectiveAddress highoff;                // High limit for offsets (used in calculation of 'void' operands)

        // High limit for offsets (used in calculation of 'void' operands)
        internal EffectiveAddress HighOffsetsLimit
        {
            get { return GetEffectiveAddress(0x67, 0x3F); }
        }

        /* 0x43 - 0x6F */
        // MemoryChunkSize maxref;                 // Max tail for references

        // Max tail for references
        internal MemoryChunkSize MaxReferenceTail
        {
            get { return GetMemoryChunkSize(0x6F, 0x43); }
        }

        /* 0x47 - 0x77 */
        // byte ASCIIbreak;             // ASCII line break symbol.

        // ASCII line break symbol.
        internal char ASCIILineBreakSymbol
        {
            get
            {
#if __EA64__
                return (char)Marshal.ReadByte(NativePointer, 0x77);
#else
                return (char)Marshal.ReadByte(NativePointer, 0x47);
#endif
            }
        }

        /* 0x48 - 0x78 */
        // byte wide_high_byte_first;   // Bit order of wide bytes: high byte first? (wide bytes: ph.nbits > 8)

        // Bit order of wide bytes: high byte first? (wide bytes: ph.nbits > 8)
        internal bool WideHighByteFirst
        {
            get { return GetBool(0x78, 0x48); }
        }

        /* 0x49 - 0x79 */
        // byte indent;                 // Indention for instructions

        // Indention for instructions
        internal byte InstructionsIndentation
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0x79);
#else
                return Marshal.ReadByte(NativePointer, 0x49);
#endif
            }
        }

        /* 0x4A - 0x7A */
        // byte comment;                // Indention for comments

        // Indention for comments
        internal byte CommentsIndentation
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0x7A);
#else
                return Marshal.ReadByte(NativePointer, 0x4A);
#endif
            }
        }

        /* 0x4B - 0x7B */
        // byte xrefnum;                // Number of references to generate 0 - xrefs won't be generated at all

        // Number of references to generate 0 - xrefs won't be generated at all
        internal byte MaxCrossReferencesCount
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0x7B);
#else
                return Marshal.ReadByte(NativePointer, 0x4B);
#endif
            }
        }

        /* 0x4C - 0x7C */
        // byte s_entab;                // Use '\t' chars in the output file?

        // Use '\t' chars in the output file?
        internal bool UseTabsInOutputFile
        {
            get { return GetBool(0x7C, 0x4C); }
        }

        /* 0x4D - 0x7D */
        // byte specsegs;               // New format of special segments?

        // New format of special segments?
        internal bool UseNewFormatForSpecialSegmnts
        {
            get { return GetBool(0x7D, 0x4D); }
        }

        /* 0x4E - 0x7E */
        // byte s_void;                 // Display void marks?

        // Display void marks?
        internal bool DisplayVoidMarks
        {
            get { return GetBool(0x7E, 0x4E); }
        }

        /* 0x4F - 0x7F */
        // byte s_reserved2;            // obsolete

        /* 0x50 - 0x80 */
        // byte s_showauto;             // Display autoanalysis indicator?

        // Display autoanalysis indicator?
        internal bool DisplayAutoAnalysisIndicator
        {
            get { return GetBool(0x80, 0x50); }
        }

        /* 0x51 - 0x81 */
        // byte s_auto;                 // Autoanalysis is enabled?

        // Autoanalysis is enabled?
        internal bool AutoAnalysisEnabled
        {
            get { return GetBool(0x81, 0x51); }
        }

        /* 0x52 - 0x82 */
        // DelimiterStyle s_limiter;    // Generate delimiters:

        // Delimiters generation style
        internal DelimiterStyle DelimiterStyle
        {
            get
            {
#if __EA64__
                return (DelimiterStyle) Marshal.ReadByte(NativePointer, 0x82);
#else
                return (DelimiterStyle) Marshal.ReadByte(NativePointer, 0x52);
#endif
            }
        }

        /* 0x53 - 0x83 */
        // byte s_null;                 // Generate empty lines?

        // Generate empty lines?
        internal bool GenerateEmptyLines
        {
            get { return GetBool(0x83, 0x53); }
        }

        /* 0x54 - 0x84 */
        // GeneralFlags s_genflags;             // General flags:

        // General flags:
        internal GeneralFlags GeneralFlags
        {
            get
            {
#if __EA64__
                return (GeneralFlags)Marshal.ReadByte(NativePointer, 0x84);
#else
                return (GeneralFlags)Marshal.ReadByte(NativePointer, 0x54);
#endif
            }
        }

        /* 0x55 - 0x85 */
        // byte s_showpref;             // Show line prefixes?

        // Show line prefixes?
        internal bool ShowLinePrefixes
        {
            get { return GetBool(0x85, 0x55); }
        }

        /* 0x56 - 0x86 */
        // byte s_prefseg;              // line prefixes with segment name?

        // line prefixes with segment name?
        internal bool IncludeSegmentNameInLinePrefix
        {
            get { return GetBool(0x86, 0x56); }
        }

        /* 0x57 - 0x87 */
        // byte asmtype;                // target assembler number

        // target assembler number
        internal byte TargetAssemblerNumber
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0x87);
#else
                return Marshal.ReadByte(NativePointer, 0x57);
#endif
            }
        }

        /* 0x58 - 0x88 */
        // MemoryChunkSize baseaddr;               // base address of the program (paragraphs)

        // base address of the program (paragraphs)
        internal MemoryChunkSize BaseAddress
        {
            get { return GetMemoryChunkSize(0x88, 0x58); }
        }

        /* 0x5C - 0x90 */
        // CrossReferencesFlags s_xrefflag;

        internal CrossReferencesFlags CrossReferencesFlags
        {
            get
            {
#if __EA64__
                return (CrossReferencesFlags)Marshal.ReadByte(NativePointer, 0x90);
#else
                return (CrossReferencesFlags)Marshal.ReadByte(NativePointer, 0x5C);
#endif
            }
        }

        /* 0x5D - 0x91 */
        // short binSize;                // # of instruction bytes to show in line prefix

        // # of instruction bytes to show in line prefix
        internal short LinePrefixBytesCount
        {
            get
            {
#if __EA64__
                return Marshal.ReadInt16(NativePointer, 0x91);
#else
                return Marshal.ReadInt16(NativePointer, 0x5D);
#endif
            }
        }

        /* 0x5F - 0x93 */
        // CommentFlags s_cmtflg;               // comments:

        // Comments
        internal CommentFlags CommentFlags
        {
            get
            {
#if __EA64__
                return (CommentFlags)Marshal.ReadByte(NativePointer, 0x93);
#else
                return (CommentFlags)Marshal.ReadByte(NativePointer, 0x5F);
#endif
            }
        }

        /* 0x60 - 0x94 */
        // NameType nametype;               // dummy names represenation type

        internal NameType DummyNamesType
        {
            get
            {
#if __EA64__
                return (NameType)Marshal.ReadByte(NativePointer, 0x94);
#else
                return (NameType)Marshal.ReadByte(NativePointer, 0x60);
#endif
            }
        }

        /* 0x61 - 0x95 */
        // byte s_showbads;             // show bad instructions? an instruction is bad if it appears in the ash.badworks array

        // show bad instructions? an instruction is bad if it appears in the ash.badworks array
        internal bool ShowBadInstructions
        {
            get { return GetBool(0x95, 0x61); }
        }

        /* 0x62 - 0x96 */
        // PrefixFlags s_prefflag;

        internal PrefixFlags PrefixFlags
        {
            get
            {
#if __EA64__
                return (PrefixFlags)Marshal.ReadByte(NativePointer, 0x96);
#else
                return (PrefixFlags)Marshal.ReadByte(NativePointer, 0x62);
#endif
            }
        }

        /* 0x63 - 0x97 */
        // byte s_packbase;             // pack database?

        internal bool PackDatabase
        {
            get { return GetBool(0x97, 0x63); }
        }

        /* 0x64 - 0x98 */
        // AsciiFlags asciiflags;             // ASCII string flags

        // ASCII string flags
        internal AsciiFlags AsciiFlags
        {
            get
            {
#if __EA64__
                return (AsciiFlags)Marshal.ReadByte(NativePointer, 0x98);
#else
                return (AsciiFlags)Marshal.ReadByte(NativePointer, 0x64);
#endif
            }
        }

        /* 0x65 - 0x99 */
        // ListNameFlags listnames;              // What names should be included in the list?

        // What names should be included in the list?
        internal ListNameFlags ListNameFlags
        {
            get
            {
#if __EA64__
                return (ListNameFlags)Marshal.ReadByte(NativePointer, 0x98);
#else
                return (ListNameFlags)Marshal.ReadByte(NativePointer, 0x64);
#endif
            }
        }

        /* 0x66 - 0x9A */
        // byte[] ASCIIpref; // [16];          // ASCII names prefix
        
        // TODO

        /* 0x76 - 0xAA */
        // MemoryChunkSize ASCIIsernum;            // serial number

        // Serial number
        internal MemoryChunkSize SerialNumber
        {
            get { return GetMemoryChunkSize(0xAA, 0x76); }
        }

        /* 0x7A - 0xA2 */
        // byte ASCIIzeroes;            // leading zeroes

        // leading zeroes
        internal byte LeadingZeroesCount
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0xA2);
#else
                return Marshal.ReadByte(NativePointer, 0x7A);
#endif
            }
        }

        /* 0x7B - 0xA3 */
        // byte graph_view;             // currently using graph options (dto.graph)

        // currently using graph options (dto.graph)
        internal bool GraphicalView
        {
            get { return GetBool(0xA3, 0x78); }
        }

        /* 0x7C - 0xA4 */
        // byte s_reserved5;            // old memory model & calling convention

        /* 0x7D - 0xA5 */
        // byte tribyte_order;          // tribyte_order_t: order of bytes in 3-byte items

        // tribyte_order_t: order of bytes in 3-byte items
        internal byte TriByteOrder
        {
            get
            {
#if __EA64__
                return Marshal.ReadByte(NativePointer, 0xA5);
#else
                return Marshal.ReadByte(NativePointer, 0x7D);
#endif
            }
        }

        /* 0x7E - 0xA6 */
        // byte mf;                     // Byte order: is MSB first?

        // Byte order: is MSB first?
        internal bool MSBFirst
        {
            get { return GetBool(0xA6, 0x7E); }
        }

        /* 0x7F - 0xA7 */
        // byte s_org;                  // Generate 'org' directives?

        // Generate 'org' directives?
        internal bool GenerateORGDirectives
        {
            get { return GetBool(0xA7, 0x7F); }
        }

        /* 0x80 - 0xA8 */
        // byte s_assume;               // Generate 'assume' directives?

        // Generate 'assume' directives?
        internal bool GenerateAssumeDirectives
        {
            get { return GetBool(0xA8, 0x80); }
        }

        /* 0x81 - 0xA9 */
        // byte s_checkarg;             // Check manual operands?

        // Check manual operands?
        internal bool CheckManualOperands
        {
            get { return GetBool(0xA9, 0x81); }
        }

        /* 0x82 - 0xAA */
        // SegmentSelector start_ss;               // selector of the initial stack segment

        // selector of the initial stack segment
        internal SegmentSelector InitialStackSegment
        {
            get { return GetSegmentSelector(0xAA, 0x82); }
        }

        /* 0x86 - 0xB2 */
        // SegmentSelector start_cs;               // selector of the segment with the main entry point

        // selector of the segment with the main entry point
        internal SegmentSelector InitialCodeSegment
        {
            get { return GetSegmentSelector(0xB2, 0x86); }
        }

        /* 0x8A - 0xBA */
        // EffectiveAddress main;                   // address of main()

        // address of main()
        internal EffectiveAddress MainAddress
        {
            get { return GetEffectiveAddress(0xBA, 0x8A); }
        }

        /* 0x8E - 0xC2 */
        // uint short_demnames;        // short form of demangled names

        // short form of demangled names
        internal uint DemangledNamesShortForm
        {
            get
            {
#if __EA64__
                return (uint)Marshal.ReadInt32(NativePointer, 0xC2);
#else
                return (uint)Marshal.ReadInt32(NativePointer, 0x8E);
#endif
            }
        }

        /* 0x92 - 0xC6 */
        // EA64_ALIGN(align_short_demnames)
        /* 0x92 - 0xCA */
        // uint long_demnames;         // long form of demangled names see demangle.h for definitions

        // long form of demangled names see demangle.h for definitions
        internal uint DemangledNamesLongForm
        {
            get
            {
#if __EA64__
                return (uint)Marshal.ReadInt32(NativePointer, 0xCA);
#else
                return (uint)Marshal.ReadInt32(NativePointer, 0x92);
#endif
            }
        }

        /* 0x96 - 0xCE */
        // EA64_ALIGN(align_long_demnames)
        /* 0x96 - 0xD2 */
        // MemoryChunkSize datatypes; // data types allowed in data carousel used in MakeData command.

        // data types allowed in data carousel used in MakeData command.
        internal MemoryChunkSize DataTypes
        {
            get { return GetMemoryChunkSize(0xD2, 0x96); }
        }

        /* 0x9A - 0xDA */
        // int strtype;               // current ascii string type see nalt.hpp for string types

        // current ascii string type see nalt.hpp for string types
        internal int StartType
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0xDA, 0x9A); }
        }

        /* 0x9E - 0xDE */
        // EA64_ALIGN(align_strtype)
        /* 0x9E - 0xE2 */
        // AnalysisFlagsEx af2;                    // Analysis flags 2:
        // Analysis flags 2:
        internal AnalysisFlagsEx AnalysisFlagsEx
        {
            get { return (AnalysisFlagsEx)MarshalingUtils.GetInt32(NativePointer, 0xE2, 0x9E); }
        }

        /* 0xA0 - 0xE4 */
        // ushort namelen;                // max name length (without zero byte)

        // max name length (without zero byte)
        internal ushort MaxNameLength
        {
            get { return GetUShort(0xE4, 0xA0); }
        }

        /* 0xA2 - 0xE6 */
        // ushort margin;                 // max length of data lines

        internal ushort MaxDataLineLength
        {
            get { return GetUShort(0xE6, 0xA2); }
        }

        /* 0xA4 - 0xE8 */
        // ushort lenxref;                // max length of line with xrefs

        // max length of line with xrefs
        internal ushort MaxLineLengthWithXRefs
        {
            get { return GetUShort(0xE8, 0xA4); }
        }

        /* 0xA6 - 0xEA */
        // byte[] lprefix; // [16];            // prefix of local names if a new name has this prefix, it will be automatically converted to a local name
        private string _localPrefix;

        internal string LocalPrefix
        {
            get
            {
                if (null == _localPrefix)
                {
                    IntPtr prefixAddress = new IntPtr(NativePointer.ToInt32() +
#if __EA64__
                        0xEA
#else
                        0xA6
#endif
                        );
                    _localPrefix = Marshal.PtrToStringAnsi(prefixAddress, LocalPrefixLength);
                }
                return _localPrefix;
            }
        }

        // TODO

        /* 0xB6 - 0xFA */
        // byte lprefixlen;             // length of the lprefix
        // length of the lprefix
        internal byte LocalPrefixLength
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0xFA, 0xB6); }
        }

        /* 0xB7 - 0xFB */
        // CompilerInfo cc; // Target compiler WARNING inlined.
        private CompilerInfo _compiler;

        internal CompilerInfo Compiler
        {
            get
            {
                if (null == _compiler)
                {
                    _compiler = new CompilerInfo(MarshalingUtils.Combine(NativePointer, 0xFB, 0xB7));
                }
                return _compiler;
            }
        }

        /* 0xC0 - 0x104 */
        // uint database_change_count;  // incremented after each byte and regular segment modifications
        // incremented after each byte and regular segment modifications
        internal uint DatabaseChangeCount
        {
            get { return MarshalingUtils.GetUInt(NativePointer, 0x104, 0xC0); }
        }

        /* 0xC4 - 0x108 */
        // byte size_ldbl;              // sizeof(long double) if different from ph.tbyte_size
        // sizeof(long double) if different from ph.tbyte_size
        internal byte SizeofLongDouble
        {
            get { return MarshalingUtils.GetByte(NativePointer, 0x108, 0xC4); }
        }

        /* 0xC5 - 0x109 */
        // uint appcall_options;        // appcall options, see idd.hpp
        // appcall options, see idd.hpp
        internal uint AppCallOptions
        {
            get { return MarshalingUtils.GetUInt(NativePointer, 0x109, 0xC5); }
        }

        /* 0xC9 - 0x10D */
        // byte[] reserved; // [55];           // 55 zero bytes for the future total size: 256 bytes (for 32bit)
        /* 0x100 - 0x144 */

  //void init();
  //bool retrieve();                  // low level function to get this structure from the database
  //bool read(string basename);      // high level function to get this structure from the database and convert to the current format
  //void write();                     // write back to the database
        #endregion
    }
}
