using System;

namespace IdaNet.IdaInterop
{
    internal enum OperandValueType : byte
    {
        dt_byte = 0,       // 8 bit
        dt_word = 1,       // 16 bit
        dt_dword = 2,       // 32 bit
        dt_float = 3,       // 4 byte
        dt_double = 4,       // 8 byte
        dt_tbyte = 5,       // variable size (ph.tbyte_size)
        dt_packreal = 6,       // packed real format for mc68040
        // ...to here the order should not be changed, see mc68000
        dt_qword = 7,       // 64 bit
        dt_byte16 = 8,       // 128 bit
        dt_code = 9,      // ptr to code (not used?)
        dt_void = 10,      // none
        dt_fword = 11,      // 48 bit
        dt_bitfild = 12,      // bit field (mc680x0)
        dt_string = 13,      // pointer to asciiz string
        dt_unicode = 14,      // pointer to unicode string
        dt_3byte = 15,      // 3-byte data
        dt_ldbl = 16,      // long double (which may be different from tbyte)
    }
}
