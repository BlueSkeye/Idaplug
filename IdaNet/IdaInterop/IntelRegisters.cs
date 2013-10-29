using System;

namespace IdaNet.IdaInterop
{
    internal enum IntelRegisters
    {
        R_none = -1,
        R_ax = 0,
        R_cx = 1,
        R_dx = 2,
        R_bx = 3,
        R_sp = 4,
        R_bp = 5,
        R_si = 6,
        R_di = 7,
        R_r8 = 8,
        R_r9 = 9,
        R_r10 = 10,
        R_r11 = 11,
        R_r12 = 12,
        R_r13 = 13,
        R_r14 = 14,
        R_r15 = 15,

        R_al = 16,
        R_cl = 17,
        R_dl = 18,
        R_bl = 19,
        R_ah = 20,
        R_ch = 21,
        R_dh = 22,
        R_bh = 23,

        R_spl = 24,
        R_bpl = 25,
        R_sil = 26,
        R_dil = 27,

        R_ip = 28,

        R_es = 29,    // 0
        R_cs = 30,    // 1
        R_ss = 31,    // 2
        R_ds = 32,    // 3
        R_fs = 33,
        R_gs = 34,

        R_cf = 35,    // main cc's
        R_zf = 36,
        R_sf = 37,
        R_of = 38,

        R_pf = 39,    // additional cc's
        R_af = 40,
        R_tf = 41,
        R_if = 42,
        R_df = 43,

        R_efl = 44,   // eflags

        // the following registers will be used in the disassembly
        // starting from ida v5.7

        R_st0 = 45,   // floating point registers (not used in disassembly)
        R_st1 = 46,
        R_st2 = 47,
        R_st3 = 48,
        R_st4 = 49,
        R_st5 = 50,
        R_st6 = 51,
        R_st7 = 52,
        R_fpctrl = 53,// fpu control register
        R_fpstat = 54,// fpu status register
        R_fptags = 55,// fpu tags register

        R_mm0 = 56,   // mmx registers (not used in disassembly)
        R_mm1 = 57,
        R_mm2 = 58,
        R_mm3 = 59,
        R_mm4 = 60,
        R_mm5 = 61,
        R_mm6 = 62,
        R_mm7 = 63,

        R_xmm0 = 64,  // xmm registers (not used in disassembly)
        R_xmm1 = 65,
        R_xmm2 = 66,
        R_xmm3 = 67,
        R_xmm4 = 68,
        R_xmm5 = 69,
        R_xmm6 = 70,
        R_xmm7 = 71,
        R_xmm8 = 72,
        R_xmm9 = 73,
        R_xmm10 = 74,
        R_xmm11 = 75,
        R_xmm12 = 76,
        R_xmm13 = 77,
        R_xmm14 = 78,
        R_xmm15 = 79,
        R_mxcsr = 80,
    }
}
