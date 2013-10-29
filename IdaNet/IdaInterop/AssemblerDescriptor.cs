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
    //      This structure describes the target assembler.
    //      An IDP module may have several target assemblers.
    //      In this case you should create a structure for each supported
    //      assembler.
    internal class AssemblerDescriptor
    {
        #region CONSTRUCTORS
        internal AssemblerDescriptor(IntPtr native)
        {
            NativePointer = native;
            return;
        }
        #endregion

        #region PROPERTIES
        internal IntPtr NativePointer { get; private set; }
        #endregion

        #region IDA NATIVE FIELDS
        /* 0x00 - 0x00 */
        // uint32 flag;                           // Assembler features:
        internal AssemblerFeatures Features
        {
            get { return (AssemblerFeatures)MarshalingUtils.GetUInt(NativePointer, 0x00, 0x00); }
        }

        /* 0x04 - 0x04 */
        // uint16 uflag; // user defined flags (local only for IDP) you may define and use your own bits
        internal ushort UserDefinedFlags
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x04, 0x04); }
        }

        /* 0x06 - 0x06 */
        // const char *name;                     // Assembler name (displayed in menus)
        internal string Name
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x06, 0x06); }
        }

        /* 0x0A - 0x0A */
        // int help;                          // Help screen number, 0 - no help
        internal int HelpScreenNumber
        {
            get { return MarshalingUtils.GetInt32(NativePointer, 0x0A, 0x0A); }
        }

        /* 0x0E - 0x0E */
        // const char **header;                  // array of automatically generated header lines they appear at the start of disassembled text
        internal string[] Headers
        {
            get { return MarshalingUtils.GetStringsArray(NativePointer, 0x0E, 0x0E); }
        }

        /* 0x12 - 0x12 */
        // const uint16 *badworks;               // array of unsupported instructions (array of cmd.itype, zero terminated)

        // 0x16 - 0x16 */
        // const char *origin;                   // org directive
        internal string OriginDirectiveText
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x16, 0x16); }
        }

        // 0x1A - 0x1A */
        // const char *end;                      // end directive
        internal string EndDirectiveText
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x1A, 0x1A); }
        }

        /* 0x1E - 0x1E */
        // const char *cmnt;                     // comment string (see also cmnt2)
        internal string CommentText
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x1E, 0x1E); }
        }

        /* 0x22 - 0x22 */
        // char ascsep;                          // ASCII string delimiter
        internal char AsciiStringDelimiter
        {
            get { return (char)MarshalingUtils.GetByte(NativePointer, 0x22, 0x22); }
        }

        /* 0x23 - 0x23 */
        // char accsep;                          // ASCII char constant delimiter
        internal char AsciiConstantCharacterDelimiter
        {
            get { return (char)MarshalingUtils.GetByte(NativePointer, 0x23, 0x23); }
        }

        /* 024 - 0x24 */
        // const char *esccodes;                 // ASCII special chars (they can't appear in character and ascii constants)

        //      Data representation (db,dw,...):
        /* 0x28 - 0x28 */
        // const char *a_ascii;                  // ASCII string directive
        internal string AsciiStringDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x28, 0x28); }
        }

        // 0x2C - 0x2C */
        // const char *a_byte;                   // byte directive
        internal string ByteDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x2C, 0x2C); }
        }

        // 0x30 - 0x30 */
        // const char *a_word;                   // word directive
        internal string WordDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x30, 0x30); }
        }

        // 0x34 - 0x34 */
        // const char *a_dword;                  // NULL if not allowed
        internal string DoubleWordDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x34, 0x34); }
        }

        /* 0x38 - 0x38 */
        // const char *a_qword;                  // NULL if not allowed
        internal string QuadroWordDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x38, 0x38); }
        }

        /* 0x3C - 0x3C */
        // const char *a_oword;                  // NULL if not allowed
        internal string OctoWordDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x3C, 0x3C); }
        }

        /* 0x40 - 0x40 */
        // const char *a_float;                  // float;  4bytes; NULL if not allowed
        internal string FloatDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x40, 0x40); }
        }

        /* 0x4C - 0x4C */
        // const char *a_double;                 // double; 8bytes; NULL if not allowed
        internal string DoubleDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x4C, 0x4C); }
        }
        
        /* 0x50 - 0x50 */
        // const char *a_tbyte;                  // long double;    NULL if not allowed
        internal string LongDoubleDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x50, 0x50); }
        }
        
        /* 0x54 - 0x54 */
        // const char *a_packreal;               // packed decimal real NULL if not allowed
        internal string PackedDecimalDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x54, 0x54); }
        }

        /* 0x58 - 0x58 */
        // const char *a_dups; // array keyword. the following sequences may appear: #h - header #d - size #v - value #s(b,w,l,q,f,d,o) - size specifiers for byte,word, dword,qword, float,double,oword
        internal string ArrayKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x58, 0x58); }
        }

        /* 0x5C - 0x5C */
        // const char *a_bss;                    // uninitialized data directive should include '%s' for the size of data
        internal string UnitializedDataDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x5C, 0x5C); }
        }

        /* 0x60 - 0x60 */
        // const char *a_equ;                    // 'equ' Used if AS_UNEQU is set
        internal string EquateDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x60, 0x60); }
        }

        /* 0x64 - 0x64 */
        // const char *a_seg;                    // 'seg ' prefix (example: push seg seg001)
        internal string SegmentprefixDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x64, 0x64); }
        }

        //  Pointer to checkarg_dispatch() function. If NULL, checkarg won't be called.
        /* 0x68 - 0x68 */
        // bool (__stdcall* checkarg_dispatch)(void *a1, void *a2, uchar cmd);
        
        /* 0x6C - 0x6C */
        // void *_UNUSED1_was_atomprefix;

        /* 0x70 - 0x70 */
        // void *_UNUSED2_was_checkarg_operations;

        // translation to use in character and string constants. usually 1:1, i.e. trivial translation (may specify NULL)
        /* 0x74 - 0x74 */
        // const byte *XlatAsciiOutput;         // If specified, must be 256 chars long

        /* 0x78 - 0x78 */
        // const char *a_curip;                  // current IP (instruction pointer) symbol in assembler
        internal string CurrentIPSymbol
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x78, 0x78); }
        }

        /* 0x7C - 0x7C */
        // void (__stdcall *func_header)(func_t *); // generate function header lines if NULL, then function headers are displayed as normal lines

        /* 0x80 - 0x80 */
        // void (__stdcall *func_footer)(func_t *); // generate function footer lines if NULL, then a comment line is displayed

        /* 0x84 - 0x84 */
        // const char *a_public;                 // "public" name keyword. NULL-gen default, ""-do not generate
        internal string PublicNameKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x84, 0x84); }
        }

        /* 0x88 - 0x88 */
        // const char *a_weak;                   // "weak"   name keyword. NULL-gen default, ""-do not generate
        internal string WeakNameKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x88, 0x88); }
        }

        /* 0x8C - 0x8C */
        // const char *a_extrn;                  // "extrn"  name keyword
        internal string ExternalNameKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x8C, 0x8C); }
        }

        /* 0x90 - 0x90 */
        // const char *a_comdef;                 // "comm" (communal variable)
        internal string CommunalVariableKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x90, 0x90); }
        }

        /* 0x94 - 0x94 */
        // Get name of type of item at ea or id. (i.e. one of: byte,word,dword,near,far,etc...)
        // ssize_t (__stdcall *get_type_name)(flags_t flag, EffectiveAddress ea_or_id, char *buf, size_t bufsize);

        /* 0x98 - 0x98 */
        // const char *a_align;                  // "align" keyword
        internal string AlignKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x98, 0x98); }
        }

        // Left and right braces used in complex expressions
        /* 0x9C - 0x9C */
        // char lbrace;
        internal char LeftBrace
        {
            get { return (char)MarshalingUtils.GetByte(NativePointer, 0x9C, 0x9C); }
        }

        /* 0x9D - 0x9D */
        // char rbrace;
        internal char RightBrace
        {
            get { return (char)MarshalingUtils.GetByte(NativePointer, 0x9D, 0x9D); }
        }

        /* 0x9E - 0x9E */
        // const char *a_mod;    // %  mod     assembler time operation
        internal string ModuloOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0x9E, 0x9E); }
        }

        /* 0xA2 - 0xA2 */
        // const char *a_band;   // &  bit and assembler time operation
        internal string BinaryAndOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xA2, 0xA2); }
        }

        /* 0xA6 - 0xA6 */
        // const char *a_bor;    // |  bit or  assembler time operation
        internal string BinaryOrOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xA6, 0xA6); }
        }

        /* 0xAA - 0xAA */
        // const char *a_xor;    // ^  bit xor assembler time operation
        internal string XorOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xAA, 0xAA); }
        }

        /* 0xAE - 0xAE */
        // const char *a_bnot;   // ~  bit not assembler time operation
        internal string BitNotOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xAE, 0xAE); }
        }

        /* 0xB2 - 0xB2 */
        // const char *a_shl;    // << shift left assembler time operation
        internal string LeftShiftOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xB2, 0xB2); }
        }

        /* 0xB6 - 0xB6 */
        // const char *a_shr;    // >> shift right assembler time operation
        internal string RightShiftOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xB6, 0xB6); }
        }

        /* 0xBA - 0xBA */
        // const char *a_sizeof_fmt; // size of type (format string)
        internal string SizeofOperator
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xBA, 0xBA); }
        }

        /* 0xBE - 0xBE */
        // uint32 flag2;
        internal AssemblerSyntax Syntax
        {
            get { return (AssemblerSyntax)MarshalingUtils.GetUInt(NativePointer, 0xBE, 0xBE); }
        }

        /* 0xC2 - 0xC2 */
        // const char *cmnt2;                    // comment close string (usually NULL) this is used to denote a string which closes comments, for example, if the comments are represented with (* ... *) then cmnt = "(*" and cmnt2 = "*)"
        internal string CommentClosingString
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xC2, 0xC2); }
        }

        /* 0xC6 - 0xC6 */
        // const char *low8;     // low8 operation, should contain %s for the operand
        internal string Low8
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xC6, 0xC6); }
        }

        /* 0xCA - 0xCA */
        // const char *high8;    // high8
        internal string High8
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xCA, 0xCA); }
        }

        /* 0xCE - 0xCE */
        // const char *low16;    // low16
        internal string Low16
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xCE, 0xCE); }
        }

        /* 0xD2 - 0xD2 */
        // const char *high16;   // high16
        internal string High16
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xD2, 0xD2); }
        }
        
        /* 0xD6 - 0xD6 */
        // const char *a_include_fmt;            // the include directive (format string)
        internal string IncludeDirective
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xD6, 0xD6); }
        }
        
        /* 0xDA - 0xDA */
        // const char *a_vstruc_fmt; // if a named item is a structure and displayed in the verbose (multiline) form then display the name as printf(a_strucname_fmt, typename) (for asms with type checking, e.g. tasm ideal)
        internal string VerboseStructureTemplate
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xDA, 0xDA); }
        }

        /* 0xDE - 0xDE */
        // const char *a_3byte;                  // 3-byte data
        internal string ThreeBytesData
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xDE, 0xDE); }
        }

        /* 0xE2 - 0xE2 */
        // const char *a_rva;                    // 'rva' keyword for image based offsets (see nalt.hpp, REFINFO_RVA)
        internal string RvaKeyword
        {
            get { return MarshalingUtils.GetString(NativePointer, 0xE2, 0xE2); }
        }
        #endregion
    }
}
