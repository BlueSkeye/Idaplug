using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IdaNet.IdaInterop
{
    /// <summary>Provides various way to interact with the user.</summary>
    internal static class Interactivity
    {
        // Display info dialog box and wait for the user to press Enter or Esc
        //      format - printf() style format string.
        //               It may have some prefixes, see 'Format of dialog box' for details.
        // This messagebox will by default contain a "Don't display this message again"
        // checkbox. If checked, the message will never be displayed anymore (state saved
        // in the Windows registry or the idareg.cfg file for a non-Windows version).
        // Info() function does the same but the format string is taken from IDA.HLP
        internal static int Info(string format, params object[] parameters)
        {
            return InteropConstants.CallUI(UiNotificationType.ui_mbox,
                __arglist(MessageBoxKind.mbox_info, string.Format(format, parameters)));
        }

        // Output a formatted string to messages window [analog of printf()]
        //      format - printf() style message string.
        // Message() function does the same but the format string is taken from IDA.HLP
        // Returns: number of bytes output
        // Everything appearing on the messages window may be written
        // to a text file. For this the user should define the following environment
        // variable:
        //         set IDALOG=idalog.txt
        internal static int Message(string format, params object[] parameters)
        {
            string message = ((null == parameters) || (0 == parameters.Length))
                ? format
                : string.Format(format, parameters);
            return InteropConstants.CallUI(UiNotificationType.ui_msg, __arglist(message));
        }

        // Display warning dialog box and wait for the user to press Enter or Esc
        //      format - printf() style format string.
        //               It may have some prefixes, see 'Format of dialog box' for details.
        // This messagebox will by default contain a "Don't display this message again"
        // checkbox if the message is repetitively displayed. If checked, the message
        // won't be displayed anymore during the current IDA session.
        // Warn() function does the same but the format string is taken from IDA.HLP
        internal static int Warning(string format, params object[] parameters)
        {
            return InteropConstants.CallUI(UiNotificationType.ui_mbox,
                __arglist(MessageBoxKind.mbox_warning, string.Format(format, parameters)));
        }
    }
}
