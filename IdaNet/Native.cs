using System;
using System.Runtime.InteropServices;

namespace IdaNet
{
    internal static class Native
    {
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        internal static extern IntPtr CreateEvent([In] IntPtr securityAttributes, [In] bool manualReset, [In] bool initialState, [In] string name);

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        internal static extern IntPtr SetEvent([In] IntPtr hEvent);
    }
}
