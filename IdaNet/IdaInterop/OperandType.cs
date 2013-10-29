using System;

namespace IdaNet.IdaInterop
{
    // Type of an operand
    // An operand of an instruction has a type. The kernel knows about
    // some operand types and accordingly interprets some fields of op_t
    // structure. The fields used by the kernel is shown below.
    // There are some IDP specific types (o_idpspec?). You are free to
    // give any meaning to these types. I suggest you to create a #define
    // to use mnemonic names. However, don't forget that the kernel will
    // know nothing about operands of those types.
    // As about "data field", you may use any additional fields to store
    // processor specific operand information
    // How to assign the operand types
    // -------------------------------
    //
    //o_reg    denotes a simple register, the register number should
    //         be stored in x.reg. All processor registers, including special
    //         registers, can be represented by this operand type
    //o_mem    a direct memory data reference whose target address is known at the compliation time.
    //         The target virtual address is stored in x.addr and the full address
    //         is calculated as toEA(cmd.cs, x.addr). For the processors with
    //         complex memory organization the final address can be calculated
    //         using other segment registers. For flat memories, x.addr is the final
    //         address and cmd.cs is usually equal to zero. In any case, the address
    //         within the segment should be stored in x.addr.
    //o_phrase a memory reference using register contents. indexed, register based,
    //         and other addressing modes can be represented with the operand type.
    //         This addressing mode can not contain immediate values (use o_displ for them)
    //         The phrase number should be stored in x.phrase. To denote the preincrement
    //         and similar features please use additional operand fields like specflags.
    //         Usually x.phrase contains the register number and additional information
    //         is stored in x.specflags. Please note that this operand type can not
    //         contain immediate values (except the scaling coefficients)
    //o_displ  a memory reference using register contents with displacement.
    //         The displacement should be stored in the x.addr field. The rest of information
    //         is stored the same way as in o_phrase.
    //o_imm    an immediate value. Any operand consisting of only a number is represented
    //         by this operand type. The value should be stored in x.value. You may sign
    //         extend short (1-2 byte) values. In any case don't forget to specify x.dtyp
    //         (x.dtyp should be set for all operand types)
    //o_near   a direct memory code reference whose target address is known at the compliation time.
    //         The target virtual address is stored in x.addr and the final address
    //         is always toEA(cmd.cs, x.addr). Usually this operand type is used for
    //         the branches and calls whose target address is known. If the current
    //         processor has 2 different types of references for intersegment and intrasegment
    //         references, then o_near should be used only for intrasegment references.
    //o_far    If the current processor has a special addressing mode for intersegment
    //         references, then this operand type should be used instead of o_near.
    //         If you want, you may use PR_CHK_XREF in ph.flag to disable intersegment
    //         calls if o_near operand type is used. Currently only IBM PC uses this flag.
    //
    //      If the above operand types do not cover all possible addressing modes,
    //      then use o_idpspec operand types.
    internal enum OperandType : byte
    {
        o_void     =  0, // No Operand                           ----------
        o_reg      =  1, // General Register (al,ax,es,ds...)    reg
        o_mem      =  2, // Direct Memory Reference  (DATA)      addr
        o_phrase   =  3, // Memory Ref [Base Reg + Index Reg]    phrase
        o_displ    =  4, // Memory Reg [Base Reg + Index Reg + Displacement] phrase+addr
        o_imm      =  5, // Immediate Value                      value
        o_far      =  6, // Immediate Far Address  (CODE)        addr
        o_near     =  7, // Immediate Near Address (CODE)        addr
        o_idpspec0 =  8, // IDP specific type
        o_idpspec1 =  9, // IDP specific type
        o_idpspec2 = 10, // IDP specific type
        o_idpspec3 = 11, // IDP specific type
        o_idpspec4 = 12, // IDP specific type
        o_idpspec5 = 13, // IDP specific type
        o_last     = 14, // first unused type
    }
}
