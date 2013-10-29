using System;

namespace IdaNet.IdaInterop
{
    [Flags()]
    internal enum OpenFormOption
    {
        FORM_MDI = 0x01, // start by default as MDI
        FORM_TAB = 0x02, // attached by default to a tab
        FORM_RESTORE = 0x04, // restore state from desktop config
        FORM_ONTOP = 0x08, // form should be "ontop"
        FORM_MENU = 0x10, // form must be listed in the windows menu
                                // (automatically set for all plugins)
        FORM_CENTERED = 0x20, // form will be centered on the screen
                            // Returns: nothing
        FORM_PERSIST = 0x40, // form will persist until explicitly closed with close_tform()
                            // Returns: nothing
        FORM_QWIDGET = 0x80, // windows: use QWidget* instead of HWND in ui_tform_visible
                                // this flag is ignored in unix because we never use HWND there.
                                // around 2011/09 we plan to get rid of HWND and always use QWidget*
                                // regardless of this flag.
    }
}
