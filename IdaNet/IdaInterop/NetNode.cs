using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
    // This file contains definitions of various information kept in netnodes. Each address in the
    // program has a corresponding netnode: netnode(ea). If we have no information about an address,
    // the corresponding netnode is not created. Otherwise we will create a netnode and save information
    // in it. All variable length information (names, comments, offset information, etc) is stored in the
    // netnode. Don't forget that some information is already stored in the flags (bytes.hpp)
    // IMPORTANT NOTE:
    // Many of functions in this file are very low level (they are marked as low level functions). Use
    // them only if you can't find higher level function to set/get/del information.
    // You can create your own nodes in IDP module and store information in them. Look at netnode.hpp for
    // the definition of netnodes.

    // This file contains functions that provide the lowest level interface to the ida database, namely
    // Btree. To learn more about Balanced Trees: http://www.bluerwhite.org/btree/
    // We don't use Btree directly. Instead, we have another layer built on the top of Btree. Here is a
    // brief explanation of this layer.
    // There is a graph. The graph consists of nodes and links between them. We call a node "netnode".
    // Netnodes are numbered with 32-bit values. Usually there is a trivial mapping of the linear addresses
    // used in the program to the netnodes. If we have additional information about an address (a comment
    // is attached to it, for example), this information is stored in the corresponding netnode. See nalt.hpp
    // if you want to see how the kernel uses netnodes. Also, some netnodes have no corresponding linear
    // address. They are used to store information not related to a particular address.
    //
    // Each netnode _may_ have the following attributes:
    //
    // - a name (max length of name is MAXNAMESIZE) there is no limitation on characters used in names.
    // - a value: arbitary sized object, max size is MAXSPECSIZE
    // - altvals: a sparse array of 32-bit values. indexes in this array may be 8-bit or 32-bit values
    // - supvals: an array of arbitrary sized objects. (size of each object is limited by MAXSPECSIZE)
    //      indexes in this array may be 8-bit or 32-bit values
    // - charvals: a sparse array of 8-bit values. indexes in this array may be 8-bit or 32-bit values
    // - hashvals: a hash (an associative array) indexes in this array are strings values are arbitrary
    //      sized (max size is MAXSPECSIZE)
    // Initially a new netnode contains no information so no disk space is used for it. As you add new information,
    // the netnode grows. All arrays behave in the same manner: initally
    // - all members of altvals/charvals array are zeroes
    // - all members of supvals/hashvals array are undefined.
    //
    // About values returned by the netnode function: the returned string values may be modified freely.
    // They are returned in static buffers. The size of each buffer is MAXSPECSIZE+1. There are 10 buffers
    // and they are used in a round-robin manner.
    // There are high-level functions to store arbitrary sized objects (blobs) in supvals.
    // You may use netnodes to store additional information about the program. Limitations on the use of netnodes
    // are the following:
    // - use netnodes only if you could not find a kernel service to store your type of information
    // - Do not create netnodes with valid identifier names. Use the "$ " prefix (or any other prefix with characters
    //      not allowed in the identifiers for the names of your netnodes. Although you will probably not destroy
    //      anything by accident, using already defined names for the names of your netnodes is still discouraged.
    // - you may create as many netnodes as you want (creation of unnamed netnode doesn't increase the size of the
    //      database). however, since each netnode has a number, creating too many netnodes could lead to the exhaustion
    //      of the netnode numbers (the numbering starts at 0xFF000000)
    // - remember that netnodes are automatically saved to the disk by the kernel.
    // Advanced info:
    // In fact a netnode may contain up to 256 arrays of arbitrary sized objects. Each array has its 8-bit tag.
    // Usually tags are represented by characters constants. Altvals and supvals are simply 2 of 256 arrays, with
    // tags 'A' and 'S' respectively.
    // Links between the netnodes are called netlinks. Each netlink has a type. The ida kernel doesn't use the links.
    // The netlink type is represented as a 32-bit number and has a name. The netlinks are used to build the graph.
    // Also, each particular netlink between two netnodes may have arbitrary text attached to it. Netlinks are deprecated!

    // Note that the size of the 'netnode' class is 4 bytes and it can be freely casted to 'uint32' and back.
    // This makes it easy to store information about the program location in the netnodes. Please pass netnodes
    // to functions by value.
    internal class NetNode : IDisposable
    {
        #region CONSTRUCTORS
        internal NetNode()
        {
            NativePointer = Marshal.AllocCoTaskMem(sizeof(NodeIndex));
            return;
        }

        // Constructor to create a netnode to access information about the specified linear address
        internal NetNode(NodeIndex index)
            : this()
        {
            Index = index;
            return;
        }

        // Construct an instance of netnode class to access the specified netnode
        //      name      - name of netnode
        //      namlen    - length of the name. may be omitted, in this
        //                  case the length will be calcuated with strlen()
        //      do_create - true:  create the netnode if it doesn't exist yet.
        //                  false: don't create the netnode, set netnumber to BADNODE if
        //                         it doesn't exist
        internal NetNode(string name)
            : this(name, false)
        {
            return;
        }

        internal NetNode(string name, bool do_create)
            : this()
        {
            IntPtr nativeName = (null == name) ? IntPtr.Zero : Marshal.StringToCoTaskMemAnsi(name);

            try { netnode_check(NativePointer, nativeName, string.IsNullOrEmpty(name) ? 0 : name.Length, do_create); }
            finally { Marshal.FreeCoTaskMem(nativeName); }
            return;
        }

        ~NetNode()
        {
            Dispose(false);
            return;
        }
        #endregion

        #region OPERATORS
        public static bool operator == (NetNode first, NetNode second)
        {
            // WARNING : Explicit casting to object for null reference comparison is mandatory
            // otherwise we would enter an infinite recursive loop invoking our overloaded
            // operator again and again.
            if ((object)null == (object)first) { return ((object)null == (object)second); }
            if ((object)null == (object)second)
            {
                // Already checkd first for null reference.
                return false;
            }
            return (first.Index == second.Index);
        }

        public static bool operator !=(NetNode first, NetNode second)
        {
            Interactivity.Message("NetNodeEnumerator::!=\r\n");
            return !(first == second);
        }
        #endregion

        #region PROPERTIES
        internal byte[] BlobValue
        {
            get
            {
                IntPtr nativeBuffer = Marshal.AllocCoTaskMem(MAXSPECSIZE);

                try
                {
                    int blobLength = netnode_valobj(Index, nativeBuffer, MAXSPECSIZE);

                    if (-1 == blobLength) { return null; }
                    byte[] result = new byte[(int)blobLength];

                    Marshal.Copy(nativeBuffer, result, 0, (int)blobLength);
                    return result;
                }
                finally { Marshal.FreeCoTaskMem(nativeBuffer); }
            }
            set
            {
                if (null == value)
                {
                    netnode_delvalue(Index);
                    return;
                }
                IntPtr nativeBuffer = Marshal.AllocCoTaskMem(value.Length);

                try
                {
                    Marshal.Copy(value, 0, nativeBuffer, value.Length);
                    netnode_set(Index, nativeBuffer, value.Length);
                }
                finally { Marshal.FreeCoTaskMem(nativeBuffer); }
            }
        }

        internal NodeIndex Index
        {
            get
            {
#if __EA64__
                return (NodeIndex)Marshal.ReadInt64(NativePointer);
#else
                return (NodeIndex)Marshal.ReadInt32(NativePointer);
#endif
            }
            private set
            {
#if __EA64__
                Marshal.WriteInt64(NativePointer, (long)(NodeIndex)value);
#else
                Marshal.WriteInt32(NativePointer, (int)(NodeIndex)value);
#endif
            }
        }

        internal string Name
        {
            get
            {
                IntPtr nativeBuffer = Marshal.AllocCoTaskMem(MAXNAMESIZE);

                try
                {
                    int nameLength = netnode_name(Index, nativeBuffer, MAXNAMESIZE);
                    if (-1 == nameLength) { return null; }
                    byte[] buffer = new byte[Math.Min((int)nameLength, nameLength)];

                    Marshal.Copy(nativeBuffer, buffer, 0, (int)nameLength);
                    return ASCIIEncoding.ASCII.GetString(buffer);
                }
                finally { Marshal.FreeCoTaskMem(nativeBuffer); }
            }
            set
            {
                IntPtr nativeBuffer;

                if (string.IsNullOrEmpty(value)) { nativeBuffer = IntPtr.Zero; }
                else
                {
                    byte[] localBuffer = ASCIIEncoding.ASCII.GetBytes(value);
                    nativeBuffer = Marshal.AllocCoTaskMem(localBuffer.Length);

                    Marshal.Copy(localBuffer, 0, nativeBuffer, localBuffer.Length);
                }
                netnode_rename(Index, nativeBuffer, 0);
                return;
            }
        }

        private IntPtr NativePointer { get; set; }
        #endregion

        #region METHODS
        internal delegate bool AdjusterDelegate(NodeIndex ea);

        // Adjust values of altval arrays elements
        // All altvals in the range from+1..from+size+1 and adjusted to have
        // values in the range to+1..to+size+1.
        // The function should_skip can be used to skip the adjustment of some altvals
        internal void AdjustAlternateValuesRange(NodeIndex from, NodeIndex to, NodeIndex size, AdjusterDelegate shouldSkip)
        {
            IntPtr callback = (null == shouldSkip)
                ? IntPtr.Zero
                : Marshal.GetFunctionPointerForDelegate(shouldSkip);

            netnode_altadjust(this.Index, from, to, size, callback);
            return;
        }

        private bool Check(string oldName)
        {
            IntPtr nativeName = (null == oldName) ? IntPtr.Zero : Marshal.StringToCoTaskMemAnsi(oldName);

            try { return netnode_check(this.NativePointer, nativeName, oldName.Length, false); }
            finally { Marshal.FreeCoTaskMem(nativeName); }
        }

        internal int CopyTo(NetNode target, NodeIndex count)
        {
            return netnode_copy(this.Index, count, target.Index, false);
        }

        // Create unnamed netnode
        // returns: 1 - ok
        //          0 - should not happen, indicates internal error
        internal bool Create()
        {
            return Create("");
        }

        // Create a named netnode
        //      name   - name of netnode to create Names of user-defined netnodes must have the "$ " prefix
        //               in order to avoid clashes with program byte names.
        //      namlen - length of the name. If not specified, it will be calculated using strlen()
        // returns: 1 - ok, the node is created
        //          0 - the node already exists. you may use the netnode class to access it.
        internal bool Create(string name)
        {
            IntPtr nativeName = (null == name) ? IntPtr.Zero : Marshal.StringToCoTaskMemAnsi(name);

            try { return netnode_check(NativePointer, nativeName, string.IsNullOrEmpty(name) ? 0 : name.Length, true); }
            finally { Marshal.FreeCoTaskMem(nativeName); }
        }

        // Delete all elements of hash
        // This function deletes the whole hash
        //      tag   - tag of hash. Default: htag
        // returns: 1 - ok, 0 - some error
        internal bool DeleteAllHashes(byte tag)
        {
            return DeleteAllSupplementaryValues(tag);
        }

        // Delete all elements of supplementary/alternate values array
        // This function may be applied to 32-bit and 8-bit supval arrays.
        // This function deletes the whole supval array.
        // returns: 1 - ok, 0 - some error
        internal bool DeleteAllSupplementaryValues(byte tag)
        {
            return netnode_supdel_all(this.Index, tag);
        }

        // Delete a blob
        //      start - index of the first supval element used to store blob
        //      tag   - tag of supval array
        // returns: number of deleted supvals
        internal int DeleteBlob(NodeIndex start, byte tag)
        {
            return netnode_delblob(this.Index, start, tag);
        }

        // Delete hash element
        //      idx   - index into hash
        //      tag   - tag of hash. Default: htag
        // returns: true - deleted, false - element does not exist
        internal bool DeleteHashedItem(string idx, byte tag)
        {
            IntPtr nativeKey = Marshal.StringToCoTaskMemAnsi(idx);

            try { return netnode_hashdel(this.Index, nativeKey, tag); }
            finally { Marshal.FreeCoTaskMem(nativeKey); }
        }

        // Delete supplementary/alternate element
        //      alt   - index into array of supvals
        //      tag   - tag of array
        // returns: true - deleted, false - element does not exist
        internal bool DeleteSupplementaryValue(NodeIndex alt, byte tag)
        {
            return netnode_supdel(this.Index, alt, tag);
        }

        // Delete range of elements in the specified array
        // Elements in range [idx1, idx2) will be deleted
        //      idx1  - first element to delete
        //      idx2  - last element to delete + 1
        //      tag   - tag of array
        // returns: number of deleted elements
        internal int DeleteSupplementaryValuesRange(NodeIndex from, NodeIndex toExcluded, byte tag)
        {
            return netnode_supdel_range(this.Index, from, toExcluded, tag);
        }

        public void Dispose()
        {
            Dispose(true);
            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) { GC.SuppressFinalize(this); }
            if (IntPtr.Zero != NativePointer)
            {
                Marshal.FreeCoTaskMem(NativePointer);
                NativePointer = IntPtr.Zero;
            }
            return;
        }

        /// <summary>Enumerate all nodes in the database.</summary>
        /// <returns></returns>
        internal static IEnumerable<NetNode> Enumerate()
        {
            return new NetNodeEnumerator.Factory();
        }

        internal static IEnumerable<byte> EnumerateKnownTags()
        {
            yield return (byte)'A'; // Array of altvals
            yield return (byte)'S'; // Array of supvals
            yield return (byte)'H'; // Array of hashvals
            yield return (byte)'V'; // Value of netnode
            yield return (byte)'N'; // Name of netnode
            yield return (byte)'L'; // Links between netnodes
            yield break;
        }

        /// <summary>Check whether a node having the given index exists or not.</summary>
        /// <param name="index">Node index.</param>
        /// <returns>true if the node exists, false otherwise.</returns>
        internal static bool Exist(NodeIndex index)
        {
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(sizeof(NodeIndex));

            try
            {
#if __EA64__
                Marshal.WriteInt64(nativeBuffer, (long)index);
#else
                Marshal.WriteInt32(nativeBuffer, (int)index);
#endif
                return netnode_exist(nativeBuffer);
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        // Get altval element of the specified array
        //      alt - index into array of altvals
        //      tag - tag of array. may be omitted
        // returns: value of altval element. Unexistent altval members are returned as zeroes
        internal NodeIndex GetAlternateValue(NodeIndex at, byte tag)
        {
            return netnode_altval(this.Index, at, tag);
        }

        // Get blob from a netnode
        //      buf     - buffer to read into. if NULL, the buffer will be allocated using qalloc()
        //      bufsize - in:  size of 'buf' in bytes (if buf == NULL then meaningless)
        //                out: size of the blob if it exists bufsize may be NULL
        //      start   - index of the first supval element used to store blob
        //      tag     - tag of supval array
        // returns: NULL - blob doesn't exist otherwise returns pointer to blob
        internal byte[] GetBlob(byte[] buf, NodeIndex start, byte tag)
        {
            IntPtr nativeBuffer = IntPtr.Zero;

            try
            {
                int bufsize = 0;
                nativeBuffer = netnode_getblob(this.Index, nativeBuffer, ref bufsize, start, tag);
                if (null == buf) { buf = new byte[bufsize]; }
                Marshal.Copy(nativeBuffer, buf, 0, Math.Min(buf.Length, bufsize));
                return buf;
            }
            finally { if (IntPtr.Zero != nativeBuffer) { Marshal.FreeCoTaskMem(nativeBuffer); } }
        }

        // Get size of blob
        //      start - index of the first supval element used to store blob
        //      tag   - tag of supval array
        // returns: number of bytes required to store a blob
        internal int GetBlobSize(NodeIndex start, byte tag)
        {
            return netnode_blobsize(this.Index, start, tag);
        }

        // Get first existing element of hash
        //      buf   - output buffer, may be NULL
        //      bufsize - output buffer size
        //      tag   - tag of hash. Default: htag
        // returns: size of index of first existing element of hash -1 if hash is empty
        // note: elements of hash are kept sorted in lexical order
        internal int GetFirstHashedValue(byte tag, out string key)
        {
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(MAXSPECSIZE);

            try
            {
                int result = netnode_hash1st(this.Index, nativeBuffer, MAXSPECSIZE, tag);

                if (-1 == result) { key = null; }
                else
                {
                    byte[] localBuffer = new byte[result];

                    Marshal.Copy(nativeBuffer, localBuffer, 0, (int)result);
                    key = ASCIIEncoding.ASCII.GetString(localBuffer);
                }
                return result;
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        // Get first existing element of supval array
        //      tag   - tag of array
        // returns: index of first existing element of supval array BADNODE if supval array is empty
        internal NodeIndex GetFirstSupplementaryValue(byte tag)
        {
            return netnode_sup1st(this.Index, tag);
        }

        // Get value of the specified hash element
        //      key   - index into hash
        //      buf   - output buffer, may be NULL
        //      bufsize - output buffer size
        //      tag   - tag of hash. Default: htag
        // returns: -1 - element doesn't exist or key is NULL otherwise the value size in bytes
        internal SignedSize GetHashedValue(string key, out byte[] buf, byte tag)
        {
            IntPtr nativeKey = Marshal.StringToCoTaskMemAnsi(key);
            IntPtr nativeBuffer = IntPtr.Zero;

            try
            {
                nativeBuffer = Marshal.AllocCoTaskMem(MAXSPECSIZE);
                SignedSize result = netnode_hashval(this.Index, nativeKey, nativeBuffer, MAXSPECSIZE, tag);

                buf = new byte[(int)result];
                Marshal.Copy(nativeBuffer, buf, 0, (int)result);
                return result;
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativeKey);
                if (IntPtr.Zero != nativeBuffer) { Marshal.FreeCoTaskMem(nativeBuffer); }
            }
        }

        // Get last existing element of hash
        //      buf   - output buffer, may be NULL
        //      bufsize - output buffer size
        //      tag   - tag of hash. Default: htag
        // returns: size of index of last existing element of hash
        //          -1 if hash is empty
        // note: elements of hash are kept sorted in lexical order
        internal int GetLastHashedValue(byte tag, out string key)
        {
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(MAXSPECSIZE);

            try
            {
                int result = netnode_hashlast(this.Index, nativeBuffer, MAXSPECSIZE, tag);
                key = Marshal.PtrToStringAnsi(nativeBuffer);
                return result;
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        // Get last existing element of supval array
        //      tag   - tag of array
        // returns: index of last existing element of supval array
        //          BADNODE if supval array is empty
        internal NodeIndex GetLastSupplementaryValue(byte tag)
        {
            return netnode_suplast(this.Index, tag);
        }

        // Get next existing element of hash
        //      idx   - current index into hash
        //      buf   - output buffer, may be NULL
        //      bufsize - output buffer size
        //      tag   - tag of hash. Default: htag
        // returns: size of index of the next existing element of hash
        //          -1 if no more hash elements exist
        // note: elements of hash are kept sorted in lexical order
        internal int GetNextHashedValue(string currentKey, byte tag, out string nextKey)
        {
            IntPtr nativeCurrentKey = Marshal.StringToCoTaskMemAnsi(currentKey);
            IntPtr nativeNextKey = IntPtr.Zero;

            try
            {
                nativeNextKey = Marshal.AllocCoTaskMem(MAXNAMESIZE);
                int result = netnode_hashnxt(this.Index, nativeCurrentKey, nativeNextKey, MAXNAMESIZE, tag);
                nextKey = (-1 == result) ? null : Marshal.PtrToStringAnsi(nativeNextKey);
                return result;
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativeCurrentKey);
                if (IntPtr.Zero != nativeNextKey) { Marshal.FreeCoTaskMem(nativeNextKey); }
            }
        }

        // Get next existing element of supval array
        //      cur   - current index
        //      tag   - tag of array
        // returns: index of the next existing element of supval array BADNODE if no more supval array elements exist
        internal NodeIndex GetNextSupplementaryValue(NodeIndex cur, byte tag)
        {
            return netnode_supnxt(this.Index, cur, tag);
        }

        // Get previous existing element of supval array
        //      idx   - current index into hash
        //      buf   - output buffer, may be NULL
        //      bufsize - output buffer size
        //      tag   - tag of hash. Default: htag
        // returns: size of index of the previous existing element of hash
        //          -1 if no more hash elements exist
        // note: elements of hash are kept sorted in lexical order
        internal int GetPreviousHashedValue(string currentKey, byte tag, out string previousKey)
        {
            IntPtr nativeCurrentKey = Marshal.StringToCoTaskMemAnsi(currentKey);
            IntPtr nativePreviousKey = IntPtr.Zero;

            try
            {
                nativePreviousKey = Marshal.AllocCoTaskMem(MAXNAMESIZE);
                int result = netnode_hashprev(this.Index, nativeCurrentKey, nativePreviousKey, MAXNAMESIZE, tag);
                previousKey = (-1 == result) ? null : Marshal.PtrToStringAnsi(nativePreviousKey);
                return result;
            }
            finally
            {
                Marshal.FreeCoTaskMem(nativeCurrentKey);
                if (IntPtr.Zero != nativePreviousKey) { Marshal.FreeCoTaskMem(nativePreviousKey); }
            }
        }

        // Get previous existing element of supval array
        //      cur   - current index
        //      tag   - tag of array
        // returns: index of the previous exitsing element of supval array
        //          BADNODE if no more supval array elements exist
        internal NodeIndex GetPreviousSupplementaryValue(NodeIndex cur, byte tag)
        {
            return netnode_supprev(this.Index, cur, tag);
        }

        // Get value of the specified supval array element
        //      alt   - index into array of supvals
        //      buf   - output buffer, may be NULL
        //      bufsize - size of output buffer
        //      tag   - tag of array. Default: stag
        // returns: size of value, -1 - element doesn't exist
        // NB: do not use this function to retrieve strings, see supstr()!
        internal SignedSize GetSupplementaryValue(NodeIndex alt, byte tag, out byte[] buffer)
        {
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(MAXSPECSIZE);

            try
            {
                SignedSize result = netnode_supval(this.Index, alt, nativeBuffer, MAXSPECSIZE, tag);
                if (-1 == (int)result) { buffer = null; }
                else
                {
                    buffer = new byte[(int)result];
                    Marshal.Copy(nativeBuffer, buffer, 0, (int)result);
                }
                return result;
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        // Delete a netnode with all information attached to it
        internal void Kill()
        {
            netnode_kill(NativePointer);
            return;
        }

        private IntPtr MarshalToNative(byte[] data)
        {
            return MarshalToNative(data, 0, data.Length);
        }

        /// <summary>Translate the given part of a byte array into a native buffer.
        /// The caller is responsible for freeing the native buffer using the
        /// <see cref="Marshal.FreeCoTaskMem"/> method.</summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private IntPtr MarshalToNative(byte[] data, int offset, int length)
        {
            IntPtr result = Marshal.AllocCoTaskMem(length);

            Marshal.Copy(data, offset, result, length);
            return result;
        }

        internal int MoveTo(NetNode target, NodeIndex count)
        {
            return netnode_copy(this.Index, count, target.Index, true);
        }

        // Set value of altval array
        //      alt   - index into array of altvals
        //      value - new value of altval element
        //      tag   - tag of array
        // returns: 1 - ok
        //          0 - failed, normally should not occur
        internal bool SetAlternateValue(NodeIndex at, NodeIndex value, byte tag)
        {
            byte[] binaryValue;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(value);
                    // TODO : Make sure tehh right number of bytes is returned.
                    binaryValue = stream.GetBuffer();
                    return SetSupplementaryValue(at, binaryValue, (int)stream.Length, tag);
                }
            }
        }

        // Store a blob in a netnode
        //      buf     - pointer to blob to save
        //      size    - size of blob in bytes
        //      start   - index of the first supval element used to store blob
        //      tag     - tag of supval array
        // returns: 1 - ok, 0 - error
        internal bool SetBlob(byte[] buf, NodeIndex start, byte tag)
        {
            IntPtr nativeBuffer = Marshal.AllocCoTaskMem(buf.Length);

            try
            {
                Marshal.Copy(buf, 0, nativeBuffer, buf.Length);
                return netnode_setblob(this.Index, nativeBuffer, buf.Length, start, tag);
            }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }

        //// Set value of hash element to long value
        ////      idx   - index into hash
        ////      value - new value of hash element
        ////      tag   - tag of hash. Default: htag
        //// returns: 1 - ok, 0 - error, should not occur
        //internal bool SetHashedItemValue(string idx, NodeIndex value, byte tag)
        //{
        //    byte[] binaryValue;

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        using (BinaryWriter writer = new BinaryWriter(stream))
        //        {
        //            writer.Write(value);
        //            // TODO : Make sure the right number of bytes is returned.
        //            binaryValue = stream.GetBuffer();
        //            return SetSupplementaryValue(at, binaryValue, (int)stream.Length, tag);
        //        }
        //    }
        //    return hashset(idx, &value, sizeof(value), tag);
        //}

        // Set value of hash element
        //      idx   - index into hash
        //      value - pointer to value
        //      length- length of 'value'. If not specified, the length is calculated
        //              using strlen()+1.
        //      tag   - tag of hash. Default: htag
        // returns: 1 - ok, 0 - error, should not occur
        internal bool SetHashedItemValue(string idx, IntPtr value, int length, byte tag)
        {
            IntPtr nativeKey = MarshalToNative(ASCIIEncoding.ASCII.GetBytes(idx));

            try { return netnode_hashset(this.Index, nativeKey, value, length, tag); }
            finally { Marshal.FreeCoTaskMem(nativeKey); }
        }

        // Set value of supval array element
        //      alt   - index into array of supvals
        //      value - pointer to supval value
        //      length- length of 'value'. If not specified, the length is calculated using strlen() + 1.
        //      tag   - tag of array
        // returns: 1 - ok, 0 - error, should not occur
        internal bool SetSupplementaryValue(NodeIndex at, byte[] value, int length, byte tag)
        {
            IntPtr nativeBuffer = MarshalToNative(value, 0, length);

            try { return netnode_supset(this.Index, at, nativeBuffer, length, tag); }
            finally { Marshal.FreeCoTaskMem(nativeBuffer); }
        }
        
        // Shift the altval array elements
        // Moves the array elements at (from..from+size) to (to..to+size)
        // Returns: number of shifted elements
        internal int ShiftAlternateValues(NodeIndex from, NodeIndex to, NodeIndex size, byte tag)
        {
            return netnode_altshift(this.Index, from, to, size, tag);
        }

        // Shift the supval array elements
        // Moves the array elements at (from..from+size) to (to..to+size)
        // Returns: number of shifted elements
        internal int ShiftSupplementartValues(NodeIndex from, NodeIndex to, NodeIndex size, byte tag)
        {
            return netnode_supshift(this.Index, from, to, size, tag);
        }
        #endregion

        #region FIELDS
        internal const NodeIndex BadNode = NodeIndex.MaxValue;

        // Tags internally used in netnodes. You should not use them for your tagged alt/sup/char/hash arrays.
        internal const byte atag = (byte)'A'; // Array of altvals
        internal const byte stag = (byte)'S'; // Array of supvals
        internal const byte htag = (byte)'H'; // Array of hashvals
        internal const byte vtag = (byte)'V'; // Value of netnode
        internal const byte ntag = (byte)'N'; // Name of netnode
        internal const byte ltag = (byte)'L'; // Links between netnodes

        // The BTREE page size. This is not interesting for the end-users.
        internal const int BTREE_PAGE_SIZE = 8192;  // don't use the default 2048 page size
        // Maximum length of a netnode name
        internal const int MAXNAMESIZE = 512;
        // Maximum length of strings or objects stored in supval array element
        internal const int MAXSPECSIZE = 1024;
        #endregion

        #region INNER CLASSES
        internal class HashedValuesEnumerator : IEnumerator<KeyValuePair<string, byte[]>>
        {
            internal class Factory : IEnumerable<KeyValuePair<string, byte[]>>
            {
                internal Factory(NetNode owner, byte tag)
                {
                    _owner = owner;
                    _tag = tag;
                    return;
                }

                public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
                {
                    return new HashedValuesEnumerator(_owner, _tag, true);
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                #region FIELDS
                private NetNode _owner;
                private byte _tag;
                #endregion
            }

            #region CONSTRUCTORS
            internal HashedValuesEnumerator(NetNode owner, byte tag, bool forward)
            {
                _owner = owner;
                _tag = tag;
                Forward = forward;
                return;
            }

            ~HashedValuesEnumerator()
            {
                Dispose(false);
                return;
            }
            #endregion

            #region PROPERTIES
            internal bool Forward { get; set; }
            #endregion

            #region METHODS
            public KeyValuePair<string, byte[]> Current
            {
                get
                {
                    if ((null == _currentKey) || _noMore) { throw new InvalidOperationException(); }
                    return _currentValue;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
                Dispose(true);
                return;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing) { GC.SuppressFinalize(this); }
                return;
            }

            public bool MoveNext()
            {
                if (null == _currentKey)
                {
                    _currentIndex = (Forward)
                        ? _owner.GetFirstHashedValue(_tag, out _currentKey)
                        : _owner.GetLastHashedValue(_tag, out _currentKey);
                }
                else
                {
                    _currentIndex = (Forward)
                        ? _owner.GetNextHashedValue(_currentKey, _tag, out _currentKey)
                        : _owner.GetPreviousHashedValue(_currentKey, _tag, out _currentKey);
                }
                _noMore = (-1 == _currentIndex);
                if (!_noMore)
                {
                    byte[] hashedValue;
                    _owner.GetHashedValue(_currentKey, out hashedValue, _tag);
                    _currentValue = new KeyValuePair<string, byte[]>(_currentKey, hashedValue);
                }
                return !_noMore;
            }

            public void Reset()
            {
                if (null != _currentKey) { _currentKey = null; }
                return;
            }
            #endregion

            #region FIELDS
            private string _currentKey;
            private KeyValuePair<string, byte[]> _currentValue;
            private int _currentIndex;
            private bool _noMore;
            private NetNode _owner;
            private byte _tag;
            #endregion
        }

        /// <summary>An enumerator suitable for retrieving A and S tagged values.</summary>
        internal class NodeValuesEnumerator : IEnumerator<byte[]>
        {
            internal class Factory : IEnumerable<byte[]>
            {
                internal Factory(NetNode owner, byte tag)
                {
                    _owner = owner;
                    _tag = tag;
                    return;
                }

                public IEnumerator<byte[]> GetEnumerator()
                {
                    return new NodeValuesEnumerator(_owner, _tag, true);
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                #region FIELDS
                private NetNode _owner;
                private byte _tag;
                #endregion
            }

            #region CONSTRUCTORS
            internal NodeValuesEnumerator(NetNode owner, byte tag, bool forward)
            {
                _owner = owner;
                _tag = tag;
                Forward = forward;
                return;
            }

            ~NodeValuesEnumerator()
            {
                Dispose(false);
                return;
            }
            #endregion

            #region PROPERTIES
            internal bool Forward { get; set; }
            #endregion

            #region METHODS
            public byte[] Current
            {
                get
                {
                    if ((null == _currentValue) || _noMore) { throw new InvalidOperationException(); }
                    return _currentValue;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
                Dispose(true);
                return;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing) { GC.SuppressFinalize(this); }
                return;
            }

            public bool MoveNext()
            {
                if (null == _currentValue)
                {
                    _currentIndex = (Forward)
                        ? _owner.GetFirstSupplementaryValue(_tag)
                        : _owner.GetLastSupplementaryValue(_tag);
                }
                else
                {
                    _currentIndex = (Forward)
                        ? _owner.GetNextSupplementaryValue(_currentIndex, _tag)
                        : _owner.GetPreviousSupplementaryValue(_currentIndex, _tag);
                }
                _noMore = (InteropConstants.BadNode == _currentIndex);
                if (!_noMore) { _owner.GetSupplementaryValue(_currentIndex, _tag, out _currentValue); }
                return !_noMore;
            }

            public void Reset()
            {
                if (null != _currentValue) { _currentValue = null; }
                return;
            }
            #endregion

            #region FIELDS
            private byte[] _currentValue;
            private NodeIndex _currentIndex;
            private bool _noMore;
            private NetNode _owner;
            private byte _tag;
            #endregion
        }

        internal class NetNodeEnumerator : IEnumerator<NetNode>
        {
            internal class Factory : IEnumerable<NetNode>
            {
                public IEnumerator<NetNode> GetEnumerator()
                {
                    return new NetNodeEnumerator(true);
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
            }

            #region CONSTRUCTORS
            internal NetNodeEnumerator(bool forward)
            {
                Forward = forward;
                return;
            }

            ~NetNodeEnumerator()
            {
                Dispose(false);
                return;
            }
            #endregion

            #region PROPERTIES
            bool Forward { get; set; }
            #endregion

            #region METHODS
            public NetNode Current
            {
                get
                {
                    if ((null == _currentNode) || _noMore) { throw new InvalidOperationException(); }
                    return new NetNode(_currentNode.Index);
                }
            }

            public void Dispose()
            {
                Dispose(true);
                return;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing) { GC.SuppressFinalize(this); }
                return;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (null == _currentNode)
                {
                    _currentNode = new NetNode(0x0);
                    _noMore = (Forward)
                        ? (0 == netnode_start(_currentNode.NativePointer))
                        : (0 == netnode_end(_currentNode.NativePointer));
                }
                else
                {
                    _noMore = (Forward)
                        ? (0 == netnode_next(_currentNode.NativePointer))
                        : (0 == netnode_prev(_currentNode.NativePointer));
                }
                return !_noMore;
            }

            public void Reset()
            {
                if (null != _currentNode)
                {
                    _currentNode.Dispose();
                    _currentNode = null;
                }
                return;
            }
            #endregion

            #region FIELDS
            private NetNode _currentNode;
            private bool _noMore = false;
            #endregion
        }
        #endregion

        #region IDA NATIVE FUNCTIONS
        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_check(IntPtr nativeNode, IntPtr name, int namlen, bool create);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern void netnode_kill(IntPtr nativeNode);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_start(IntPtr nativeNode);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_end(IntPtr nativeNode);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_next(IntPtr nativeNode);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_prev(IntPtr nativeNode);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_name(NodeIndex num, IntPtr buf, int bufsize);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_rename(NodeIndex num, IntPtr newname, int namlen);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_valobj(NodeIndex num, IntPtr buf, int bufsize);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_valstr(NodeIndex num, byte[] buf, int bufsize);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_set(NodeIndex num, IntPtr value, int length);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_delvalue(NodeIndex num);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_altval(NodeIndex num, NodeIndex alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_charval(NodeIndex num, NodeIndex alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_altval_idx8(NodeIndex num, byte alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern byte netnode_charval_idx8(NodeIndex num, byte alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_supval(NodeIndex num, NodeIndex alt, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_supstr(NodeIndex num, NodeIndex alt, byte[] buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool  netnode_supset(NodeIndex num, NodeIndex alt, IntPtr value, int length, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool  netnode_supdel(NodeIndex num, NodeIndex alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_sup1st(NodeIndex num, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_supnxt(NodeIndex num, NodeIndex cur, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_suplast(NodeIndex num, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_supprev(NodeIndex num, NodeIndex cur, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_supval_idx8(NodeIndex num, byte alt, byte[] buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_supstr_idx8(NodeIndex num, byte alt, byte[] buf,int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_supset_idx8(NodeIndex num, byte alt, byte[] value, int length, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_supdel_idx8(NodeIndex num, byte alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_sup1st_idx8(NodeIndex num, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_supnxt_idx8(NodeIndex num, byte alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_suplast_idx8(NodeIndex num, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_supprev_idx8(NodeIndex num, byte alt, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool  netnode_supdel_all(NodeIndex num, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_supdel_range(NodeIndex num, NodeIndex idx1, NodeIndex idx2, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_supdel_range_idx8(NodeIndex num, NodeIndex idx1, NodeIndex idx2, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_hashval(NodeIndex num, IntPtr idx, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern SignedSize netnode_hashstr(NodeIndex num,  byte[] idx, byte[] buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern NodeIndex netnode_hashval_long(NodeIndex num,  byte[] idx, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_hashset(NodeIndex num,  IntPtr idx, IntPtr value, int length, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_hashdel(NodeIndex num,  IntPtr idx, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_hash1st(NodeIndex num, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_hashnxt(NodeIndex num, IntPtr idx, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_hashlast(NodeIndex num, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_hashprev(NodeIndex num, IntPtr idx, IntPtr buf, int bufsize, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_blobsize(NodeIndex num, NodeIndex start,byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr netnode_getblob(NodeIndex num, IntPtr buf, ref int bufsize, NodeIndex start, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_setblob(NodeIndex num, IntPtr buf, int size, NodeIndex start, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_delblob(NodeIndex num, NodeIndex start,byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_inited();

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_copy(NodeIndex num, NodeIndex count, NodeIndex target, bool move);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_altshift(NodeIndex num, NodeIndex from, NodeIndex to, NodeIndex size, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_charshift(NodeIndex num, NodeIndex from, NodeIndex to, NodeIndex size, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int netnode_supshift(NodeIndex num, NodeIndex from, NodeIndex to, NodeIndex size, byte tag);

        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool netnode_exist(IntPtr candidate);

        // shouldSkipCallback : bool (__stdcall *should_skip)(NodeIndex ea)
        [DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern void netnode_altadjust(NodeIndex num, NodeIndex from, NodeIndex to, NodeIndex size,
            IntPtr shouldSkipCallback);

        //[DllImport(InteropConstants.IdaDllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //internal static extern bool netnode_exist(const netnode &n);
        #endregion
    }
}
