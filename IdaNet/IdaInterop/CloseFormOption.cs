using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum CloseFormOption
    {
        FORM_SAVE = 0x1, // save state in desktop config
        FORM_NO_CONTEXT = 0x2, // don't change the current context (useful for toolbars)
        FORM_DONT_SAVE_SIZE = 0x4, // don't save size of the window
    }
}
