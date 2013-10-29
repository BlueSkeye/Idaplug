using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
    internal delegate Area ControlBlockFactoryDelegate(IntPtr native);

    /// <summary>This structure maps to an areacb_t (area control block) structure as defined
    /// in the IDA SDK. Each type of areas has its own control block. Access to all areas of
    /// one type is made using this control block.</summary>
    internal class AreaControlBlock
    {
        #region CLASS INTIALIZER
        static AreaControlBlock()
        {
            // Functions = new AreaControlBlock(InteropConstants.GetExportedData(ExportedDataIdentifier.ExportedFunctionsId));
            Segments = new AreaControlBlock(InteropConstants.GetExportedData(ExportedDataIdentifier.ExportedSegmentsId), Segment.Create);
            return;
        }
        #endregion

        #region CONSTRUCTORS
        //internal AreaControlBlock()
        //{
        //    Zeroise();
        //    return;
        //}

        internal AreaControlBlock(IntPtr native, ControlBlockFactoryDelegate factory)
        {
            NativePointer = native;
            _factory = factory;
            return;
        }
        
        ~AreaControlBlock()
        {
            Terminate();
        }

        internal static AreaControlBlock Functions { get; private set; }

        /// <summary>Retrieve the native pointer associated with this control block.</summary>
        internal IntPtr NativePointer { get; private set; }

        internal static AreaControlBlock Segments { get; private set; }
        #endregion

        #region PROPERTIES
        /// <summary>Get the numbr of areas that are grouped under this control block.</summary>
        internal uint Count
        {
            get { return areacb_t_get_area_qty(NativePointer); }
        }
        #endregion

        #region METHODS
        /// <summary>Get an area instance based on its index in this control block.</summary>
        /// <param name="index"></param>
        /// <returns>null if the specified area doesn't exist.</returns>
        internal Area this[uint index]
        {
            get
            {
                IntPtr nativeArea = areacb_t_getn_area(NativePointer, index);
                
                return (IntPtr.Zero == nativeArea) ? null : _factory(nativeArea);
            }
        }

        // Let the user choose an area. (1-column chooser)
        // This function displays a window with a list of areas
        // and allows the user to choose an area from the list.
        //      flags - see kernwin.hpp for choose() flags description and callbacks usage
        //      width - width of the window
        //      getl  - callback function to get text representation of an area
        //                      obj - pointer to area control block
        //                      n   - (number of area + 1). if n==0 then getl() should
        //                            return text of a header line.
        //                      buf - buffer for the text representation
        //              getl() should return pointer to text representation string
        //              (not nesessarily the same pointer as 'buf')
        //      title - title of the window.
        //      icon  - number of icon to display
        //      defea - address which points to the default area. The cursor will be
        //              position to this area.
        //      (x0,y0,x1,y1) - window position on the screen
        //                      -1 values specify default window position
        //                      (txt:upper left corner of the screen)
        //                      (gui:centered on the foreground window)
        //      popup_menus - default is insert, delete, edit, refresh
        // returns: NULL - the user pressed Esc.
        //      otherwise - pointer to the selected area.
        // TODO
        //internal delegate byte[] GetlineDelegate(AreaControlBlock obj, uint n, string buf);
        //internal Area ChooseArea(int flags, int width, GetlineDelegate getl, string title,
        //    int icon, int x0, int y0, int x1, int y1, string[] popup_menus,
        //    EffectiveAddress defea)
        //{
        //    return areacb_t_choose_area(NativePointer, flags, width, getl, title, icon,
        //        x0, y0, x1, y1, popup_menus, defea);
        //}

        // Let the user choose an area. (n-column chooser)
        // This function displays a window with a list of areas
        // and allows the user to choose an area from the list.
        //      flags - see kernwin.hpp for choose() flags description and callbacks usage
        //      ncol  - number of columns
        //      widths- widths of each column in characters (may be NULL)
        //      getl  - callback function to get text representation of an area
        //                      obj - pointer to area control block
        //                      n   - (number of area + 1). if n==0 then getl() should
        //                            return text of a header line.
        //                      arrptr - array of buffers for the text representation
        //      title - title of the window.
        //      icon  - number of icon to display
        //      defea - address which points to the default area. The cursor will be
        //              position to this area.
        //      (x0,y0,x1,y1) - window position on the screen
        //                      -1 values specify default window position
        //                      (txt:upper left corner of the screen)
        //                      (gui:centered on the foreground window)
        //      popup_menus - default is insert, delete, edit, refresh
        // returns: NULL - the user cancelled the selection
        //     otherwise - pointer to the selected area.
        // TODO
        //internal delegate void Getline2Delegate(AreaControlBlock obj, uint n, string[] arrptr);
        //internal Area ChooseAreaEx(int flags, int ncol, int[] widths, Getline2Delegate getl,
        //    string title, int icon, int x0, int y0, int x1, int y1, string[] popup_menus,
        //    EffectiveAddress defea)
        //{
        //    return areacb_t_choose_area2(NativePointer, flags, ncol, widths, getl, title, icon,
        //        x0, y0, x1, y1, popup_menus, defea);
        //}

        // Create area information node in Btree.
        // This function usually is used when a new file is loaded into the database.
        // See link() for explanations of input parameteres.
        // This function properly terminates work with the previous area control
        // block in Btree if the current class was used to access information.
        // returns:1-ok
        //         0-failure (Btree already contains node with the specified name)
        internal bool Create(string file, string name, uint infosize)
        {
            return areacb_t_create(NativePointer, file, name, infosize);
        }

        // Create an area.
        // The new area should not overlap with existing ones.
        //      info  - structure containing information about a new area
        //              startEA and endEA specify address range.
        //              startEA should be lower than endEA
        // returns 1-ok,0-failure, area overlaps with another area or bad address range.
        internal bool CreateArea(Area info)
        {
            return areacb_t_create_area(NativePointer, info.NativePointer);
        }

        // Delete an area.
        //      ea     - any address in the area
        //      delcmt - delete area comments
        //               you may choose not to delete comments if you want to
        //               create a new area with the same start address immediately.
        //               In this case the new area will inherit old area comments.
        // returns 1-ok,0-failure (no such area)
        internal bool DeleteArea(EffectiveAddress ea, bool delcmt)
        {
            return areacb_t_del_area(NativePointer, ea, delcmt);
        }

        // Call a function for all areas in the specified range
        // Stop the enumeration if the function returns non-zero
        // Returns: 0 if all areas were visited, otherwise the code returned
        //          by the callback
        // TODO
        //internal delegate int AreaVisitorDelegate(IntPtr /* area_t */ a);
        //internal int ForAllAreas2(EffectiveAddress ea1, EffectiveAddress ea2, AreaVisitorDelegate av)
        //{
        //    return areacb_t_for_all_areas2(NativePointer, ea1, ea2, av);
        //}

        /// <summary>Get the area instance bound to this control block that hosts the
        /// given effective address.</summary>
        /// <param name="address"></param>
        /// <returns>An area instance or a null reference if the current control block
        /// doesn't contain any area matching the address.</returns>
        internal Area GetArea(EffectiveAddress address)
        {
            IntPtr nativeArea = areacb_t_get_area(NativePointer, address);

            return (IntPtr.Zero == nativeArea) ? null : _factory(nativeArea);
        }

        // Get number of area by address
        //      ea - any address in the area
        // returns -1: no area occupies the specified address
        //         otherwise returns number of the specified area (0..get_area_qty()-1)
        internal int GetAreasCount(EffectiveAddress ea)
        {
            return areacb_t_get_area_num(NativePointer, ea);
        }

        // Get the first area.
        // returns: NULL - no areas
        internal Area GetFirstArea()
        {
            IntPtr nativeArea = areacb_t_first_area_ptr(NativePointer);
            return _factory(nativeArea);
        }

        // Get the last area.
        // returns: NULL - no areas
        internal Area GetLastArea()
        {
            IntPtr nativeArea = areacb_t_prev_area_ptr(NativePointer, EffectiveAddress.MaxValue);
            return _factory(nativeArea);
        }

        // Get the next area.
        //      ea - any address in the program
        // returns: NULL - no (more) areas
        internal Area GetNextArea(EffectiveAddress ea)
        {
            IntPtr nativeArea = areacb_t_next_area_ptr(NativePointer, ea);
            return _factory(nativeArea);
        }

        // Get number of the next area.
        // This function returns number of the next (higher in the addressing space)
        // area.
        //      ea - any address in the program
        // returns -1: no (more) areas
        //         otherwise returns number in the range (0..get_area_qty()-1)
        internal int GetNextAreaId(EffectiveAddress ea)
        {
            return areacb_t_get_next_area(NativePointer, ea);
        }

        // Get the previous area.
        //      ea - any address in the program
        // returns: NULL - no (more) areas
        internal Area GetPreviousArea(EffectiveAddress ea)
        {
            IntPtr nativeArea = areacb_t_prev_area_ptr(NativePointer, ea);
            return _factory(nativeArea);
        }

        // Get number of the previous area.
        // This function returns number of the previous (lower in the addressing space)
        // area.
        //      ea - any address in the program
        // returns -1: no (more) areas
        //         otherwise returns number in the range (0..get_area_qty()-1)
        internal int GetPreviousAreaId(EffectiveAddress ea)
        {
            return areacb_t_get_prev_area(NativePointer, ea);
        }

        /// <summary>Delete area information in Btree. All information about the current type
        /// of areas is deleted. Deallocate cache.</summary>
        internal void Kill()
        {
            areacb_t_kill(NativePointer);
            return;
        }

        // Link area control block to Btree. Allocate cache, etc.
        // Btree should contain information about the specified areas.
        // After calling this function you may work with areas.
        //      file  - name of input file being disassembled. Doesn't matter if useva==0.
        //              This parameter is used to build name of the file with the virtual array.
        //      name  - name of area information in Btree.
        //              The name should start with "$ " (yes, including a space)
        //              You may use any name you like. For example, area control
        //              block keeping information about separation of program regions
        //              to different output files might be:
        //              "$ here is info about output file areas"
        //      infosize- size of a structure with actual area information
        //              (size of class based on class area_t)
        // This function properly terminates work with the previous area control
        // block in Btree if the current class was used to access information.
        // returns:1-ok,0-failure (no such node in Btree)
        internal bool Link(string file, string name, int infosize)
        {
            return areacb_t_link(NativePointer, file, name, infosize);
        }

        // Make a hole at the specified address by deleting or modifying existing areas
        //      ea1, ea2 - range to clear
        //      create_tail_area - in the case if there is a big area overlapping
        //              the specified range, should it be divided in two areas?
        //              if 'false', then it will be truncated and the tail
        //              will be left without any covering area
        internal void MakeHole(EffectiveAddress ea1, EffectiveAddress ea2, bool create_tail_area)
        {
            areacb_t_make_hole(NativePointer, ea1, ea2, create_tail_area);
            return;
        }

        // Check if the specified area may end at the specified address.
        // This function checks whether the specified area can be changed so
        // that its end address would be 'newend'.
        //      n        - number of area to check
        //      newend   - new end address for the area
        // returns: 1-yes, it can
        //          0-no
        //                the specified area doesn't exist
        //                new end address is lower or equal to start address
        //                the area would overlap with another area
        internal bool MayEndAt(uint n, EffectiveAddress newend)
        {
            return areacb_t_may_end_at(NativePointer, n, newend);
        }

        // Check if the specified area may start at the specified address.
        // This function checks whether the specified area can be changed so
        // that its start address would be 'newstart'.
        //      n        - number of area to check
        //      newstart - new start address for the area
        // returns: 1-yes, it can
        //          0-no
        //                the specified area doesn't exist
        //                new start address is higher or equal to end address
        //                the area would overlap with another area
        internal bool MayStartAt(uint n, EffectiveAddress newstart)
        {
            return areacb_t_may_start_at(NativePointer, n, newstart);
        }

        // Move area information to the specified addresses
        // Returns: 0 if ok, otherwise the code returned by area_mover
        internal delegate int AreaMoverDelegate(IntPtr nativeArea, AddressDifference delta, IntPtr userData);
        internal int MoveAreas(EffectiveAddress from, EffectiveAddress to,
            MemoryChunkSize size, AreaMoverDelegate area_mover, IntPtr userData)
        {
            return areacb_t_move_areas(NativePointer, from, to, size, area_mover, userData);
        }

        // Prepare to create a new area
        // This function checks whether the new area overlap with an existing one
        // and trims an existing area to make enough address range for the creation
        // of the new area. If the trimming of the existing area doesn't make enough
        // room for the new area, the existing area is simply deleted.
        //      start   - start address of the new area
        //      end     - end   address of the new area
        // returns: an adjusted end address for the new area. The end address may
        //          require some adjustment if another (next) area exists and occupies
        //          some addresses from (start..end) range. In this case we don't
        //          delete the existing area but adjust end address of the new area.
        internal EffectiveAddress PrepareCreation(EffectiveAddress start, EffectiveAddress end)
        {
            return areacb_t_prepare_to_create(NativePointer, start, end);
        }

        // Resize adjacent areas simultaneously
        //      n        - number of area to change
        //                 The adjacent (previous) area will be trimmed or expanded
        //                 if it exists and two areas are contiguous.
        //                 Otherwise this function behaves like set_start() function.
        // returns: 1-ok,0-failure
        internal bool ResizeAreas(uint n, EffectiveAddress newstart)
        {
            return areacb_t_resize_areas(NativePointer, n, newstart);
        }

        /// <summary>Flush area control block to Btree.</summary>
        internal void Save()
        {
            areacb_t_save(NativePointer);
            return;
        }

        // Change end address of the area
        //      n        - number of area to change
        //      newend   - new end address for the area
        // This function doesn't modify other areas.
        // returns: 1-ok, area is changed
        //          0-failure
        //                the specified area doesn't exist
        //                new end address is lower or equal to start address
        //                the area would overlap with another area
        internal bool SetEnd(uint n, EffectiveAddress newend)
        {
            return areacb_t_set_end(NativePointer, n, newend);
        }

        // Change start address of the area
        //      n        - number of area to change
        //      newstart - new start address for the area
        // This function doesn't modify other areas.
        // returns: 1-ok, area is changed
        //          0-failure
        //                the specified area doesn't exist
        //                new start address is higher or equal to end address
        //                the area would overlap with another area
        internal bool SetStart(uint n, EffectiveAddress newstart)
        {
            return areacb_t_set_start(NativePointer, n, newstart);
        }

        /// <summary>Flushes all information to Btree, deallocates caches and unlinks area control
        /// block from Btree.</summary>
        internal void Terminate()
        {
            areacb_t_terminate(NativePointer);
            return;
        }

        /// <summary>Update information about area in Btree.This function can't change startEA and
        /// endEA fields. Its only purpose is to update additional characteristics of the area.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal bool Update(Area info)
        {
            return areacb_t_update(NativePointer, info.NativePointer);
        }

        /// <summary>Initialized area control block. You need to link area control block to existing
        /// area information in Btree (link) or to create a new area information structure in Btree
        /// (create).</summary>
        internal void Zeroise()
        {
            areacb_t_zero(NativePointer);
            return;
        }

        // Find previous gap in areas.
        // This function finds a gap between areas. Only enabled addresses
        // (see bytes.hpp for explanations on addressing) are used in the search.
        //      ea - any linear address
        // returns: EffectiveAddress.MaxValue - no previous gap is found
        //      otherwise returns maximal address in the previous gap
        // MISSING
        // internal EffectiveAddress find_prev_gap(EffectiveAddress ea);

        // Find next gap in areas.
        // This function finds a gap between areas. Only enabled addresses
        // (see bytes.hpp for explanations on addressing) are used in the search.
        //      ea - any linear address
        // returns: EffectiveAddress.MaxValue - no next gap is found
        //      otherwise returns start address of the next gap
        // MISSING
        // internal EffectiveAddress find_next_gap(EffectiveAddress ea);
        #endregion

        // This expand to nothing albeit with SWIG.
        // AREA_HELPER_DEFINITIONS(friend)
        internal IntPtr vtable; // Do not forget this for alignemtn purpose.

        // private definitions, should never be directly accessed
        // code in the database
        /* 0x00 - 0x00 */
        internal MemoryChunkSize AreasCode
        {
            get { return MarshalingUtils.GetMemoryChunkSize(NativePointer, 0x00, 0x00); }
        }

        // sizeof info for area
        /* 0x04 - 0x08 */
        internal ushort InformationSize
        {
            get { return MarshalingUtils.GetUShort(NativePointer, 0x08, 0x04); }
        }

        // last request information (internal). Do not use this opaque definition.
        /* 0x06 - 0x0A */
        // internal IntPtr LastRequest // Undocumented lastreq_t type

        // number of used cache entries
        /* 0x0A - 0x0E */
        internal uint UsedCacheEntriesCount
        {
            get { return MarshalingUtils.GetUInt(NativePointer, 0x0E, 0x0A); }
        }

        // sorted array on supvals. For internal use only.
        /* 0x0E - 0x12 */
        // internal IntPtr SortedSupplementaryValues; // Undocumented sarray structure

        // cache of arEffectiveAddress objects. For internal use only.
        /* 0x12 - 0x16 */
        // internal IntPtr cache; // Undocumented lastreq_t structure

        // doubly linked list of area_cache_t. For internal use only.
        /* 0x16 - 0x1A */
        // internal IntPtr head; // Undocumented lastreq_t structure

        // not used
        /* 0x1A - 0x1E */
        // internal IntPtr[] reserved2 = new IntPtr[126];

        // Read callback: read area from the database.
        // This function is called when a (possibly packed) area is read from the database.
        //      packed - stream of packed bytes
        //      ebd    - ptr to the end of the stream
        //      a      - place to put unpacked version of the area
        // This callback may be NULL.
        internal delegate void ReadCallbackDelegate(IntPtr packedBytesStream, IntPtr streamEnd, IntPtr /* area_t */ a);
        // void (__stdcall *read_cb)(const uchar *packed, const uchar *end, area_t *a);

        // Write callback: write area to the database.
        // This function is called when an area is about to be writed to the database.
        // It may pack the the area to the stream of bytes.
        //      a       - area to be written
        //      packbuf - buffer to hold packed version of the area
        //      packend - ptr to the end of packbuf
        // Returns: number of bytes in the packed form
        // This callback may be NULL.
        internal delegate int WriteCallbackDelegate(IntPtr /* area_t */ a, IntPtr packBuffer, IntPtr bufferEnd);
        // size_t (__stdcall *write_cb)(const area_t *a,uchar *packbuf, uchar *packend);

        // Destroy callback: remove an area from the internal cache.
        // This function is called when an area is freed from the cache.
        // This callback may be NULL.
        internal delegate void DeleteFromCacheCallbackDelegate(IntPtr a /* area_t */);
        // void (__stdcall *delcache_cb)(area_t *a);

        // The following three callbacks are used in open_areas_window() function.
        // When the user presses Ctrl-E key the following callback is called
        // to edit the selected area.
        // This callback may be NULL.
        internal delegate int EditCallbackDelegate(IntPtr a /* area_t */);
        // int (__stdcall *edit_cb)(area_t *a);

        // Callback to handle "Del" keystroke in open_areas_window() function
        // This callback may be NULL.
        internal delegate int KillCallbackDelegate(IntPtr /* area_t */ area);
        // int (__stdcall *kill_cb)(area_t *a);

        // Callback to handle "Ins" keystroke in open_areas_window() function
        // This callback may be NULL.
        internal delegate int NewCallbackDelegate();
        // int (__stdcall *new_cb)(void);

        internal int NativeSize
        {
            get
            {
#if __EA64__
                return 0x216;
#else
                return 0x212;
#endif
            }
        }

        #region MANAGED MEMBERS
        private ControlBlockFactoryDelegate _factory;
        #endregion

        #region NATIVE IDA FUNCTIONS
        private delegate string AreaChooserDelegate(IntPtr nativeControlBlock, uint n, string buf);
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr /* area_t */ areacb_t_choose_area(IntPtr nativeControlBlock, int flags, int width,
            AreaChooserDelegate getl, string title, int icon, int x0, int y0, int x1, int y1, string[] popup_menus,
            EffectiveAddress defea);

        private delegate string AreaChooserDelegate2(IntPtr nativeControlBlock, uint n, string[] arrptr);
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr /* area_t */ areacb_t_choose_area2(IntPtr nativeControlBlock,int flags, int ncol,
            int[] widths, AreaChooserDelegate2 getl, string title, int icon, int x0,int y0,int x1,int y1,
            string[] popup_menus, EffectiveAddress defea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_create(IntPtr nativeControlBlock, string file, string name, uint infosize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_create_area(IntPtr nativeControlBlock, IntPtr /* Area */ info);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_del_area(IntPtr nativeControlBlock, EffectiveAddress ea, bool delcmt);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr /* area_t */ areacb_t_first_area_ptr(IntPtr nativeControlBlock);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int areacb_t_for_all_areas(IntPtr nativeControlBlock, EffectiveAddress ea1, EffectiveAddress ea2,
            IntPtr /* area_visitor2_t*/ av);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int areacb_t_for_all_areas2(IntPtr nativeControlBlock, EffectiveAddress ea1, EffectiveAddress ea2,
            IntPtr /* area_visitor2_t */ av);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern uint areacb_t_get_area_qty(IntPtr native);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr areacb_t_get_area(IntPtr areaControlBlock, EffectiveAddress address);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int areacb_t_get_area_num(IntPtr nativeControlBlock, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr areacb_t_getn_area(IntPtr areaControlBlock, uint index);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern int areacb_t_get_next_area(IntPtr areaControlBlock,EffectiveAddress ea);
        
        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern int areacb_t_get_prev_area(IntPtr areaControlBlock, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall)]
        private static extern void areacb_t_kill(IntPtr nativeControlBlock);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_link(IntPtr nativeControlBlock, string file, string name, int infosize);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void areacb_t_make_hole(IntPtr nativeControlBlock, EffectiveAddress ea1, EffectiveAddress ea2, bool create_tail_area);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_may_start_at(IntPtr nativeControlBlock, uint n, EffectiveAddress newstart);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_may_end_at(IntPtr nativeControlBlock, uint n, EffectiveAddress newend);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern int areacb_t_move_areas(IntPtr nativeControlBlock, EffectiveAddress from, EffectiveAddress to,
            MemoryChunkSize size, AreaMoverDelegate area_mover, IntPtr userData);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr /* area_t */ areacb_t_next_area_ptr(IntPtr nativeControlBlock, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern EffectiveAddress areacb_t_prepare_to_create(IntPtr nativeControlBlock, EffectiveAddress start,
            EffectiveAddress end);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr /* area_t */ areacb_t_prev_area_ptr(IntPtr nativeControlBlock, EffectiveAddress ea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_resize_areas(IntPtr nativeControlBlock, uint n, EffectiveAddress newstart);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void areacb_t_save(IntPtr nativeControlBlock);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_set_end(IntPtr nativeControlBlock, uint n, EffectiveAddress newend);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_set_start(IntPtr nativeControlBlock, uint n, EffectiveAddress newstart);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void areacb_t_terminate(IntPtr nativeControlBlock);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool areacb_t_update(IntPtr nativeControlBlock, IntPtr nativeArea);

        [DllImport(InteropConstants.IdaDllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void areacb_t_zero(IntPtr nativeControlBlock);
        #endregion

        // TODO
        //internal struct area_visitor2_t
        //{
        //      virtual int __stdcall visit_area(area_t *a) = 0;
        //      DEFINE_VIRTUAL_DTOR(area_visitor2_t)
        //};
    }
}
