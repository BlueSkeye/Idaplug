using System;

namespace IdaNet.IdaInterop
{
    internal enum MessageBoxKind
    {
        mbox_internal,                // internal error
        mbox_info,
        mbox_warning,
        mbox_error,
        mbox_nomem,
        mbox_feedback,
        mbox_readerror,
        mbox_writeerror,
        mbox_filestruct,
        mbox_wait,
        mbox_hide,
        mbox_replace,
    }
}
