using System;

namespace IdaNet.IdaInterop
{
    internal static class Intel
    {
        internal static bool is_segreg(IntelRegisters r)
        {
            return r >= IntelRegisters.R_es && r <= IntelRegisters.R_gs;
        }

        internal static bool is_fpureg(IntelRegisters r)
        {
            return r >= IntelRegisters.R_st0 && r <= IntelRegisters.R_st7;
        }
        
        internal static bool is_mmxreg(IntelRegisters r)
        {
            return r >= IntelRegisters.R_mm0 && r <= IntelRegisters.R_mm7;
        }
        
        internal static bool is_xmmreg(IntelRegisters r)
        {
            return r >= IntelRegisters.R_xmm0 && r <= IntelRegisters.R_xmm15;
        }

        internal static bool is_push_ecx(byte b)
        {
            return b == 0x51; // push ecx
        }

        internal static bool is_push_eax(byte b)
        {
            return b == 0x50; // push eax
        }

        internal static bool is_push_edx(byte b)
        {
            return b == 0x52; // push edx
        }

        internal static bool is_push_ebx(byte b)
        {
            return b == 0x53; // push ebx
        }

        internal static bool is_volatile_reg(IntelRegisters r)
        {
            switch (r)
            {
                case IntelRegisters.R_bx:
                case IntelRegisters.R_bp:
                case IntelRegisters.R_si:
                case IntelRegisters.R_di:
                case IntelRegisters.R_r12:
                case IntelRegisters.R_r13:
                case IntelRegisters.R_r14:
                case IntelRegisters.R_r15:
                    return false;
                default:
                    return true;
            }
        }
    }
}
