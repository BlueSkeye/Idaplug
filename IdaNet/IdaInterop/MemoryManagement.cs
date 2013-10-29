using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IdaNet.IdaInterop
{
    /// <summary>Provides access to native functions that are to be used for memory
    /// management.</summary>
    internal static class MemoryManagement
    {
        // idaman  
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void qfree(IntPtr alloc);
    }
}
