using System;

namespace IdaNet.IdaInterop
{
    internal enum IdcChkKeyCode
    {
        IDCHK_OK = 0,       // ok
        IDCHK_ARG = -1,      // bad argument(s)
        IDCHK_KEY = -2,      // bad hotkey name
        IDCHK_MAX = -3      // too many IDC hotkeys
    }
}
