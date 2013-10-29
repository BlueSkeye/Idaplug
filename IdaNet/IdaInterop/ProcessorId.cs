using System;

namespace IdaNet.IdaInterop
{
    // Numbers above 0x8000 are reserved for the third-party modules
    internal enum ProcessorId
    {
        PLFM_386 = 0,// = Intel = 80x86
        PLFM_Z80 = 1,// = 8085, = Z80
        PLFM_I860 = 2,// = Intel = 860
        PLFM_8051 = 3,// = 8051
        PLFM_TMS = 4,// = Texas = Instruments = TMS320C5x
        PLFM_6502 = 5,// = 6502
        PLFM_PDP = 6,// = PDP11
        PLFM_68K = 7,// = Motoroal = 680x0
        PLFM_JAVA = 8,// = Java
        PLFM_6800 = 9,// = Motorola = 68xx
        PLFM_ST7 = 10,// = SGS-Thomson = ST7
        PLFM_MC6812 = 11,// = Motorola = 68HC12
        PLFM_MIPS = 12,// = MIPS
        PLFM_ARM = 13,// = Advanced = RISC = Machines
        PLFM_TMSC6 = 14,// = Texas = Instruments = TMS320C6x
        PLFM_PPC = 15,// = PowerPC
        PLFM_80196 = 16,// = Intel = 80196
        PLFM_Z8 = 17,// = Z8
        PLFM_SH = 18,// = Renesas = (formerly = Hitachi) = SuperH
        PLFM_NET = 19,// = Microsoft = Visual = Studio.Net
        PLFM_AVR = 20,// = Atmel = 8-bit = RISC = processor(s)
        PLFM_H8 = 21,// = Hitachi = H8/300, = H8/2000
        PLFM_PIC = 22,// = Microchip's = PIC
        PLFM_SPARC = 23,// = SPARC
        PLFM_ALPHA = 24,// = DEC = Alpha
        PLFM_HPPA = 25,// = Hewlett-Packard = PA-RISC
        PLFM_H8500 = 26,// = Hitachi = H8/500
        PLFM_TRICORE = 27,// = Tasking = Tricore
        PLFM_DSP56K = 28,// = Motorola = DSP5600x
        PLFM_C166 = 29,// = Siemens = C166 = family
        PLFM_ST20 = 30,// = SGS-Thomson = ST20
        PLFM_IA64 = 31,// = Intel = Itanium = IA64
        PLFM_I960 = 32,// = Intel = 960
        PLFM_F2MC = 33,// = Fujistu = F2MC-16
        PLFM_TMS320C54 = 34,// = Texas = Instruments = TMS320C54xx
        PLFM_TMS320C55 = 35,// = Texas = Instruments = TMS320C55xx
        PLFM_TRIMEDIA = 36,// = Trimedia
        PLFM_M32R = 37,// = Mitsubishi = 32bit = RISC
        PLFM_NEC_78K0 = 38,// = NEC = 78K0
        PLFM_NEC_78K0S = 39,// = NEC = 78K0S
        PLFM_M740 = 40,// = Mitsubishi = 8bit
        PLFM_M7700 = 41,// = Mitsubishi = 16bit
        PLFM_ST9 = 42,// = ST9+
        PLFM_FR = 43,// = Fujitsu = FR = Family
        PLFM_MC6816 = 44,// = Motorola = 68HC16
        PLFM_M7900 = 45,// = Mitsubishi = 7900
        PLFM_TMS320C3 = 46,// = Texas = Instruments = TMS320C3
        PLFM_KR1878 = 47,// = Angstrem = KR1878
        PLFM_AD218X = 48,// = Analog = Devices = ADSP = 218X
        PLFM_OAKDSP = 49,// = Atmel = OAK = DSP
        PLFM_TLCS900 = 50,// = Toshiba = TLCS-900
        PLFM_C39 = 51,// = Rockwell = C39
        PLFM_CR16 = 52,// = NSC = CR16
        PLFM_MN102L00 = 53,// = Panasonic = MN10200
        PLFM_TMS320C1X = 54,// = Texas = Instruments = TMS320C1x
        PLFM_NEC_V850X = 55,// = NEC = V850 = and = V850ES/E1/E2
        PLFM_SCR_ADPT = 56,// = Processor = module = adapter = for = processor = modules = written = in = scripting = languages
        PLFM_EBC = 57,// = EFI = Bytecode
        PLFM_MSP430 = 58,// = Texas = Instruments = MSP430
        PLFM_SPU = 59,// = Cell = Broadband = Engine = Synergistic = Processor = Unit
        PLFM_DALVIK = 60,// = Android = Dalvik = Virtual = Machine
    }
}
