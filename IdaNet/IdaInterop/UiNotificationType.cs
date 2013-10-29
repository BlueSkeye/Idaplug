using System;

namespace IdaNet.IdaInterop
{
    // Events marked as '*' should be used as a parameter to callui()
    // See convenience functions below (like get_screen_ea())
    // Events marked as 'cb' are designed to be callbacks and should not
    // be used in callui(). The user may hook to HT_UI events to catch them
    internal enum UiNotificationType
    {
        ui_null = 0,

        ui_range,             // cb: the disassembly range have been changed (inf.minEA..inf.maxEA)
                            // UI should redraw the scrollbars
                            // See also: lock_range_refresh
                            // Parameters: none
                            // Returns:    none

        ui_list,              // cb: the list (chooser) window contents have been changed
                            // (names, signatures, etc) UI should redraw them
                            // Parameters: none
                            // Returns:    none
                            // Please consider request_refresh() instead

        ui_idcstart,          // cb: Start of IDC engine work
                            // Parameters: none
                            // Returns:    none

        ui_idcstop,           // cb: Stop of IDC engine work
                            // Parameters: none
                            // Returns:    none

        ui_suspend = 5,           // cb: Suspend graphical interface.
                            // Only the text version
                            // interface should response to it
                            // Parameters: none
                            // Returns:    none

        ui_resume,            // cb: Resume the suspended graphical interface.
                            // Only the text version
                            // interface should response to it
                            // Parameters: none
                            // Returns:    none

        ui_old_jumpto,            // * Jump to the specified address
                            // Parameters:
                            //      ea_t ea
                            //      int operand_num (-1: don't change x coord)
                            // Returns: bool success

        ui_readsel,           // * Get the selected area boundaries
                            // Parameters:
                            //      ea_t *startea
                            //      ea_t *endea
                            // Returns: bool
                            //          0 - no area is selected
                            //          1 - ok, startea and endea are filled
                            // See also: ui_readsel2

        ui_unmarksel,         // * Unmark selection
                            // Parameters: none
                            // Returns:    none

        ui_screenea = 10,          // * Return the address at the screen cursor
                            // Parameters: ea_t *result
                            // Returns:    none

        ui_saving,            // cb: The kernel is saving the database.
                            // The user interface should save its state.
                            // Parameters: none
                            // Returns:    none

        ui_saved,             // cb: The kernel has saved the database.
                            // This callback just informs the interface.
                            // Parameters: none
                            // Returns:    none

        ui_refreshmarked,     // * Refresh marked windows
                            // Parameters: none
                            // Returns:    none

        ui_refresh,           // * Refresh all disassembly views
                            // Parameters: none
                            // Returns:    none
                            // Forces an immediate refresh.
                            // Please consider request_refresh() instead

        ui_choose = 15,            // * Allow the user to choose an object
                            // Parameters:
                            //      choose_type_t type
                            //      ...
                            // other parameters depend on the 'type'
                            // see below for inline functions using this
                            // notification code.
                            // Always use the helper inline functions below.
                            // Returns: depends on the 'type'

        ui_close_chooser,     // * Close a non-modal chooser
                            // Parameters:
                            //      const char *title
                            // Returns: bool success

        ui_banner,            // * Show a banner dialog box
                            // Parameters:
                            //      int wait
                            // Returns: bool 1-ok, 0-esc was pressed

        ui_setidle,           // * Set a function to call at idle times
                            // Parameters:
                            //      int (*func)(void);
                            // Returns: none

        ui_noabort,           // * Disable 'abort' menu item - the database was not
                            // compressed
                            // Parameters: none
                            // Returns:    none

        ui_term = 20,              // cb: IDA is terminated
                            // The database is already closed.
                            // The UI may close its windows in this callback.
                            // Parameters: none
                            // Returns:    none

        ui_mbox,              // * Show a message box
                            // Parameters:
                            //      mbox_kind_t kind
                            //      const char *format
                            //      va_list va
                            // Returns: none

        ui_beep,              // * Beep
                            // Parameters:
                            //      beep_t beep_type
                            // Returns:    none

        ui_msg,             // * Show a message in the message window
                            // Parameters:
                            //      const char *format
                            //      va_list va
                            // Returns: number of bytes output

        ui_askyn,             // * Ask the user and get his yes/no response
                            // Parameters:
                            //      const char *yes_button
                            //      const char *no_button
                            //      const char *cancel_button
                            //      int default_answer
                            //      const char *format
                            //      va_list va
                            // Returns: -1-cancel/0-no/1-yes

        ui_askfile = 25,           // * Ask the user a file name
                            // Parameters:
                            //      int savefile
                            //      const char *default_answer
                            //      const char *format
                            //      va_list va
                            // Returns: file name

        ui_form,              // * Show a dialog form
                            // Parameters:
                            //      const char *format
                            //      va_list va
                            // Returns: bool 0-esc, 1-ok

        ui_close_form,        // * Close the form
                            // This function may be called from pushbutton
                            // callbacks in ui_form
                            //      TView *fields[]
                            //      int is_ok
                            // Returns: none

        ui_clearbreak,        // * clear ctrl-break flag
                            // Parameters: none
                            // Returns: none
                            // NB: this call is also used to get ida version

        ui_wasbreak,          // * test the ctrl-break flag
                            // Parameters: none
                            // Returns: 1 - Ctrl-Break is detected, a message is displayed
                            //          2 - Ctrl-Break is detected again, a message is not displayed
                            //          0 - Ctrl-Break is not detected

        ui_asktext = 30,           // * Ask text
                            // Parameters:
                            //      size_t size
                            //      char *answer
                            //      const char *default_value
                            //      const char *format
                            //      va_list va
                            // Returns: the entered text

        ui_askstr,            // * Ask a string
                            // Parameters:
                            //      int history_number
                            //      const char *default_value
                            //      const char *format
                            //      va_list va
                            // Returns: the entered string

        ui_askident,          // * Ask an identifier
                            // Parameters:
                            //      const char *default_value
                            //      const char *format
                            //      va_list va
                            // Returns: cptr the entered identifier

        ui_askaddr,           // * Ask an address
                            // Parameters:
                            //      ea_t *answer
                            //      const char *format
                            //      va_list va
                            // Returns: bool success

        ui_askseg,            // * Ask a segment
                            // Parameters:
                            //      sel_t *answer
                            //      const char *format
                            //      va_list va
                            // Returns: bool success

        ui_asklong = 35,           // * Ask a long
                            // Parameters:
                            //      sval_t *answer
                            //      const char *format
                            //      va_list va
                            // Returns: bool success

        ui_showauto,          // * Show the autoanalysis state
                            // Parameters:
                            //      ea_t ea
                            //      int auto_t (see auto.hpp)
                            // Returns: none

        ui_setstate,          // * Show READY, BUSY, THINKING, etc
                            // Parameters:
                            //      int idastate_t (see auto.hpp)
                            // Returns: int: old ida state

        ui_add_idckey,        // * Add hotkey for IDC function
                            // After this function the UI should call the
                            // specified IDC function
                            // when the hotkey is pressed
                            // Parameters:
                            //      const char *hotkey
                            //      const char *idcfuncname
                            // Returns: int code (See IdcChkKeyCode)

        ui_del_idckey,        // * Delete IDC function hotkey
                            // Parameters:
                            //      hotkey  - hotkey name
                            // Returns: bool success

        ui_old_get_marker = 40,    // * Get pointer to function
                            // "void mark_idaview_for_refresh(ea_t ea)"
                            // This function will be called by the kernel when the
                            // database is changed
                            // Parameters: none
                            // Returns: vptr: (idaapi*marker)(ea_t ea) or NULL
                            // OBSOLETE

        ui_analyzer_options,  // * Allow the user to set analyzer options
                            // (show a dialog box)
                            // Parameters: none
                            // Returns: none

        ui_is_msg_inited,     // * Can we use msg() functions?
                            // Parameters: none
                            // Returns: bool cnd

        ui_load_file,         // Display a load file dialog and load file
                            // Parameters:
                            //      const char *filename
                            //              the name of input file as is
                            //              (if the input file is from library,
                            //               then this is the name in the library)
                            //      linput_t *li
                            //              loader input source
                            //      ushort neflags
                            //              combination of NEF_... bits
                            //              (see loader.hpp)
                            // Returns: bool cnd;

        ui_run_dbg,           // * Load a debugger plugin and run the specified program
                            // Parameters:
                            //      const char *dbgopts - value of the -r command line switch
                            //      const char *exename - name of the file to run
                            //      int argc            - number of arguments for the executable
                            //      char **argv         - argument vector
                            // Returns: bool cnd

        ui_get_cursor = 45,        // * Get the cursor position on the screen
                            // Parameters:
                            //             int *x
                            //             int *y
                            // Returns:    bool cnd
                            //               true: x,y pointers are filled
                            //               false: no disassembly window open

        ui_get_curline,       // * Get current line from the disassemble window
                            // Parameters: none
                            // Returns:    cptr current line with the color codes
                            // (use tag_remove function to remove the color codes)

        ui_get_hwnd,          // * Get HWND of the main IDA window
                            // Parameters: none
                            // Returns:    txt version: NULL
                            //             gui version: HWND
                            //             qt version under windows: HWND
                            // HWND is returned in result.vptr

        ui_copywarn,          // * Display copyright warning
                            // Parameters: none
                            // Returns:    bool yes/no

        ui_getvcl,            // * Get VCL variables
                            // Parameters:
                            //              TApplication **app
                            //              TScreen **screen
                            //              TMouse **mouse
                            // Returns: int sizeof(TApplication)+sizeof(TScreen)+sizeof(TMouse)
                            // The text version fills the pointers with NULLs and returns 0

        ui_idp_event = 50,         // cb: A processor module event has been generated (idp.hpp, idp_notify)
                            // Parameteres:
                            //      ph::idp_notify event_code
                            //      va_list va
                            // Returns:
                            //      int code; code==0 - process the event
                            //                otherwise return code as the result
                            // This event should not be used as a parameter to callui()
                            // The kernel uses it to notify the ui about the events

        ui_lock_range_refresh,// * Lock the ui_range refreshes. The ranges will not
                            // be refreshed until the corresponding unlock_range_refresh
                            // is issued.
                            // See also: unlock_range_refresh
                            // Parameters: none
                            // Returns:    none

        ui_unlock_range_refresh,// * Unlock the ui_range refreshes. If the number of locks
                            // is back to zero, then refresh the ranges.
                            // See also: ui_range
                            // Parameters: none
                            // Returns:    none

        ui_setbreak,          // * set ctrl-break flag
                            // Parameters: none
                            // Returns: none

        ui_genfile_callback,  // cb: handle html generation
                            // parameters: html_header_cb_t **,
                            //             html_footer_cb_t **,
                            //             html_line_cb_t **
                            // returns: nothing

        ui_open_url = 55,          // * open url
                            // Parameters: const char *url
                            // Returns: none

        ui_hexdumpea,         // * Return the current address in a hex view
                            // Parameters: ea_t *result
                            //             int hexdump_num
                            // Returns:    none

        ui_set_xml,           // * set/update one or more XML values. The 'name' element
                            // or attribute (use @XXX for an attribute) is created in
                            // all XML elements returned by the evaluation of the
                            // 'path' XPath expression, and receives the given 'value'.
                            // If 'name' is empty, the returned elements or attributes
                            // are directly updated to contain the new 'value'.
                            // Parameters: const char *path
                            //             const char *name
                            //             const char *value
                            // Returns:    bool

        ui_get_xml,           // * return an XML value by evaluating the 'path' XPath
                            // expression.
                            // Parameters: const char *path
                            //             idc_value_t *value
                            // Returns:    bool

        ui_del_xml,           // * delete XML values corresponding to the evaluation of the
                            // 'path' XPath expression.
                            // Parameters: const char *path
                            // Returns:    bool

        ui_push_xml = 60,          // * push an XML element on a stack whose uppermost element will be
                            // used to evaluate future relative XPath expressions.
                            // Parameters: const char *path
                            // Returns:    bool

        ui_pop_xml,           // * pop the uppermost XML element from the stack.
                            // Parameters: none
                            // Returns:    bool

        ui_get_key_code,      // * get keyboard key code by its name
                            // Parameters: const char *keyname
                            // Returns:    short code

        ui_setup_plugins_menu,// * setup plugins submenu
                            // Parameters: none
                            // Returns:    none

        ui_refresh_navband,   // * refresh navigation band if changed
                            // Parameters: bool force
                            // Returns:    none

        ui_new_custom_viewer = 65, // * create new ida viewer based on place_t (gui)
                            // Parameters:
                            //      const char *title
                            //      TWinControl *parent
                            //      const place_t *minplace
                            //      const place_t *maxplace
                            //      const place_t *curplace
                            //      int y
                            //      void *ud
                            // returns: TCustomControl *

        ui_add_menu_item,     // * add a menu item
                            // Parameters: const char *menupath,
                            //             const char *name,
                            //             const char *hotkey,
                            //             int flags, (See SetMenuFlags)
                            //             menu_item_callback_t *callback,
                            //             void *ud
                            // Returns:    bool

        ui_del_menu_item,     // * del a menu item
                            // Parameters: const char *menupath
                            // Returns:    bool

        ui_debugger_menu_change, // cb: debugger menu modification detected
                            // Parameters: bool enable
                            // enable=true: debugger menu has been added
                            // enable=false: debugger menu will be removed

        ui_get_curplace,      // * Get current place in a custom viewer
                            // Parameters:
                            // TCustomControl *v
                            // bool mouse_position (otherwise keyboard position)
                            // int *x
                            // int *y
                            // returns: place_t *

        ui_create_tform = 70,      // * create a new tform (only gui version)
                            // Parameters: const char *caption
                            //             HWND *handle
                            // If you need the handle of the new window,
                            // you can get it using the 'handle' parameter.
                            // You will need it if you do not use VCL.
                            // If a window with the specified caption exists, return
                            // a pointer to it. 'handle' will be NULL is this case.
                            // Returns: TForm * of a new or existing window
                            // The text version always returns NULL
                            // NB: Do not use 'handle' to populate the window
                            // because it can be destroyed by the user interface
                            // at any time. Also, the handle is invalid at the
                            // form creation time. It is present only for
                            // the compatibility reasons.
                            // Hook to ui_tform_visible event instead.

        ui_open_tform,        // * open tform (only gui version)
                            // Parameters: TForm *form
                            //             int options (See openFormOption)

        ui_close_tform,       // * close tform (only gui version)
                            // Parameters: TForm *form
                            //             int options (See CloseFormOption)
                            // Returns: nothing

        ui_switchto_tform,    // * activate tform (only gui version)
                            // Parameters: TForm *form
                            //             bool take_focus
                            // Returns: nothing

        ui_find_tform,        // * find tform with the specified caption  (only gui version)
                            // Parameters: const char *caption
                            // Returns: TFrom *
                            // NB: this callback works only with the tabbed forms!

        ui_get_current_tform = 75, // * get current tform (only gui version)
                            // Parameters: none
                            // Returns: TFrom *
                            // NB: this callback works only with the tabbed forms!

        ui_get_tform_handle,  // * get tform handle
                            // Parameters: TForm *form
                            // Returns: HWND
                            // tform handles can be modified by the interface
                            // (for example, when the user switch from mdi to desktop)
                            // This function returns the current tform handle.
                            // It is better to hook to the 'ui_tform_visible'
                            // event and populate the window with controls at
                            // that time.

        ui_tform_visible,     // tform is displayed on the screen
                            // Use this event to populate the window with controls
                            // Parameters: TForm *form
                            //             HWND hwnd or QWidget* widget
                            // In unix, always work with QWigdet*
                            // In windows, use HWND if FORM_QWIDGET was not specified in open_tform()
                            // Returns: nothing

        ui_tform_invisible,   // tform is being closed
                            // Use this event to destroy the window controls
                            // Parameters: TForm *form
                            //             HWND hwnd or QWidget* widget
                            // See comment for ui_tform_visible
                            // Returns: nothing

        ui_get_ea_hint,       // cb: ui wants to display a simple hint for an address
                            // Use this event to generate a custom hint
                            // Parameters: ea_t ea
                            //             char *buf
                            //             size_t bufsize
                            // Returns: bool: true if generated a hint
                            // See also more generic ui_get_item_hint

        ui_get_item_hint = 80,     // cb: ui wants to display multiline hint for an item
                            // Parameters: ea_t ea (or item id like a structure or enum member)
                            //             int max_lines -- maximal number of lines
                            //             int *important_lines  -- out: number of important lines
                            //                                           if zero, output is ignored
                            //             qstring *hint  -- the output string
                            // Returns: bool: true if generated a hint
                            // See also more generic ui_get_custom_viewer_hint

        ui_set_nav_colorizer, // * setup navigation band color calculator (gui)
                            // Parameters: nav_colorizer_t *func
                            // Returns: vptr: pointer to old colorizer

        ui_refresh_custom_viewer, // * refresh custom ida viewer
                            // Parameters:
                            // TCustomControl *custom_viewer
                            // returns: nothing

        ui_destroy_custom_viewer, // * destroy custom ida viewer
                            // Parameters:
                            // TCustomControl *custom_viewer
                            // returns: nothing

        ui_jump_in_custom_viewer, // * set cursor position in custom ida viewer
                            // Parameters:
                            // TCustomControl *custom_viewer
                            // place_t *new_position
                            // int x
                            // int y
                            // returns: bool success

        ui_set_custom_viewer_popup = 85, // * clear custom viewer popup menu
                            // TCustomControl *custom_viewer
                            // TPopupMenu *popup (NULL-clear menu)
                            // returns: nothing

        ui_add_custom_viewer_popup, // * add custom viewer popup menu item
                            // TCustomControl *custom_viewer
                            // const char *title
                            // const char *hotkey
                            // menu_item_callback_t *cb
                            // void *ud
                            // returns: nothing

        ui_set_custom_viewer_handlers,
                            // * set handlers for custom viewer events
                            // TCustomControl *custom_viewer
                            // custom_viewer_keydown_t *keyboard_handler
                            // custom_viewer_popup_t *popup_handler
                            // custom_viewer_dblclick_t *dblclick_handler
                            // custom_viewer_curpos_t *curpos_handler
                            // custom_viewer_close_t *close_handler
                            // void *user_data
                            // Any of these handlers may be NULL
                            // returns: nothing
                            // see also: ui_set_custom_viewer_handler

        ui_get_custom_viewer_curline,
                            // * get custom viewer current line
                            // TCustomControl *custom_viewer
                            // bool mouse (current for mouse pointer?)
                            // returns: cptr: const char * or NULL
                            // The returned line is with color codes

        ui_get_current_viewer,// * get current ida viewer (idaview or custom viewer)
                            // returns: TCustomControl *viewer

        ui_is_idaview = 90,        // * is idaview viewer? (otherwise-custom viewer)
                            // TCustomControl *custom_viewer
                            // returns: bool

        ui_get_custom_viewer_hint,
                            // cb: ui wants to display a hint for a viewer (idaview or custom)
                            // TCustomControl *viewer - viewer
                            // place_t *place         - current position in it
                            // int *important_lines  -- out: number of important lines
                            //                               if zero, the result is ignored
                            // qstring *hint -- the output string
                            // Returns: bool: true if generated a hint

        ui_readsel2,          // * Get the selected area boundaries
                            // Parameters:
                            //      TCustomControl *custom_viewer
                            //      twinpos_t *start
                            //      twinpos_t *end
                            // Returns: bool
                            //          0 - no area is selected
                            //          1 - ok, start and end are filled
                            // This is more complex version of ui_readsel.
                            // If you see only the addresses, use ui_readsel.

        ui_set_custom_viewer_range,
                            // * set position range for custom viewer
                            // Parameters:
                            //      TCustomControl *custom_viewer
                            //      const place_t *minplace
                            //      const place_t *maxplace
                            // returns: nothing

        ui_database_inited,   // cb: database initialization has completed
                            // the kernel is about to run idc scripts
                            // Parameters: int is_new_database
                            //             const char *idc_script (maybe NULL)
                            // Returns:    none

        ui_ready_to_run = 95,      // cb: all UI elements have been initialized.
                            // Automatic plugins may hook to this event to
                            // perform their tasks.
                            // Parameters: none
                            // Returns: nothing

        ui_set_custom_viewer_handler,
                            // * set a handler for a custom viewer event
                            // TCustomControl *custom_viewer
                            // custom_viewer_handler_id_t handler_id
                            // void *handler_or_data
                            // returns: old value of the handler or data
                            // see also: ui_set_custom_viewer_handlers

        ui_refresh_chooser,   // * Mark a non-modal custom chooser for a refresh
                            // Parameters:
                            //      const char *title
                            // Returns: bool success

        ui_add_chooser_cmd,   // * add a menu item to a chooser window
                            // const char *chooser_caption
                            // const char *cmd_caption
                            // chooser_cb_t *chooser_cb
                            // int menu_index
                            // int icon
                            // int flags
                            // Returns: bool success

        ui_open_builtin,      // * open a window of a built-in type
                            // int window_type (one of BWN_... constants)
                            // additional params depend on the window type
                            // see below for the inline convenience functions
                            // Returns: TForm * window pointer

        ui_preprocess = 100,        // cb: ida ui is about to handle a user command
                            // const char *name: ui command name
                            //   these names can be looked up in ida[tg]ui.cfg
                            // returns: int 0-ok, nonzero: a plugin has handled the command

        ui_postprocess,       // cb: an ida ui command has been handled

        ui_set_custom_viewer_mode,
                            // * switch between graph/text modes
                            // TCustomControl *custom_viewer
                            // bool graph_view
                            // bool silent
                            // Returns: bool success

        ui_gen_disasm_text,   // * generate disassembly text for a range
                            // ea_t ea1
                            // ea_t ea2
                            // text_t *text
                            // bool truncate_lines (on inf.margin)
                            // returns: nothing, appends lines to 'text'

        ui_gen_idanode_text,  // cb: generate disassembly text for a node
                            // qflow_chart_t *fc
                            // int node
                            // text_t *text
                            // Plugins may intercept this event and provide
                            // custom text for an IDA graph node
                            // They may use gen_disasm_text() for that.
                            // Returns: bool text_has_been_generated

        ui_install_cli = 105,       // * install command line interpreter
                            // cli_t *cp,
                            // bool install
                            // Returns: nothing

        ui_execute_sync,      // * execute code in the main thread
                            // exec_request_t *req
                            // Returns: int code

        ui_enable_input_hotkeys,
                            // * enable or disable alphanumeric hotkeys
                            //   which can interfere with user input
                            // bool enable
                            // Returns: bool new_state

        ui_get_chooser_obj,
                            // * get underlying object of the specified chooser
                            // const char *chooser_caption
                            // Returns: void *chooser_object

        ui_enable_chooser_item_attrs,
                            // * enable item-specific attributes for a chooser
                            // const char *chooser_caption
                            // bool enable
                            // Returns: success

        ui_get_chooser_item_attrs = 110,
                            // cb: get item-specific attributes for a chooser
                            // void *chooser_object
                            // uint32 n
                            // chooser_item_attrs_t *attrs
                            // Returns: nothing
                            // This callback is generated only after enable_chooser_attrs()

        ui_set_dock_pos,      // * sets the docking position of a form
                            // const char *src_form
                            // const char *dest_form
                            // const int orientation (one of DP_XXXX flags See DockPositionFlags)
                            // const int left, top, right, bottom
                            // Returns: boolean 

        ui_get_opnum,         // * get current operand number
                            // Returns int operand number. -1 means no operand

        ui_install_custom_datatype_menu,
                            // * install/remove custom data type menu item
                            // int dtid - data type id
                            // bool install
                            // Returns: success

        ui_install_custom_optype_menu,
                            // * install/remove custom operand type menu item
                            // int fid - format id
                            // bool install
                            // Returns: success

        ui_get_range_marker = 115,  // * Get pointer to function
                            // "void mark_range_for_refresh(ea_t ea, asize_t size)"
                            // This function will be called by the kernel when the
                            // database is changed
                            // Parameters: none
                            // Returns: vptr: (idaapi*marker)(ea_t ea, asize_t) or NULL

        ui_get_highlighted_identifier,
                            // * Returns the highlighted identifier in the current IDAView
                            // char *buf - buffer to copy identifier to
                            // size_t bufsize - buffer size
                            // int flags - currently not used (pass 0)
                            // Returns: bool (false if no identifier is highlighted)

        ui_lookup_key_code,   // * Get shortcut code previously created by get_key_code
                            // Parameters: int key
                            //             int shift
                            //             bool is_qt
                            // Returns:    short code

        ui_load_custom_icon_file,
                            // * Loads an icon and returns its id
                            // Parameters: const char *file_name
                            // Returns: int

        ui_load_custom_icon,  // * Loads an icon and returns its id
                            // Parameters: const void *ptr
                            //             uint len
                            //             const char *format
                            // Returns: int

        ui_free_custom_icon = 120,  // * Frees an icon loaded with ui_load_custom_icon(_file)
                            // Parameters: int icon_id

        ui_process_action,    // * Processes a UI action by name
                            // Parameters: const char *name
                            //             int flags (reserved / not used)
                            //             void *param (reserved / not used)

        // Added after SDK 6.1
        ui_new_code_viewer,   // * Create a code viewer
                            // Parameters: TWinControl *parent
                            //             TCustomControl *custview
                            //             int flags (combination of CDVF_* flags)
                            // returns: TCustomControl *

        //#define CDVF_NOLINES        0x0001    // don't show line numbers
        //#define CDVF_LINEICONS      0x0002    // icons can be drawn over the line control
        //#define CDVF_STATUSBAR      0x0004    // keep the status bar in the custom viewer

        ui_addons,            // * work with registered add-ons

        ui_execute_ui_requests,
                            // * Execute a variable number of UI requests.
                            // (the UI requests will be dispatched in the context of the main thread)
                            // Parameters: ui_request_t *req (the first request)
                            //             ... (variable arg ui_request_t *)
                            //             NULL (to terminate the var arg request list)

        ui_execute_ui_requests_list = 125,
                            // * Execute a list of UI requests
                            // (the UI requests will be dispatched in the context of the main thread)
                            // Parameters: ui_requests_t *

        ui_register_timer,    // * Register a timer
                            // Timer functions are thread-safe and the callback is executed
                            // in the context of the main thread.
                            // Parameters: int interval (in milliseconds)
                            //             int (idaapi *callback)(void *ud)
                            //                 (the callback can return -1 to unregister the timer;
                            //                  any other value >= 0 defines the new interval for the timer)
                            //             void *ud
                            // Returns: qtimer_t (use this handle to unregister the timer)

        ui_unregister_timer,  // * Unregister a timer
                            // Parameters: qtimer_t t (handle to a registered timer)

        ui_take_database_snapshot,
                            // * Take a database snapshot
                            // Parameters: snapshot_t *ss - in/out parameter.
                            //                            - in: description, flags
                            //                            - out: filename, id
                            //             qstring *err_msg - optional error msg buffer
                            // Returns: bool

        ui_restore_database_snapshot,
                            // * Restore a database snapshot
                            // Parameters: const snapshot_t *ss - snapshot instance (see build_snapshot_tree())
                            //             ss_restore_cb_t cb - A callback that will be triggered with a NULL string
                            //             on success and an actual error message on failure.
                            //             void *ud - user data passed to be passed to the callback
                            // Note: This call is asynchronous. When it is completed, the callback will be triggered.
                            // Returns: boolean. False if restoration could not be started (snapshot file was not found).
                            //          If the returned value is True then check if the operation succeeded from the callback.

        ui_set_code_viewer_line_handlers = 130,
                            // * Set handlers for code viewer line events
                            // Parameters: TCustomControl *code_viewer
                            //             code_viewer_lines_click_t *click_handler
                            //             code_viewer_lines_click_t *popup_handler
                            //             code_viewer_lines_click_t *dblclick_handler
                            //             code_viewer_lines_icon_t *drawicon_handler
                            //             code_viewer_lines_linenum_t *linenum_handler
                            //             Any of these handlers may be NULL
                            // Returns: nothing

        ui_refresh_custom_code_viewer,
                            // * Refresh custom code viewer
                            // Parameters: TCustomControl *code_viewer
                            // Returns: nothing

        ui_new_source_viewer, // * Create new source viewer
                            // Parameters: TWinControl *parent
                            //             TCustomControl *custview
                            //             const char *path
                            //             strvec_t *lines
                            //             int lnnum
                            //             int colnum
                            //             itn flags (SVF_... bits)
                            // Returns: source_view_t *

        //#define SVF_COPY_LINES  0x0000   // keep a local copy of '*lines'
        //#define SVF_LINES_BYPTR 0x0001   // remeber the 'lines' ptr. do not make a copy of '*lines'

        ui_get_tab_size,      // * Get the size of a tab in spaces
                            // Parameters: const char *path
                            //             (The path of the source view for which the tab size is requested.
                            //              If NULL, the default size is returned.)
                            // Returns: int

        ui_set_menu_item_icon,// * Set the icon of a menu item
                            // Parameters: const char *item_name
                            //             int icon_id
                            // Returns: bool

        ui_repaint_qwidget = 135,   // * Repaint the widget immediately
                            // Parameters: QWidget *widget
                            // Returns: nothing

        ui_enable_menu_item,  // * Enable or disable a menu item
                            // Parameters: const char *item_name
                            //             bool enable
                            // Returns: bool

        ui_custom_viewer_set_userdata,
                            // * Change place_t user data for a custom view
                            // Parameters: TCustomControl *custom_viewer
                            //             void *user_data
                            // Returns: old user_data

        ui_new_ea_viewer,     // * create new ea viewer based on place_t (gui)
                            // Parameters:
                            //      const char *title
                            //      TWinControl *parent
                            //      const place_t *minplace
                            //      const place_t *maxplace
                            //      const place_t *curplace
                            //      int y
                            //      void *ud
                            //      int flags
                            //      eaviewer_cb_t *fillloc
                            //      eaviewer_cb_t *jumploc
                            //      location_t **loc
                            // returns: TCustomControl *
                            // see also: ui_new_custom_viewer

        ui_jumpto,            // * Jump to the specified address
                            // Parameters:
                            //      ea_t ea
                            //      int operand_num (-1: don't change x coord)
                            //      int uijmp_flags
                            // Returns: bool success

        ui_choose_info = 140,       // * Invoke the chooser with a choose_info_t structure
                            // Parameters:
                            //      chooser_info_t *chi
                            // Returns: see the choose function

        ui_cancel_exec_request,
                            // Cancel a queued exec request
                            // Parameters:
                            //      int req_id - request id
                            // Returns: success

        ui_show_form,         // Show a dockable modeless dialog form
                            // Parameters:
                            //      const char *format
					        //      int flags
                            //      va_list va
                            // Returns: *TForm

        ui_last = 143,              // The last notification code

        // debugger callgates. should not be used directly, see dbg.hpp for details

        ui_dbg_begin = 1000,
        ui_dbg_run_requests = ui_dbg_begin,
        ui_dbg_get_running_request,
        ui_dbg_get_running_notification,
        ui_dbg_clear_requests_queue,
        ui_dbg_get_process_state,
        ui_dbg_start_process,
        ui_dbg_request_start_process,
        ui_dbg_suspend_process,
        ui_dbg_request_suspend_process,
        ui_dbg_continue_process,
        ui_dbg_request_continue_process,
        ui_dbg_exit_process,
        ui_dbg_request_exit_process,
        ui_dbg_get_thread_qty,
        ui_dbg_getn_thread,
        ui_dbg_select_thread,
        ui_dbg_request_select_thread,
        ui_dbg_step_into,
        ui_dbg_request_step_into,
        ui_dbg_step_over,
        ui_dbg_request_step_over,
        ui_dbg_run_to,
        ui_dbg_request_run_to,
        ui_dbg_step_until_ret,
        ui_dbg_request_step_until_ret,
        ui_dbg_get_oldreg_val,                // obsolete
        ui_dbg_set_oldreg_val,                // obsolete
        ui_dbg_request_set_oldreg_val,        // obsolete
        ui_dbg_get_bpt_qty,
        ui_dbg_getn_oldbpt,                   // obsolete
        ui_dbg_get_oldbpt,                    // obsolete
        ui_dbg_add_oldbpt,                    // obsolete
        ui_dbg_request_add_oldbpt,            // obsolete
        ui_dbg_del_oldbpt,                    // obsolete
        ui_dbg_request_del_oldbpt,            // obsolete
        ui_dbg_update_oldbpt,                 // obsolete
        ui_dbg_enable_oldbpt,                 // obsolete
        ui_dbg_request_enable_oldbpt,         // obsolete
        ui_dbg_set_trace_size,
        ui_dbg_clear_trace,
        ui_dbg_request_clear_trace,
        ui_dbg_is_step_trace_enabled,
        ui_dbg_enable_step_trace,
        ui_dbg_request_enable_step_trace,
        ui_dbg_get_step_trace_options,
        ui_dbg_set_step_trace_options,
        ui_dbg_request_set_step_trace_options,
        ui_dbg_is_insn_trace_enabled,
        ui_dbg_enable_insn_trace,
        ui_dbg_request_enable_insn_trace,
        ui_dbg_get_insn_trace_options,
        ui_dbg_set_insn_trace_options,
        ui_dbg_request_set_insn_trace_options,
        ui_dbg_is_func_trace_enabled,
        ui_dbg_enable_func_trace,
        ui_dbg_request_enable_func_trace,
        ui_dbg_get_func_trace_options,
        ui_dbg_set_func_trace_options,
        ui_dbg_request_set_func_trace_options,
        ui_dbg_get_tev_qty,
        ui_dbg_get_tev_info,
        ui_dbg_get_insn_tev_oldreg_val,       // obsolete
        ui_dbg_get_insn_tev_oldreg_result,    // obsolete
        ui_dbg_get_call_tev_callee,
        ui_dbg_get_ret_tev_return,
        ui_dbg_get_bpt_tev_ea,
        ui_dbg_get_reg_value_type,
        ui_dbg_get_process_qty,
        ui_dbg_get_process_info,
        ui_dbg_attach_process,
        ui_dbg_request_attach_process,
        ui_dbg_detach_process,
        ui_dbg_request_detach_process,
        ui_dbg_get_first_module,
        ui_dbg_get_next_module,
        ui_dbg_bring_to_front,
        ui_dbg_get_current_thread,
        ui_dbg_wait_for_next_event,
        ui_dbg_get_debug_event,
        ui_dbg_set_debugger_options,
        ui_dbg_set_remote_debugger,
        ui_dbg_load_debugger,
        ui_dbg_retrieve_exceptions,
        ui_dbg_store_exceptions,
        ui_dbg_define_exception,
        ui_dbg_suspend_thread,
        ui_dbg_request_suspend_thread,
        ui_dbg_resume_thread,
        ui_dbg_request_resume_thread,
        ui_dbg_get_process_options,
        ui_dbg_check_bpt,
        ui_dbg_set_process_state,
        ui_dbg_get_manual_regions,
        ui_dbg_set_manual_regions,
        ui_dbg_enable_manual_regions,
        ui_dbg_set_process_options,
        ui_dbg_is_busy,
        ui_dbg_hide_all_bpts,
        ui_dbg_edit_manual_regions,
        ui_dbg_get_reg_val,
        ui_dbg_set_reg_val,
        ui_dbg_request_set_reg_val,
        ui_dbg_get_insn_tev_reg_val,
        ui_dbg_get_insn_tev_reg_result,
        ui_dbg_register_provider,
        ui_dbg_unregister_provider,
        ui_dbg_handle_debug_event,
        ui_dbg_add_vmod,
        ui_dbg_del_vmod,
        ui_dbg_compare_bpt_locs,
        ui_dbg_save_bpts,
        ui_dbg_old_getn_bpt,              // obsolete
        ui_dbg_old_get_bpt,               // obsolete
        ui_dbg_old_update_bpt,            // obsolete
        ui_dbg_set_bptloc_string,
        ui_dbg_get_bptloc_string,
        ui_dbg_internal_appcall,
        ui_dbg_internal_cleanup_appcall,
        ui_dbg_internal_get_sreg_base,
        ui_dbg_internal_ioctl,

        // Added after 6.1
        ui_dbg_read_memory,
        ui_dbg_write_memory,
        ui_dbg_read_registers,
        ui_dbg_write_register,
        ui_dbg_get_memory_info,
        ui_dbg_get_event_cond,
        ui_dbg_set_event_cond,
        ui_dbg_old_find_bpt,              // obsolete
        ui_dbg_enable_bpt,
        ui_dbg_request_enable_bpt,
        ui_dbg_old_add_bpt,               // obsolete
        ui_dbg_old_request_add_bpt,       // obsolete
        ui_dbg_del_bpt,
        ui_dbg_request_del_bpt,
        ui_dbg_map_source_path,
        ui_dbg_map_source_file_path,
        ui_dbg_modify_source_paths,
        ui_dbg_is_bblk_trace_enabled,
        ui_dbg_enable_bblk_trace,
        ui_dbg_request_enable_bblk_trace,
        ui_dbg_get_bblk_trace_options,
        ui_dbg_set_bblk_trace_options,
        ui_dbg_request_set_bblk_trace_options,

        // trace management
        ui_dbg_load_trace_file,
        ui_dbg_save_trace_file,
        ui_dbg_is_valid_trace_file,
        ui_dbg_set_trace_file_desc,
        ui_dbg_get_trace_file_desc,
        ui_dbg_choose_trace_file,
        ui_dbg_diff_trace_file,
        ui_dbg_graph_trace,
        ui_dbg_get_tev_memory_info,
        ui_dbg_get_tev_event,
        ui_dbg_get_insn_tev_reg_mem,
        
        // breakpoint management (new codes were introduced in v6.3)
        ui_dbg_getn_bpt,
        ui_dbg_get_bpt,
        ui_dbg_find_bpt,
        ui_dbg_add_bpt,
        ui_dbg_request_add_bpt,
        ui_dbg_update_bpt,
        ui_dbg_for_all_bpts,
        ui_dbg_get_tev_ea,
        ui_dbg_get_tev_type,
        ui_dbg_get_tev_tid,
        ui_dbg_get_tev_reg_val,
        ui_dbg_get_tev_reg_mem_qty,
        ui_dbg_get_tev_reg_mem_ea,
        ui_dbg_get_trace_base_address,
        
        // calluis for creating traces from scratch (added in 6.4)
        ui_dbg_set_trace_base_address,
        ui_dbg_add_tev,
        ui_dbg_add_insn_tev,
        ui_dbg_add_call_tev,
        ui_dbg_add_ret_tev,
        ui_dbg_add_bpt_tev,
        ui_dbg_add_debug_event,
        ui_dbg_add_thread,
        ui_dbg_del_thread,
        ui_dbg_add_many_tevs,

        ui_dbg_end,
    }
}
