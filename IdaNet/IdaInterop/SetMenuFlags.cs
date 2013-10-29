using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum SetMenuFlags
    {
        SETMENU_POSMASK = 0x1,
        SETMENU_INS = 0x0,         // add menu item before the specified path (default)
        SETMENU_APP = 0x1,         // add menu item after the specified path
        SETMENU_CTXMASK = 0x7C000000,  // context flags for the menu item
        SETMENU_CTXAPP = 0x00000000,  // global (default)
        SETMENU_CTXIDA = 0x04000000,  // disassembly
        SETMENU_CTXSTR = 0x08000000,  // structures
        SETMENU_CTXENUM = 0x0C000000,  // enumerations
        SETMENU_CTXEA = 0x10000000,  // ea views (disassembly, hex)
        SETMENU_CTXVIEW = 0x14000000,  // any kind of view (disassembly, hex, structures, etc.)
    }
}
