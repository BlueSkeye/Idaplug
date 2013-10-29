using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// Those aliases should be copied in every source file because C# doesn't support
// source code inclusion.
#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using NodeIndex = System.UInt64;
using SegmentSelector = System.UInt64;
using SignedSize = System.Int64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using NodeIndex = System.UInt32;
using SegmentSelector = System.UInt32;
using SignedSize = System.Int32;
#endif

namespace IdaNet.IdaInterop
{
    internal static class InteropConstants
    {
        #region CLASS INITIALIZER
        static InteropConstants()
        {
            return;
        }
        #endregion

        #region PROPERTIES
        #endregion

        #region METHODS
        internal static IntPtr Combine(IntPtr baseAddress, AddressDifference offset)
        {
            return new IntPtr((AddressDifference)(baseAddress.ToInt64() + offset));
        }

        internal static void Dump(IntPtr at, int bytesCount)
        {
            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < bytesCount; index++)
            {
                if (0 == (index % 16))
                {
                    if (0 < builder.Length)
                    {
                        builder.Append("\r\n");
                        Interactivity.Message(builder.ToString());
                        builder.Length = 0;
                    }
                    builder.AppendFormat("{0:X08} : ", at.ToInt32() + index);
                }
                else { if (0 == (index % 8)) { builder.Append("- "); } }
                builder.AppendFormat("{0:X02} ", Marshal.ReadByte(at, index));
            }
            if (0 < builder.Length)
            {
                builder.Append("\r\n");
                Interactivity.Message(builder.ToString());
            }
            return;
        }
        #endregion
        internal const NodeIndex BadNode = NodeIndex.MaxValue;
        internal const EffectiveAddress BadAddress = EffectiveAddress.MaxValue;
        // Maximum number of segment registers is 16 (see srarea.hpp)
        internal const int SREG_NUM = 16;

#if __EA64__
        internal const string NativeLoaderName = "IdaNetLoader.p64";
        internal const string IdaDllName = "ida64.wll";
#else
        internal const string NativeLoaderName = "IdaNetLoader.plw";
        internal const string IdaDllName = "ida.wll";
#endif

        /// <summary>Implement a trampoline to the callui function thanks to
        /// an exported function in our native loader.</summary>
        /// <param name="function">The function to be invoked</param>
        /// <returns></returns>
        [DllImport(InteropConstants.NativeLoaderName, EntryPoint = "CallUITrampoline",
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int CallUI(UiNotificationType function, __arglist);

        /// <summary>Implement a trampoline to the callui function thanks to
        /// an exported function in our native loader.</summary>
        /// <param name="function">The function to be invoked</param>
        /// <returns></returns>
        [DllImport(InteropConstants.NativeLoaderName, EntryPoint = "GetExportedData")]
        internal static extern IntPtr GetExportedData(ExportedDataIdentifier identifier);
    }
}
