using System;

namespace IdaNet.IdaInterop
{
    internal enum IntelInstructionCodes : ushort
    {
        NN_null = 0,            // Unknown Operation
        NN_aaa,                 // ASCII Adjust after Addition
        NN_aad,                 // ASCII Adjust AX before Division
        NN_aam,                 // ASCII Adjust AX after Multiply
        NN_aas,                 // ASCII Adjust AL after Subtraction
        NN_adc,                 // Add with Carry
        NN_add,                 // Add
        NN_and,                 // Logical AND
        NN_arpl,                // Adjust RPL Field of Selector
        NN_bound,               // Check Array Index Against Bounds
        NN_bsf,                 // Bit Scan Forward
        NN_bsr,                 // Bit Scan Reverse
        NN_bt,                  // Bit Test
        NN_btc,                 // Bit Test and Complement
        NN_btr,                 // Bit Test and Reset
        NN_bts,                 // Bit Test and Set
        NN_call,                // Call Procedure
        NN_callfi,              // Indirect Call Far Procedure
        NN_callni,              // Indirect Call Near Procedure
        NN_cbw,                 // AL -> AX (with sign)
        NN_cwde,                // AX -> EAX (with sign)
        NN_cdqe,                // EAX -> RAX (with sign)
        NN_clc,                 // Clear Carry Flag
        NN_cld,                 // Clear Direction Flag
        NN_cli,                 // Clear Interrupt Flag
        NN_clts,                // Clear Task-Switched Flag in CR0
        NN_cmc,                 // Complement Carry Flag
        NN_cmp,                 // Compare Two Operands
        NN_cmps,                // Compare Strings
        NN_cwd,                 // AX -> DX:AX (with sign)
        NN_cdq,                 // EAX -> EDX:EAX (with sign)
        NN_cqo,                 // RAX -> RDX:RAX (with sign)
        NN_daa,                 // Decimal Adjust AL after Addition
        NN_das,                 // Decimal Adjust AL after Subtraction
        NN_dec,                 // Decrement by 1
        NN_div,                 // Unsigned Divide
        NN_enterw,              // Make Stack Frame for Procedure Parameters
        NN_enter,               // Make Stack Frame for Procedure Parameters
        NN_enterd,              // Make Stack Frame for Procedure Parameters
        NN_enterq,              // Make Stack Frame for Procedure Parameters
        NN_hlt,                 // Halt
        NN_idiv,                // Signed Divide
        NN_imul,                // Signed Multiply
        NN_in,                  // Input from Port
        NN_inc,                 // Increment by 1
        NN_ins,                 // Input Byte(s) from Port to String
        NN_int,                 // Call to Interrupt Procedure
        NN_into,                // Call to Interrupt Procedure if Overflow Flag = 1
        NN_int3,                // Trap to Debugger
        NN_iretw,               // Interrupt Return
        NN_iret,                // Interrupt Return
        NN_iretd,               // Interrupt Return (use32)
        NN_iretq,               // Interrupt Return (use64)
        NN_ja,                  // Jump if Above (CF=0 & ZF=0)
        NN_jae,                 // Jump if Above or Equal (CF=0)
        NN_jb,                  // Jump if Below (CF=1)
        NN_jbe,                 // Jump if Below or Equal (CF=1 | ZF=1)
        NN_jc,                  // Jump if Carry (CF=1)
        NN_jcxz,                // Jump if CX is 0
        NN_jecxz,               // Jump if ECX is 0
        NN_jrcxz,               // Jump if RCX is 0
        NN_je,                  // Jump if Equal (ZF=1)
        NN_jg,                  // Jump if Greater (ZF=0 & SF=OF)
        NN_jge,                 // Jump if Greater or Equal (SF=OF)
        NN_jl,                  // Jump if Less (SF!=OF)
        NN_jle,                 // Jump if Less or Equal (ZF=1 | SF!=OF)
        NN_jna,                 // Jump if Not Above (CF=1 | ZF=1)
        NN_jnae,                // Jump if Not Above or Equal (CF=1)
        NN_jnb,                 // Jump if Not Below (CF=0)
        NN_jnbe,                // Jump if Not Below or Equal (CF=0 & ZF=0)
        NN_jnc,                 // Jump if Not Carry (CF=0)
        NN_jne,                 // Jump if Not Equal (ZF=0)
        NN_jng,                 // Jump if Not Greater (ZF=1 | SF!=OF)
        NN_jnge,                // Jump if Not Greater or Equal (ZF=1)
        NN_jnl,                 // Jump if Not Less (SF=OF)
        NN_jnle,                // Jump if Not Less or Equal (ZF=0 & SF=OF)
        NN_jno,                 // Jump if Not Overflow (OF=0)
        NN_jnp,                 // Jump if Not Parity (PF=0)
        NN_jns,                 // Jump if Not Sign (SF=0)
        NN_jnz,                 // Jump if Not Zero (ZF=0)
        NN_jo,                  // Jump if Overflow (OF=1)
        NN_jp,                  // Jump if Parity (PF=1)
        NN_jpe,                 // Jump if Parity Even (PF=1)
        NN_jpo,                 // Jump if Parity Odd  (PF=0)
        NN_js,                  // Jump if Sign (SF=1)
        NN_jz,                  // Jump if Zero (ZF=1)
        NN_jmp,                 // Jump
        NN_jmpfi,               // Indirect Far Jump
        NN_jmpni,               // Indirect Near Jump
        NN_jmpshort,            // Jump Short (not used)
        NN_lahf,                // Load Flags into AH Register
        NN_lar,                 // Load Access Right Byte
        NN_lea,                 // Load Effective Address
        NN_leavew,              // High Level Procedure Exit
        NN_leave,               // High Level Procedure Exit
        NN_leaved,              // High Level Procedure Exit
        NN_leaveq,              // High Level Procedure Exit
        NN_lgdt,                // Load Global Descriptor Table Register
        NN_lidt,                // Load Interrupt Descriptor Table Register
        NN_lgs,                 // Load Full Pointer to GS:xx
        NN_lss,                 // Load Full Pointer to SS:xx
        NN_lds,                 // Load Full Pointer to DS:xx
        NN_les,                 // Load Full Pointer to ES:xx
        NN_lfs,                 // Load Full Pointer to FS:xx
        NN_lldt,                // Load Local Descriptor Table Register
        NN_lmsw,                // Load Machine Status Word
        NN_lock,                // Assert LOCK# Signal Prefix
        NN_lods,                // Load String
        NN_loopw,               // Loop while ECX != 0
        NN_loop,                // Loop while CX != 0
        NN_loopd,               // Loop while ECX != 0
        NN_loopq,               // Loop while RCX != 0
        NN_loopwe,              // Loop while CX != 0 and ZF=1
        NN_loope,               // Loop while rCX != 0 and ZF=1
        NN_loopde,              // Loop while ECX != 0 and ZF=1
        NN_loopqe,              // Loop while RCX != 0 and ZF=1
        NN_loopwne,             // Loop while CX != 0 and ZF=0
        NN_loopne,              // Loop while rCX != 0 and ZF=0
        NN_loopdne,             // Loop while ECX != 0 and ZF=0
        NN_loopqne,             // Loop while RCX != 0 and ZF=0
        NN_lsl,                 // Load Segment Limit
        NN_ltr,                 // Load Task Register
        NN_mov,                 // Move Data
        NN_movsp,               // Move to/from Special Registers
        NN_movs,                // Move Byte(s) from String to String
        NN_movsx,               // Move with Sign-Extend
        NN_movzx,               // Move with Zero-Extend
        NN_mul,                 // Unsigned Multiplication of AL or AX
        NN_neg,                 // Two's Complement Negation
        NN_nop,                 // No Operation
        NN_not,                 // One's Complement Negation
        NN_or,                  // Logical Inclusive OR
        NN_out,                 // Output to Port
        NN_outs,                // Output Byte(s) to Port
        NN_pop,                 // Pop a word from the Stack
        NN_popaw,               // Pop all General Registers
        NN_popa,                // Pop all General Registers
        NN_popad,               // Pop all General Registers (use32)
        NN_popaq,               // Pop all General Registers (use64)
        NN_popfw,               // Pop Stack into Flags Register
        NN_popf,                // Pop Stack into Flags Register
        NN_popfd,               // Pop Stack into Eflags Register
        NN_popfq,               // Pop Stack into Rflags Register
        NN_push,                // Push Operand onto the Stack
        NN_pushaw,              // Push all General Registers
        NN_pusha,               // Push all General Registers
        NN_pushad,              // Push all General Registers (use32)
        NN_pushaq,              // Push all General Registers (use64)
        NN_pushfw,              // Push Flags Register onto the Stack
        NN_pushf,               // Push Flags Register onto the Stack
        NN_pushfd,              // Push Flags Register onto the Stack (use32)
        NN_pushfq,              // Push Flags Register onto the Stack (use64)
        NN_rcl,                 // Rotate Through Carry Left
        NN_rcr,                 // Rotate Through Carry Right
        NN_rol,                 // Rotate Left
        NN_ror,                 // Rotate Right
        NN_rep,                 // Repeat String Operation
        NN_repe,                // Repeat String Operation while ZF=1
        NN_repne,               // Repeat String Operation while ZF=0
        NN_retn,                // Return Near from Procedure
        NN_retf,                // Return Far from Procedure
        NN_sahf,                // Store AH into Flags Register
        NN_sal,                 // Shift Arithmetic Left
        NN_sar,                 // Shift Arithmetic Right
        NN_shl,                 // Shift Logical Left
        NN_shr,                 // Shift Logical Right
        NN_sbb,                 // Integer Subtraction with Borrow
        NN_scas,                // Compare String
        NN_seta,                // Set Byte if Above (CF=0 & ZF=0)
        NN_setae,               // Set Byte if Above or Equal (CF=0)
        NN_setb,                // Set Byte if Below (CF=1)
        NN_setbe,               // Set Byte if Below or Equal (CF=1 | ZF=1)
        NN_setc,                // Set Byte if Carry (CF=1)
        NN_sete,                // Set Byte if Equal (ZF=1)
        NN_setg,                // Set Byte if Greater (ZF=0 & SF=OF)
        NN_setge,               // Set Byte if Greater or Equal (SF=OF)
        NN_setl,                // Set Byte if Less (SF!=OF)
        NN_setle,               // Set Byte if Less or Equal (ZF=1 | SF!=OF)
        NN_setna,               // Set Byte if Not Above (CF=1 | ZF=1)
        NN_setnae,              // Set Byte if Not Above or Equal (CF=1)
        NN_setnb,               // Set Byte if Not Below (CF=0)
        NN_setnbe,              // Set Byte if Not Below or Equal (CF=0 & ZF=0)
        NN_setnc,               // Set Byte if Not Carry (CF=0)
        NN_setne,               // Set Byte if Not Equal (ZF=0)
        NN_setng,               // Set Byte if Not Greater (ZF=1 | SF!=OF)
        NN_setnge,              // Set Byte if Not Greater or Equal (ZF=1)
        NN_setnl,               // Set Byte if Not Less (SF=OF)
        NN_setnle,              // Set Byte if Not Less or Equal (ZF=0 & SF=OF)
        NN_setno,               // Set Byte if Not Overflow (OF=0)
        NN_setnp,               // Set Byte if Not Parity (PF=0)
        NN_setns,               // Set Byte if Not Sign (SF=0)
        NN_setnz,               // Set Byte if Not Zero (ZF=0)
        NN_seto,                // Set Byte if Overflow (OF=1)
        NN_setp,                // Set Byte if Parity (PF=1)
        NN_setpe,               // Set Byte if Parity Even (PF=1)
        NN_setpo,               // Set Byte if Parity Odd  (PF=0)
        NN_sets,                // Set Byte if Sign (SF=1)
        NN_setz,                // Set Byte if Zero (ZF=1)
        NN_sgdt,                // Store Global Descriptor Table Register
        NN_sidt,                // Store Interrupt Descriptor Table Register
        NN_shld,                // Double Precision Shift Left
        NN_shrd,                // Double Precision Shift Right
        NN_sldt,                // Store Local Descriptor Table Register
        NN_smsw,                // Store Machine Status Word
        NN_stc,                 // Set Carry Flag
        NN_std,                 // Set Direction Flag
        NN_sti,                 // Set Interrupt Flag
        NN_stos,                // Store String
        NN_str,                 // Store Task Register
        NN_sub,                 // Integer Subtraction
        NN_test,                // Logical Compare
        NN_verr,                // Verify a Segment for Reading
        NN_verw,                // Verify a Segment for Writing
        NN_wait,                // Wait until BUSY# Pin is Inactive (HIGH)
        NN_xchg,                // Exchange Register/Memory with Register
        NN_xlat,                // Table Lookup Translation
        NN_xor,                 // Logical Exclusive OR

        //
        //      486 instructions
        //

        NN_cmpxchg,             // Compare and Exchange
        NN_bswap,               // Swap bits in EAX
        NN_xadd,                // t<-dest; dest<-src+dest; src<-t
        NN_invd,                // Invalidate Data Cache
        NN_wbinvd,              // Invalidate Data Cache (write changes)
        NN_invlpg,              // Invalidate TLB entry

        //
        //      Pentium instructions
        //

        NN_rdmsr,               // Read Machine Status Register
        NN_wrmsr,               // Write Machine Status Register
        NN_cpuid,               // Get CPU ID
        NN_cmpxchg8b,           // Compare and Exchange Eight Bytes
        NN_rdtsc,               // Read Time Stamp Counter
        NN_rsm,                 // Resume from System Management Mode

        //
        //      Pentium Pro instructions
        //

        NN_cmova,               // Move if Above (CF=0 & ZF=0)
        NN_cmovb,               // Move if Below (CF=1)
        NN_cmovbe,              // Move if Below or Equal (CF=1 | ZF=1)
        NN_cmovg,               // Move if Greater (ZF=0 & SF=OF)
        NN_cmovge,              // Move if Greater or Equal (SF=OF)
        NN_cmovl,               // Move if Less (SF!=OF)
        NN_cmovle,              // Move if Less or Equal (ZF=1 | SF!=OF)
        NN_cmovnb,              // Move if Not Below (CF=0)
        NN_cmovno,              // Move if Not Overflow (OF=0)
        NN_cmovnp,              // Move if Not Parity (PF=0)
        NN_cmovns,              // Move if Not Sign (SF=0)
        NN_cmovnz,              // Move if Not Zero (ZF=0)
        NN_cmovo,               // Move if Overflow (OF=1)
        NN_cmovp,               // Move if Parity (PF=1)
        NN_cmovs,               // Move if Sign (SF=1)
        NN_cmovz,               // Move if Zero (ZF=1)
        NN_fcmovb,              // Floating Move if Below
        NN_fcmove,              // Floating Move if Equal
        NN_fcmovbe,             // Floating Move if Below or Equal
        NN_fcmovu,              // Floating Move if Unordered
        NN_fcmovnb,             // Floating Move if Not Below
        NN_fcmovne,             // Floating Move if Not Equal
        NN_fcmovnbe,            // Floating Move if Not Below or Equal
        NN_fcmovnu,             // Floating Move if Not Unordered
        NN_fcomi,               // FP Compare, result in EFLAGS
        NN_fucomi,              // FP Unordered Compare, result in EFLAGS
        NN_fcomip,              // FP Compare, result in EFLAGS, pop stack
        NN_fucomip,             // FP Unordered Compare, result in EFLAGS, pop stack
        NN_rdpmc,               // Read Performance Monitor Counter

        //
        //      FPP instructuions
        //

        NN_fld,                 // Load Real
        NN_fst,                 // Store Real
        NN_fstp,                // Store Real and Pop
        NN_fxch,                // Exchange Registers
        NN_fild,                // Load Integer
        NN_fist,                // Store Integer
        NN_fistp,               // Store Integer and Pop
        NN_fbld,                // Load BCD
        NN_fbstp,               // Store BCD and Pop
        NN_fadd,                // Add Real
        NN_faddp,               // Add Real and Pop
        NN_fiadd,               // Add Integer
        NN_fsub,                // Subtract Real
        NN_fsubp,               // Subtract Real and Pop
        NN_fisub,               // Subtract Integer
        NN_fsubr,               // Subtract Real Reversed
        NN_fsubrp,              // Subtract Real Reversed and Pop
        NN_fisubr,              // Subtract Integer Reversed
        NN_fmul,                // Multiply Real
        NN_fmulp,               // Multiply Real and Pop
        NN_fimul,               // Multiply Integer
        NN_fdiv,                // Divide Real
        NN_fdivp,               // Divide Real and Pop
        NN_fidiv,               // Divide Integer
        NN_fdivr,               // Divide Real Reversed
        NN_fdivrp,              // Divide Real Reversed and Pop
        NN_fidivr,              // Divide Integer Reversed
        NN_fsqrt,               // Square Root
        NN_fscale,              // Scale:  st(0) <- st(0) * 2^st(1)
        NN_fprem,               // Partial Remainder
        NN_frndint,             // Round to Integer
        NN_fxtract,             // Extract exponent and significand
        NN_fabs,                // Absolute value
        NN_fchs,                // Change Sign
        NN_fcom,                // Compare Real
        NN_fcomp,               // Compare Real and Pop
        NN_fcompp,              // Compare Real and Pop Twice
        NN_ficom,               // Compare Integer
        NN_ficomp,              // Compare Integer and Pop
        NN_ftst,                // Test
        NN_fxam,                // Examine
        NN_fptan,               // Partial tangent
        NN_fpatan,              // Partial arctangent
        NN_f2xm1,               // 2^x - 1
        NN_fyl2x,               // Y * lg2(X)
        NN_fyl2xp1,             // Y * lg2(X+1)
        NN_fldz,                // Load +0.0
        NN_fld1,                // Load +1.0
        NN_fldpi,               // Load PI=3.14...
        NN_fldl2t,              // Load lg2(10)
        NN_fldl2e,              // Load lg2(e)
        NN_fldlg2,              // Load lg10(2)
        NN_fldln2,              // Load ln(2)
        NN_finit,               // Initialize Processor
        NN_fninit,              // Initialize Processor (no wait)
        NN_fsetpm,              // Set Protected Mode
        NN_fldcw,               // Load Control Word
        NN_fstcw,               // Store Control Word
        NN_fnstcw,              // Store Control Word (no wait)
        NN_fstsw,               // Store Status Word
        NN_fnstsw,              // Store Status Word (no wait)
        NN_fclex,               // Clear Exceptions
        NN_fnclex,              // Clear Exceptions (no wait)
        NN_fstenv,              // Store Environment
        NN_fnstenv,             // Store Environment (no wait)
        NN_fldenv,              // Load Environment
        NN_fsave,               // Save State
        NN_fnsave,              // Save State (no wait)
        NN_frstor,              // Restore State
        NN_fincstp,             // Increment Stack Pointer
        NN_fdecstp,             // Decrement Stack Pointer
        NN_ffree,               // Free Register
        NN_fnop,                // No Operation
        NN_feni,                // (8087 only)
        NN_fneni,               // (no wait) (8087 only)
        NN_fdisi,               // (8087 only)
        NN_fndisi,              // (no wait) (8087 only)

        //
        //      80387 instructions
        //

        NN_fprem1,              // Partial Remainder ( < half )
        NN_fsincos,             // t<-cos(st); st<-sin(st); push t
        NN_fsin,                // Sine
        NN_fcos,                // Cosine
        NN_fucom,               // Compare Unordered Real
        NN_fucomp,              // Compare Unordered Real and Pop
        NN_fucompp,             // Compare Unordered Real and Pop Twice

        //
        //      Instructions added 28.02.96
        //

        NN_setalc,              // Set AL to Carry Flag
        NN_svdc,                // Save Register and Descriptor
        NN_rsdc,                // Restore Register and Descriptor
        NN_svldt,               // Save LDTR and Descriptor
        NN_rsldt,               // Restore LDTR and Descriptor
        NN_svts,                // Save TR and Descriptor
        NN_rsts,                // Restore TR and Descriptor
        NN_icebp,               // ICE Break Point
        NN_loadall,             // Load the entire CPU state from ES:EDI

        //
        //      MMX instructions
        //

        NN_emms,                // Empty MMX state
        NN_movd,                // Move 32 bits
        NN_movq,                // Move 64 bits
        NN_packsswb,            // Pack with Signed Saturation (Word->Byte)
        NN_packssdw,            // Pack with Signed Saturation (Dword->Word)
        NN_packuswb,            // Pack with Unsigned Saturation (Word->Byte)
        NN_paddb,               // Packed Add Byte
        NN_paddw,               // Packed Add Word
        NN_paddd,               // Packed Add Dword
        NN_paddsb,              // Packed Add with Saturation (Byte)
        NN_paddsw,              // Packed Add with Saturation (Word)
        NN_paddusb,             // Packed Add Unsigned with Saturation (Byte)
        NN_paddusw,             // Packed Add Unsigned with Saturation (Word)
        NN_pand,                // Bitwise Logical And
        NN_pandn,               // Bitwise Logical And Not
        NN_pcmpeqb,             // Packed Compare for Equal (Byte)
        NN_pcmpeqw,             // Packed Compare for Equal (Word)
        NN_pcmpeqd,             // Packed Compare for Equal (Dword)
        NN_pcmpgtb,             // Packed Compare for Greater Than (Byte)
        NN_pcmpgtw,             // Packed Compare for Greater Than (Word)
        NN_pcmpgtd,             // Packed Compare for Greater Than (Dword)
        NN_pmaddwd,             // Packed Multiply and Add
        NN_pmulhw,              // Packed Multiply High
        NN_pmullw,              // Packed Multiply Low
        NN_por,                 // Bitwise Logical Or
        NN_psllw,               // Packed Shift Left Logical (Word)
        NN_pslld,               // Packed Shift Left Logical (Dword)
        NN_psllq,               // Packed Shift Left Logical (Qword)
        NN_psraw,               // Packed Shift Right Arithmetic (Word)
        NN_psrad,               // Packed Shift Right Arithmetic (Dword)
        NN_psrlw,               // Packed Shift Right Logical (Word)
        NN_psrld,               // Packed Shift Right Logical (Dword)
        NN_psrlq,               // Packed Shift Right Logical (Qword)
        NN_psubb,               // Packed Subtract Byte
        NN_psubw,               // Packed Subtract Word
        NN_psubd,               // Packed Subtract Dword
        NN_psubsb,              // Packed Subtract with Saturation (Byte)
        NN_psubsw,              // Packed Subtract with Saturation (Word)
        NN_psubusb,             // Packed Subtract Unsigned with Saturation (Byte)
        NN_psubusw,             // Packed Subtract Unsigned with Saturation (Word)
        NN_punpckhbw,           // Unpack High Packed Data (Byte->Word)
        NN_punpckhwd,           // Unpack High Packed Data (Word->Dword)
        NN_punpckhdq,           // Unpack High Packed Data (Dword->Qword)
        NN_punpcklbw,           // Unpack Low Packed Data (Byte->Word)
        NN_punpcklwd,           // Unpack Low Packed Data (Word->Dword)
        NN_punpckldq,           // Unpack Low Packed Data (Dword->Qword)
        NN_pxor,                // Bitwise Logical Exclusive Or

        //
        //      Undocumented Deschutes processor instructions
        //

        NN_fxsave,              // Fast save FP context
        NN_fxrstor,             // Fast restore FP context

        //      Pentium II instructions

        NN_sysenter,            // Fast Transition to System Call Entry Point
        NN_sysexit,             // Fast Transition from System Call Entry Point

        //      3DNow! instructions

        NN_pavgusb,             // Packed 8-bit Unsigned Integer Averaging
        NN_pfadd,               // Packed Floating-Point Addition
        NN_pfsub,               // Packed Floating-Point Subtraction
        NN_pfsubr,              // Packed Floating-Point Reverse Subtraction
        NN_pfacc,               // Packed Floating-Point Accumulate
        NN_pfcmpge,             // Packed Floating-Point Comparison, Greater or Equal
        NN_pfcmpgt,             // Packed Floating-Point Comparison, Greater
        NN_pfcmpeq,             // Packed Floating-Point Comparison, Equal
        NN_pfmin,               // Packed Floating-Point Minimum
        NN_pfmax,               // Packed Floating-Point Maximum
        NN_pi2fd,               // Packed 32-bit Integer to Floating-Point
        NN_pf2id,               // Packed Floating-Point to 32-bit Integer
        NN_pfrcp,               // Packed Floating-Point Reciprocal Approximation
        NN_pfrsqrt,             // Packed Floating-Point Reciprocal Square Root Approximation
        NN_pfmul,               // Packed Floating-Point Multiplication
        NN_pfrcpit1,            // Packed Floating-Point Reciprocal First Iteration Step
        NN_pfrsqit1,            // Packed Floating-Point Reciprocal Square Root First Iteration Step
        NN_pfrcpit2,            // Packed Floating-Point Reciprocal Second Iteration Step
        NN_pmulhrw,             // Packed Floating-Point 16-bit Integer Multiply with rounding
        NN_femms,               // Faster entry/exit of the MMX or floating-point state
        NN_prefetch,            // Prefetch at least a 32-byte line into L1 data cache
        NN_prefetchw,           // Prefetch processor cache line into L1 data cache (mark as modified)


        //      Pentium III instructions

        NN_addps,               // Packed Single-FP Add
        NN_addss,               // Scalar Single-FP Add
        NN_andnps,              // Bitwise Logical And Not for Single-FP
        NN_andps,               // Bitwise Logical And for Single-FP
        NN_cmpps,               // Packed Single-FP Compare
        NN_cmpss,               // Scalar Single-FP Compare
        NN_comiss,              // Scalar Ordered Single-FP Compare and Set EFLAGS
        NN_cvtpi2ps,            // Packed signed INT32 to Packed Single-FP conversion
        NN_cvtps2pi,            // Packed Single-FP to Packed INT32 conversion
        NN_cvtsi2ss,            // Scalar signed INT32 to Single-FP conversion
        NN_cvtss2si,            // Scalar Single-FP to signed INT32 conversion
        NN_cvttps2pi,           // Packed Single-FP to Packed INT32 conversion (truncate)
        NN_cvttss2si,           // Scalar Single-FP to signed INT32 conversion (truncate)
        NN_divps,               // Packed Single-FP Divide
        NN_divss,               // Scalar Single-FP Divide
        NN_ldmxcsr,             // Load Streaming SIMD Extensions Technology Control/Status Register
        NN_maxps,               // Packed Single-FP Maximum
        NN_maxss,               // Scalar Single-FP Maximum
        NN_minps,               // Packed Single-FP Minimum
        NN_minss,               // Scalar Single-FP Minimum
        NN_movaps,              // Move Aligned Four Packed Single-FP
        NN_movhlps,             // Move High to Low Packed Single-FP
        NN_movhps,              // Move High Packed Single-FP
        NN_movlhps,             // Move Low to High Packed Single-FP
        NN_movlps,              // Move Low Packed Single-FP
        NN_movmskps,            // Move Mask to Register
        NN_movss,               // Move Scalar Single-FP
        NN_movups,              // Move Unaligned Four Packed Single-FP
        NN_mulps,               // Packed Single-FP Multiply
        NN_mulss,               // Scalar Single-FP Multiply
        NN_orps,                // Bitwise Logical OR for Single-FP Data
        NN_rcpps,               // Packed Single-FP Reciprocal
        NN_rcpss,               // Scalar Single-FP Reciprocal
        NN_rsqrtps,             // Packed Single-FP Square Root Reciprocal
        NN_rsqrtss,             // Scalar Single-FP Square Root Reciprocal
        NN_shufps,              // Shuffle Single-FP
        NN_sqrtps,              // Packed Single-FP Square Root
        NN_sqrtss,              // Scalar Single-FP Square Root
        NN_stmxcsr,             // Store Streaming SIMD Extensions Technology Control/Status Register
        NN_subps,               // Packed Single-FP Subtract
        NN_subss,               // Scalar Single-FP Subtract
        NN_ucomiss,             // Scalar Unordered Single-FP Compare and Set EFLAGS
        NN_unpckhps,            // Unpack High Packed Single-FP Data
        NN_unpcklps,            // Unpack Low Packed Single-FP Data
        NN_xorps,               // Bitwise Logical XOR for Single-FP Data
        NN_pavgb,               // Packed Average (Byte)
        NN_pavgw,               // Packed Average (Word)
        NN_pextrw,              // Extract Word
        NN_pinsrw,              // Insert Word
        NN_pmaxsw,              // Packed Signed Integer Word Maximum
        NN_pmaxub,              // Packed Unsigned Integer Byte Maximum
        NN_pminsw,              // Packed Signed Integer Word Minimum
        NN_pminub,              // Packed Unsigned Integer Byte Minimum
        NN_pmovmskb,            // Move Byte Mask to Integer
        NN_pmulhuw,             // Packed Multiply High Unsigned
        NN_psadbw,              // Packed Sum of Absolute Differences
        NN_pshufw,              // Packed Shuffle Word
        NN_maskmovq,            // Byte Mask write
        NN_movntps,             // Move Aligned Four Packed Single-FP Non Temporal
        NN_movntq,              // Move 64 Bits Non Temporal
        NN_prefetcht0,          // Prefetch to all cache levels
        NN_prefetcht1,          // Prefetch to all cache levels
        NN_prefetcht2,          // Prefetch to L2 cache
        NN_prefetchnta,         // Prefetch to L1 cache
        NN_sfence,              // Store Fence

        // Pentium III Pseudo instructions

        NN_cmpeqps,             // Packed Single-FP Compare EQ
        NN_cmpltps,             // Packed Single-FP Compare LT
        NN_cmpleps,             // Packed Single-FP Compare LE
        NN_cmpunordps,          // Packed Single-FP Compare UNORD
        NN_cmpneqps,            // Packed Single-FP Compare NOT EQ
        NN_cmpnltps,            // Packed Single-FP Compare NOT LT
        NN_cmpnleps,            // Packed Single-FP Compare NOT LE
        NN_cmpordps,            // Packed Single-FP Compare ORDERED
        NN_cmpeqss,             // Scalar Single-FP Compare EQ
        NN_cmpltss,             // Scalar Single-FP Compare LT
        NN_cmpless,             // Scalar Single-FP Compare LE
        NN_cmpunordss,          // Scalar Single-FP Compare UNORD
        NN_cmpneqss,            // Scalar Single-FP Compare NOT EQ
        NN_cmpnltss,            // Scalar Single-FP Compare NOT LT
        NN_cmpnless,            // Scalar Single-FP Compare NOT LE
        NN_cmpordss,            // Scalar Single-FP Compare ORDERED

        // AMD K7 instructions

        NN_pf2iw,               // Packed Floating-Point to Integer with Sign Extend
        NN_pfnacc,              // Packed Floating-Point Negative Accumulate
        NN_pfpnacc,             // Packed Floating-Point Mixed Positive-Negative Accumulate
        NN_pi2fw,               // Packed 16-bit Integer to Floating-Point
        NN_pswapd,              // Packed Swap Double Word

        // Undocumented FP instructions (thanks to norbert.juffa@adm.com)

        NN_fstp1,               // Alias of Store Real and Pop
        NN_fcom2,               // Alias of Compare Real
        NN_fcomp3,              // Alias of Compare Real and Pop
        NN_fxch4,               // Alias of Exchange Registers
        NN_fcomp5,              // Alias of Compare Real and Pop
        NN_ffreep,              // Free Register and Pop
        NN_fxch7,               // Alias of Exchange Registers
        NN_fstp8,               // Alias of Store Real and Pop
        NN_fstp9,               // Alias of Store Real and Pop

        // Pentium 4 instructions

        NN_addpd,               // Add Packed Double-Precision Floating-Point Values
        NN_addsd,               // Add Scalar Double-Precision Floating-Point Values
        NN_andnpd,              // Bitwise Logical AND NOT of Packed Double-Precision Floating-Point Values
        NN_andpd,               // Bitwise Logical AND of Packed Double-Precision Floating-Point Values
        NN_clflush,             // Flush Cache Line
        NN_cmppd,               // Compare Packed Double-Precision Floating-Point Values
        NN_cmpsd,               // Compare Scalar Double-Precision Floating-Point Values
        NN_comisd,              // Compare Scalar Ordered Double-Precision Floating-Point Values and Set EFLAGS
        NN_cvtdq2pd,            // Convert Packed Doubleword Integers to Packed Single-Precision Floating-Point Values
        NN_cvtdq2ps,            // Convert Packed Doubleword Integers to Packed Double-Precision Floating-Point Values
        NN_cvtpd2dq,            // Convert Packed Double-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvtpd2pi,            // Convert Packed Double-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvtpd2ps,            // Convert Packed Double-Precision Floating-Point Values to Packed Single-Precision Floating-Point Values
        NN_cvtpi2pd,            // Convert Packed Doubleword Integers to Packed Double-Precision Floating-Point Values
        NN_cvtps2dq,            // Convert Packed Single-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvtps2pd,            // Convert Packed Single-Precision Floating-Point Values to Packed Double-Precision Floating-Point Values
        NN_cvtsd2si,            // Convert Scalar Double-Precision Floating-Point Value to Doubleword Integer
        NN_cvtsd2ss,            // Convert Scalar Double-Precision Floating-Point Value to Scalar Single-Precision Floating-Point Value
        NN_cvtsi2sd,            // Convert Doubleword Integer to Scalar Double-Precision Floating-Point Value
        NN_cvtss2sd,            // Convert Scalar Single-Precision Floating-Point Value to Scalar Double-Precision Floating-Point Value
        NN_cvttpd2dq,           // Convert With Truncation Packed Double-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvttpd2pi,           // Convert with Truncation Packed Double-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvttps2dq,           // Convert With Truncation Packed Single-Precision Floating-Point Values to Packed Doubleword Integers
        NN_cvttsd2si,           // Convert with Truncation Scalar Double-Precision Floating-Point Value to Doubleword Integer
        NN_divpd,               // Divide Packed Double-Precision Floating-Point Values
        NN_divsd,               // Divide Scalar Double-Precision Floating-Point Values
        NN_lfence,              // Load Fence
        NN_maskmovdqu,          // Store Selected Bytes of Double Quadword
        NN_maxpd,               // Return Maximum Packed Double-Precision Floating-Point Values
        NN_maxsd,               // Return Maximum Scalar Double-Precision Floating-Point Value
        NN_mfence,              // Memory Fence
        NN_minpd,               // Return Minimum Packed Double-Precision Floating-Point Values
        NN_minsd,               // Return Minimum Scalar Double-Precision Floating-Point Value
        NN_movapd,              // Move Aligned Packed Double-Precision Floating-Point Values
        NN_movdq2q,             // Move Quadword from XMM to MMX Register
        NN_movdqa,              // Move Aligned Double Quadword
        NN_movdqu,              // Move Unaligned Double Quadword
        NN_movhpd,              // Move High Packed Double-Precision Floating-Point Values
        NN_movlpd,              // Move Low Packed Double-Precision Floating-Point Values
        NN_movmskpd,            // Extract Packed Double-Precision Floating-Point Sign Mask
        NN_movntdq,             // Store Double Quadword Using Non-Temporal Hint
        NN_movnti,              // Store Doubleword Using Non-Temporal Hint
        NN_movntpd,             // Store Packed Double-Precision Floating-Point Values Using Non-Temporal Hint
        NN_movq2dq,             // Move Quadword from MMX to XMM Register
        NN_movsd,               // Move Scalar Double-Precision Floating-Point Values
        NN_movupd,              // Move Unaligned Packed Double-Precision Floating-Point Values
        NN_mulpd,               // Multiply Packed Double-Precision Floating-Point Values
        NN_mulsd,               // Multiply Scalar Double-Precision Floating-Point Values
        NN_orpd,                // Bitwise Logical OR of Double-Precision Floating-Point Values
        NN_paddq,               // Add Packed Quadword Integers
        NN_pause,               // Spin Loop Hint
        NN_pmuludq,             // Multiply Packed Unsigned Doubleword Integers
        NN_pshufd,              // Shuffle Packed Doublewords
        NN_pshufhw,             // Shuffle Packed High Words
        NN_pshuflw,             // Shuffle Packed Low Words
        NN_pslldq,              // Shift Double Quadword Left Logical
        NN_psrldq,              // Shift Double Quadword Right Logical
        NN_psubq,               // Subtract Packed Quadword Integers
        NN_punpckhqdq,          // Unpack High Data
        NN_punpcklqdq,          // Unpack Low Data
        NN_shufpd,              // Shuffle Packed Double-Precision Floating-Point Values
        NN_sqrtpd,              // Compute Square Roots of Packed Double-Precision Floating-Point Values
        NN_sqrtsd,              // Compute Square Rootof Scalar Double-Precision Floating-Point Value
        NN_subpd,               // Subtract Packed Double-Precision Floating-Point Values
        NN_subsd,               // Subtract Scalar Double-Precision Floating-Point Values
        NN_ucomisd,             // Unordered Compare Scalar Ordered Double-Precision Floating-Point Values and Set EFLAGS
        NN_unpckhpd,            // Unpack and Interleave High Packed Double-Precision Floating-Point Values
        NN_unpcklpd,            // Unpack and Interleave Low Packed Double-Precision Floating-Point Values
        NN_xorpd,               // Bitwise Logical OR of Double-Precision Floating-Point Values

        // AMD syscall/sysret instructions

        NN_syscall,             // Low latency system call
        NN_sysret,              // Return from system call

        // AMD64 instructions

        NN_swapgs,              // Exchange GS base with KernelGSBase MSR

        // New Pentium instructions (SSE3)

        NN_movddup,             // Move One Double-FP and Duplicate
        NN_movshdup,            // Move Packed Single-FP High and Duplicate
        NN_movsldup,            // Move Packed Single-FP Low and Duplicate

        // Missing AMD64 instructions

        NN_movsxd,              // Move with Sign-Extend Doubleword
        NN_cmpxchg16b,          // Compare and Exchange 16 Bytes

        // SSE3 instructions

        NN_addsubpd,            // Add /Sub packed DP FP numbers
        NN_addsubps,            // Add /Sub packed SP FP numbers
        NN_haddpd,              // Add horizontally packed DP FP numbers
        NN_haddps,              // Add horizontally packed SP FP numbers
        NN_hsubpd,              // Sub horizontally packed DP FP numbers
        NN_hsubps,              // Sub horizontally packed SP FP numbers
        NN_monitor,             // Set up a linear address range to be monitored by hardware
        NN_mwait,               // Wait until write-back store performed within the range specified by the MONITOR instruction
        NN_fisttp,              // Store ST in intXX (chop) and pop
        NN_lddqu,               // Load unaligned integer 128-bit

        // SSSE3 instructions

        NN_psignb,              // Packed SIGN Byte
        NN_psignw,              // Packed SIGN Word
        NN_psignd,              // Packed SIGN Doubleword
        NN_pshufb,              // Packed Shuffle Bytes
        NN_pmulhrsw,            // Packed Multiply High with Round and Scale
        NN_pmaddubsw,           // Multiply and Add Packed Signed and Unsigned Bytes
        NN_phsubsw,             // Packed Horizontal Subtract and Saturate
        NN_phaddsw,             // Packed Horizontal Add and Saturate
        NN_phaddw,              // Packed Horizontal Add Word
        NN_phaddd,              // Packed Horizontal Add Doubleword
        NN_phsubw,              // Packed Horizontal Subtract Word
        NN_phsubd,              // Packed Horizontal Subtract Doubleword
        NN_palignr,             // Packed Align Right
        NN_pabsb,               // Packed Absolute Value Byte
        NN_pabsw,               // Packed Absolute Value Word
        NN_pabsd,               // Packed Absolute Value Doubleword

        // VMX instructions

        NN_vmcall,              // Call to VM Monitor
        NN_vmclear,             // Clear Virtual Machine Control Structure
        NN_vmlaunch,            // Launch Virtual Machine
        NN_vmresume,            // Resume Virtual Machine
        NN_vmptrld,             // Load Pointer to Virtual Machine Control Structure
        NN_vmptrst,             // Store Pointer to Virtual Machine Control Structure
        NN_vmread,              // Read Field from Virtual Machine Control Structure
        NN_vmwrite,             // Write Field from Virtual Machine Control Structure
        NN_vmxoff,              // Leave VMX Operation
        NN_vmxon,               // Enter VMX Operation

        // Undefined Instruction

        NN_ud2,                 // Undefined Instruction

        // Added with x86-64

        NN_rdtscp,              // Read Time-Stamp Counter and Processor ID

        // Geode LX 3DNow! extensions

        NN_pfrcpv,              // Reciprocal Approximation for a Pair of 32-bit Floats
        NN_pfrsqrtv,            // Reciprocal Square Root Approximation for a Pair of 32-bit Floats

        // SSE2 pseudoinstructions

        NN_cmpeqpd,             // Packed Double-FP Compare EQ
        NN_cmpltpd,             // Packed Double-FP Compare LT
        NN_cmplepd,             // Packed Double-FP Compare LE
        NN_cmpunordpd,          // Packed Double-FP Compare UNORD
        NN_cmpneqpd,            // Packed Double-FP Compare NOT EQ
        NN_cmpnltpd,            // Packed Double-FP Compare NOT LT
        NN_cmpnlepd,            // Packed Double-FP Compare NOT LE
        NN_cmpordpd,            // Packed Double-FP Compare ORDERED
        NN_cmpeqsd,             // Scalar Double-FP Compare EQ
        NN_cmpltsd,             // Scalar Double-FP Compare LT
        NN_cmplesd,             // Scalar Double-FP Compare LE
        NN_cmpunordsd,          // Scalar Double-FP Compare UNORD
        NN_cmpneqsd,            // Scalar Double-FP Compare NOT EQ
        NN_cmpnltsd,            // Scalar Double-FP Compare NOT LT
        NN_cmpnlesd,            // Scalar Double-FP Compare NOT LE
        NN_cmpordsd,            // Scalar Double-FP Compare ORDERED

        // SSSE4.1 instructions

        NN_blendpd,              // Blend Packed Double Precision Floating-Point Values
        NN_blendps,              // Blend Packed Single Precision Floating-Point Values
        NN_blendvpd,             // Variable Blend Packed Double Precision Floating-Point Values
        NN_blendvps,             // Variable Blend Packed Single Precision Floating-Point Values
        NN_dppd,                 // Dot Product of Packed Double Precision Floating-Point Values
        NN_dpps,                 // Dot Product of Packed Single Precision Floating-Point Values
        NN_extractps,            // Extract Packed Single Precision Floating-Point Value
        NN_insertps,             // Insert Packed Single Precision Floating-Point Value
        NN_movntdqa,             // Load Double Quadword Non-Temporal Aligned Hint
        NN_mpsadbw,              // Compute Multiple Packed Sums of Absolute Difference
        NN_packusdw,             // Pack with Unsigned Saturation
        NN_pblendvb,             // Variable Blend Packed Bytes
        NN_pblendw,              // Blend Packed Words
        NN_pcmpeqq,              // Compare Packed Qword Data for Equal
        NN_pextrb,               // Extract Byte
        NN_pextrd,               // Extract Dword
        NN_pextrq,               // Extract Qword
        NN_phminposuw,           // Packed Horizontal Word Minimum
        NN_pinsrb,               // Insert Byte
        NN_pinsrd,               // Insert Dword
        NN_pinsrq,               // Insert Qword
        NN_pmaxsb,               // Maximum of Packed Signed Byte Integers
        NN_pmaxsd,               // Maximum of Packed Signed Dword Integers
        NN_pmaxud,               // Maximum of Packed Unsigned Dword Integers
        NN_pmaxuw,               // Maximum of Packed Word Integers
        NN_pminsb,               // Minimum of Packed Signed Byte Integers
        NN_pminsd,               // Minimum of Packed Signed Dword Integers
        NN_pminud,               // Minimum of Packed Unsigned Dword Integers
        NN_pminuw,               // Minimum of Packed Word Integers
        NN_pmovsxbw,             // Packed Move with Sign Extend
        NN_pmovsxbd,             // Packed Move with Sign Extend
        NN_pmovsxbq,             // Packed Move with Sign Extend
        NN_pmovsxwd,             // Packed Move with Sign Extend
        NN_pmovsxwq,             // Packed Move with Sign Extend
        NN_pmovsxdq,             // Packed Move with Sign Extend
        NN_pmovzxbw,             // Packed Move with Zero Extend
        NN_pmovzxbd,             // Packed Move with Zero Extend
        NN_pmovzxbq,             // Packed Move with Zero Extend
        NN_pmovzxwd,             // Packed Move with Zero Extend
        NN_pmovzxwq,             // Packed Move with Zero Extend
        NN_pmovzxdq,             // Packed Move with Zero Extend
        NN_pmuldq,               // Multiply Packed Signed Dword Integers
        NN_pmulld,               // Multiply Packed Signed Dword Integers and Store Low Result
        NN_ptest,                // Logical Compare
        NN_roundpd,              // Round Packed Double Precision Floating-Point Values
        NN_roundps,              // Round Packed Single Precision Floating-Point Values
        NN_roundsd,              // Round Scalar Double Precision Floating-Point Values
        NN_roundss,              // Round Scalar Single Precision Floating-Point Values

        // SSSE4.2 instructions

        NN_crc32,                // Accumulate CRC32 Value
        NN_pcmpestri,            // Packed Compare Explicit Length Strings, Return Index
        NN_pcmpestrm,            // Packed Compare Explicit Length Strings, Return Mask
        NN_pcmpistri,            // Packed Compare Implicit Length Strings, Return Index
        NN_pcmpistrm,            // Packed Compare Implicit Length Strings, Return Mask
        NN_pcmpgtq,              // Compare Packed Data for Greater Than
        NN_popcnt,               // Return the Count of Number of Bits Set to 1

        // AMD SSE4a instructions

        NN_extrq,                // Extract Field From Register
        NN_insertq,              // Insert Field
        NN_movntsd,              // Move Non-Temporal Scalar Double-Precision Floating-Point
        NN_movntss,              // Move Non-Temporal Scalar Single-Precision Floating-Point
        NN_lzcnt,                // Leading Zero Count

        // xsave/xrstor instructions

        NN_xgetbv,               // Get Value of Extended Control Register
        NN_xrstor,               // Restore Processor Extended States
        NN_xsave,                // Save Processor Extended States
        NN_xsetbv,               // Set Value of Extended Control Register

        // Intel Safer Mode Extensions (SMX)

        NN_getsec,               // Safer Mode Extensions (SMX) Instruction

        // AMD-V Virtualization ISA Extension

        NN_clgi,                 // Clear Global Interrupt Flag
        NN_invlpga,              // Invalidate TLB Entry in a Specified ASID
        NN_skinit,               // Secure Init and Jump with Attestation
        NN_stgi,                 // Set Global Interrupt Flag
        NN_vmexit,               // Stop Executing Guest, Begin Executing Host
        NN_vmload,               // Load State from VMCB
        NN_vmmcall,              // Call VMM
        NN_vmrun,                // Run Virtual Machine
        NN_vmsave,               // Save State to VMCB

        // VMX+ instructions

        NN_invept,               // Invalidate Translations Derived from EPT
        NN_invvpid,              // Invalidate Translations Based on VPID

        // Intel Atom instructions

        NN_movbe,                // Move Data After Swapping Bytes

        // Intel AES instructions

        NN_aesenc,                // Perform One Round of an AES Encryption Flow
        NN_aesenclast,            // Perform the Last Round of an AES Encryption Flow
        NN_aesdec,                // Perform One Round of an AES Decryption Flow
        NN_aesdeclast,            // Perform the Last Round of an AES Decryption Flow
        NN_aesimc,                // Perform the AES InvMixColumn Transformation
        NN_aeskeygenassist,       // AES Round Key Generation Assist

        // Carryless multiplication

        NN_pclmulqdq,            // Carry-Less Multiplication Quadword

        // Returns modifies by operand size prefixes

        NN_retnw,               // Return Near from Procedure (use16)
        NN_retnd,               // Return Near from Procedure (use32)
        NN_retnq,               // Return Near from Procedure (use64)
        NN_retfw,               // Return Far from Procedure (use16)
        NN_retfd,               // Return Far from Procedure (use32)
        NN_retfq,               // Return Far from Procedure (use64)

        NN_last,
    }
}
