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
using SegmentSelector = System.UInt64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using SegmentSelector = System.UInt32;
#endif

namespace IdaNet.IdaInterop
{
    /// <summary>A set of marshaling utility functions.</summary>
    internal static class MarshalingUtils
    {
        private static void Assert(int ea64Offset, int ea32Offset)
        {
            if (ea64Offset >= ea32Offset) { return; }
            throw new ApplicationException(
                string.Format("Offset mismatch : x64 = {0}, x86 = {1}", ea64Offset, ea32Offset));
        }

        internal static IntPtr Combine(IntPtr baseAddress, int ea64Offset, int ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return new IntPtr(baseAddress.ToInt32() + ea64Offset);
#else
            return new IntPtr(baseAddress.ToInt32() + ea32Offset);
#endif
        }

        internal static AddressDifference GetAddressDifference(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (AddressDifference)Marshal.ReadInt64(nativePointer, ea64Offset);
#else
            return (AddressDifference)Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static  bool GetBool(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (0 != Marshal.ReadByte(nativePointer, ea64Offset));
#else
            return (0 != Marshal.ReadByte(nativePointer, ea32Offset));
#endif
        }

        internal static byte GetByte(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (byte)Marshal.ReadByte(nativePointer, ea64Offset);
#else
            return (byte)Marshal.ReadByte(nativePointer, ea32Offset);
#endif
        }

        internal static void SetByte(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, byte value)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            Marshal.WriteByte(nativePointer, ea64Offset, value);
#else
            Marshal.WriteByte(nativePointer, ea32Offset, value);
#endif
        }

        internal static byte[] GetBytes(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, int length)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
            byte[] result = new byte[length];
            IntPtr baseAddress;
#if __EA64__
            baseAddress = new IntPtr(nativePointer.ToInt32() + ea64Offset);
#else
            baseAddress = new IntPtr(nativePointer.ToInt32() + ea32Offset);
#endif
            Marshal.Copy(baseAddress, result, 0, length);
            return result;
        }

        internal static EffectiveAddress GetEffectiveAddress(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (EffectiveAddress)Marshal.ReadInt64(nativePointer, ea64Offset);
#else
            return (EffectiveAddress)Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static void SetEffectiveAddress(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, EffectiveAddress value)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            Marshal.WriteInt64(nativePointer, ea64Offset, (long)value);
#else
            Marshal.WriteInt32(nativePointer, ea32Offset, (int)value);
#endif
            return;
        }

        internal static T GetFunctionPointer<T>(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
            where T : class
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
            return Marshal.GetDelegateForFunctionPointer(
#if __EA64__
Marshal.ReadIntPtr(nativePointer, ea64Offset),
#else
                Marshal.ReadIntPtr(nativePointer, ea32Offset),
#endif
 typeof(T)) as T;
        }

        internal static int GetInt32(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return Marshal.ReadInt32(nativePointer, ea64Offset);
#else
            return Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static void SetInt32(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, int value)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            Marshal.WriteInt32(nativePointer, ea64Offset, value);
#else
            Marshal.WriteInt32(nativePointer, ea32Offset, value);
#endif
        }

        internal static IntPtr GetIntPtr(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return Marshal.ReadIntPtr(nativePointer, ea64Offset);
#else
            return Marshal.ReadIntPtr(nativePointer, ea32Offset);
#endif
        }

        internal static MemoryChunkSize GetMemoryChunkSize(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (MemoryChunkSize)Marshal.ReadInt64(nativePointer, ea64Offset);
#else
            return (MemoryChunkSize)Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static byte[] GetNullTerminatedBytes(IntPtr nativePointer)
        {
            List<byte> result = new List<byte>();
            int offset = 0;

            while (true)
            {
                byte scannedByte = Marshal.ReadByte(nativePointer, offset);
                if (0 == scannedByte) { return result.ToArray(); }
                result.Add(scannedByte);
                offset++;
            }
        }

        internal static SegmentSelector GetSegmentSelector(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (SegmentSelector)Marshal.ReadInt64(nativePointer, ea64Offset);
#else
            return (SegmentSelector)Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static string GetString(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            IntPtr stringAddress = Marshal.ReadIntPtr(nativePointer, ea64Offset);
#else
            IntPtr stringAddress = Marshal.ReadIntPtr(nativePointer, ea32Offset);
#endif
            return (IntPtr.Zero == stringAddress) ? null : Marshal.PtrToStringAnsi(stringAddress);
        }

        internal static string[] GetStringsArray(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            IntPtr arrayBase = Marshal.ReadIntPtr(nativePointer, ea64Offset);
#else
            IntPtr arrayBase = Marshal.ReadIntPtr(nativePointer, ea32Offset);
#endif
            List<string> names = new List<string>();
            int index = 0;

            do
            {
                IntPtr stringPointer = Marshal.ReadIntPtr(arrayBase, 4 * index++);

                if (IntPtr.Zero == stringPointer) { break; }
                names.Add(Marshal.PtrToStringAnsi(stringPointer));
            } while (true);
            return names.ToArray();
        }

        internal static ushort GetUShort(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (ushort)Marshal.ReadInt16(nativePointer, ea64Offset);
#else
            return (ushort)Marshal.ReadInt16(nativePointer, ea32Offset);
#endif
        }

        internal static void SetUShort(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, ushort value)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            Marshal.WriteInt16(nativePointer, ea64Offset, (short)value);
#else
            Marshal.WriteInt16(nativePointer, ea32Offset, (short)value);
#endif
            return;
        }

        internal static uint GetUInt(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            return (uint)Marshal.ReadInt32(nativePointer, ea64Offset);
#else
            return (uint)Marshal.ReadInt32(nativePointer, ea32Offset);
#endif
        }

        internal static void WriteByte(IntPtr nativePointer, ushort ea64Offset, ushort ea32Offset, byte value)
        {
#if DEBUG
            Assert(ea64Offset, ea32Offset);
#endif
#if __EA64__
            Marshal.WriteByte(nativePointer, ea64Offset, value);
#else
            Marshal.WriteByte(nativePointer, ea32Offset, value);
#endif
            return;
        }
    }
}
