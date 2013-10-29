using System;

namespace IdaNet.IdaInterop
{
    internal enum idp_notify
    {
        init,                   // The IDP module is just loaded
                                // arg - const char *idp_modname
                                //       processor module name
                                // Processor modules should return zero to indicate failure
        term,                   // The IDP module is being unloaded
        newprc,                 // Before changing proccesor type
                                // arg - int processor number in the array of processor names
                                // return 1-ok,0-prohibit
        newasm,                 // Before setting a new assembler
                                // arg = int asmnum
        newfile,                // A new file is loaded (already)
                                // arg - char * input file name
        oldfile,                // An old file is loaded (already)
                                // arg - char * input file name
        newbinary,              // Before loading a binary file
                                // args:
                                //  char *filename - binary file name
                                //  uint32 fileoff  - offset in the file
                                //  EffectiveAddress basepara  - base loading paragraph
                                //  EffectiveAddress binoff    - loader offset
                                //  uint32 nbytes   - number of bytes to load
        endbinary,              // After loading a binary file
                                //  bool ok        - file loaded successfully?
        newseg,                 // A new segment is about to be created
                                // arg = segment_t *
                                // return 1-ok, 0-segment should not be created
        assemble,               // Assemble an instruction
                                // (display a warning if an error is found)
                                // args:
                                //  EffectiveAddress ea -  linear address of instruction
                                //  EffectiveAddress cs -  cs of instruction
                                //  EffectiveAddress ip -  ip of instruction
                                //  bool use32 - is 32bit segment?
                                //  const char *line - line to assemble
                                //  uchar *bin - pointer to output opcode buffer
                                // returns size of the instruction in bytes
        obsolete_makemicro,     // Generate microcode for the instruction
                                // in 'cmd' structure.
                                // arg - mblock_t *
                                // returns MICRO_... error codes
        outlabel,               // The kernel is going to generate an instruction
                                // label line or a function header
                                // args:
                                //   EffectiveAddress ea -
                                //   const char *colored_name -
                                // If returns value <=0, then the kernel should
                                // not generate the label
        rename,                 // The kernel is going to rename a byte
                                // args:
                                //   EffectiveAddress ea
                                //   const char *new_name
                                // If returns value <=0, then the kernel should
                                // not rename it. See also the 'renamed' event
        may_show_sreg,          // The kernel wants to display the segment registers
                                // in the messages window.
                                // arg - EffectiveAddress current_ea
                                // if this function returns 0
                                // then the kernel will not show
                                // the segment registers.
                                // (assuming that the module have done it)
        closebase,              // The database will be closed now
        load_idasgn,            // FLIRT signature have been loaded
                                // for normal processing (not for
                                // recognition of startup sequences)
                                // arg - const char *short_sig_name
                                // returns: nothing
        coagulate,              // Try to define some unexplored bytes
                                // This notification will be called if the
                                // kernel tried all possibilities and could
                                // not find anything more useful than to
                                // convert to array of bytes.
                                // The module can help the kernel and convert
                                // the bytes into something more useful.
                                // arg:
                                //      EffectiveAddress start_ea
                                // returns: number of converted bytes + 1
        auto_empty,             // Info: all analysis queues are empty
                                // args: none
                                // returns: none
                                // This callback is called once when the
                                // initial analysis is finished. If the queue is
                                // not empty upon the return from this callback,
                                // it will be called later again.
                                // See also auto_empty_finally.
        auto_queue_empty,       // One analysis queue is empty
                                // args: atype_t type
                                // returns: 1-yes, keep the queue empty
                                //        <=0-no, the queue is not empty anymore
                                // This callback can be called many times, so
                                // only the autoMark() functions can be used from it
                                // (other functions may work but it is not tested)
        func_bounds,            // find_func_bounds() finished its work
                                // The module may fine tune the function bounds
                                // args: int *possible_return_code
                                //       func_t *pfn
                                //       EffectiveAddress max_func_end_ea (from the kernel's point of view)
                                // returns: none
        may_be_func,            // can a function start here?
                                // the instruction is in 'cmd'
                                // arg: int state -- autoanalysis phase
                                //   state == 0: creating functions
                                //         == 1: creating chunks
                                // returns: probability 0..100
                                // the idp module is allowed to modify 'cmd'
        is_sane_insn,           // is the instruction sane for the current file type?
                                // arg:  int no_crefs
                                // 1: the instruction has no code refs to it.
                                //    ida just tries to convert unexplored bytes
                                //    to an instruction (but there is no other
                                //    reason to convert them into an instruction)
                                // 0: the instruction is created because
                                //    of some coderef, user request or another
                                //    weighty reason.
                                // The instruction is in 'cmd'
                                // returns: 1-ok, <=0-no, the instruction isn't
                                // likely to appear in the program
        is_jump_func,           // is the function a trivial "jump" function?
                                // args:  func_t *pfn
                                //        EffectiveAddress *jump_target
                                //        EffectiveAddress *func_pointer
                                // returns: 0-no, 1-don't know, 2-yes, see jump_target
                                // and func_pointer
        gen_regvar_def,         // generate register variable definition line
                                // args:  regvar_t *v
                                // returns: 0-ok
        setsgr,                 // The kernel has changed a segment register value
                                // args:  EffectiveAddress startEA
                                //        EffectiveAddress endEA
                                //        int regnum
                                //        SegmentSelector value
                                //        SegmentSelector old_value
                                //        uchar tag (SR_... values)
                                // returns: 1-ok, 0-error
        set_compiler,           // The kernel has changed the compiler information
                                // (inf.cc structure)
        is_basic_block_end,     // Is the current instruction end of a basic block?
                                // This function should be defined for processors
                                // with delayed jump slots. The current instruction
                                // is stored in 'cmd'
                                // args:  bool call_insn_stops_block
                                // returns: 1-unknown, 0-no, 2-yes
        reglink,                // IBM PC only, ignore it
        get_vxd_name,           // IBM PC only, ignore it
                                // Get Vxd function name
                                // args: int vxdnum
                                //       int funcnum
                                //       char *outbuf
                                // returns: nothing

                                // PROCESSOR EXTENSION NOTIFICATIONS
                                // They are used to add support of new instructions
                                // to the existing processor modules.
                                // They should be processed only in notification callbacks
                                // set by hook_to_notification_point(HK_IDP,...)
        custom_ana,             // args: none, the address to analyze is in cmd.ea
                                //   cmd.ip and cmd.cs are initialized too
                                //   cmd.itype must be set >= 0x8000
                                //   cmd.size must be set to the instruction length
                                //   (good plugin would fill the whole 'cmd' including the operand fields)
                                //   in the case of error the cmd structure should be kept intact
                                // returns: 1+cmd.size
        custom_out,             // args: none (cmd structure contains information about the instruction)
                                //   optional notification
                                //   (depends on the processor module)
                                //   generates the instruction text using
                                //   the printf_line() function
                                // returns: 2
        custom_emu,             // args: none (cmd structure contains information about the instruction)
                                //   optional notification. if absent,
                                //   the instruction is supposed to be an regular one
                                //   the kernel will proceed to the analysis of the next instruction
                                // returns: 2
        custom_outop,           // args: op_t *op
                                //   optional notification to generate operand text. if absent,
                                //   the standard operand output function will be called.
                                //   the output buffer is inited with init_output_buffer()
                                //   and this notification may use out_...() functions from ua.hpp
                                //   to form the operand text
                                // returns: 2
        custom_mnem,            // args: char *outbuffer, size_t bufsize (cmd structure contains information about the instruction)
                                //   optional notification. if absent,
                                //   the IDC function GetMnem() won't work
                                // returns: 2
                                // At least one of custom_out or custom_mnem
                                // should be implemented. custom_ana should always be
                                // implemented. These custom_... callbacks will be
                                // called for all instructions. It is the responsability
                                // of the plugin to ignore the undesired callbacks
                                // END OF PROCESSOR EXTENSION NOTIFICATIONS

        undefine,               // An item in the database (insn or data) is being deleted
                                // args: EffectiveAddress ea
                                // returns: >0-ok, <=0-the kernel should stop
                                // if the return value is positive:
                                //   bit0 - ignored
                                //   bit1 - do not delete srareas at the item end
        make_code,              // An instruction is being created
                                // args: EffectiveAddress ea, MemoryChunkSize size
                                // returns: 1-ok, <=0-the kernel should stop
        make_data,              // A data item is being created
                                // args: EffectiveAddress ea, flags_t flags, EffectiveAddress tid, MemoryChunkSize len
                                // returns: 1-ok, <=0-the kernel should stop

        moving_segm,            // May the kernel move the segment?
                                // args: segment_t - segment to move
                                //       EffectiveAddress to   - new segment start address
                                // returns: 1-yes, <=0-the kernel should stop
        move_segm,              // A segment is moved
                                // Fix processor dependent address sensitive information
                                // args: EffectiveAddress from  - old segment address
                                //       segment_t* - moved segment
                                // returns: nothing

        is_call_insn,           // Is the instruction a "call"?
                                // EffectiveAddress ea  - instruction address
                                // returns: 1-unknown, 0-no, 2-yes

        is_ret_insn,            // Is the instruction a "return"?
                                // EffectiveAddress ea  - instruction address
                                // bool strict - 1: report only ret instructions
                                //               0: include instructions like "leave"
                                //                  which begins the function epilog
                                // returns: 1-unknown, 0-no, 2-yes

        get_stkvar_scale_factor,// Should stack variable references be multiplied by
                                // a coefficient before being used in the stack frame?
                                // Currently used by TMS320C55 because the references into
                                // the stack should be multiplied by 2
                                // Returns: scaling factor
                                // Note: PR_SCALE_STKVARS should be set to use this callback

        create_flat_group,      // Create special segment representing the flat group
                                // (to use for PC mainly)
                                // args - EffectiveAddress image_base, int bitness, SegmentSelector dataseg_sel

        kernel_config_loaded,   // This callback is called when ida.cfg is parsed
                                // args - none, returns - nothing

        might_change_sp,        // Does the instruction at 'ea' modify the stack pointer?
                                // args: EffectiveAddress ea
                                // returns: 1-yes, 0-false
                                // (not used yet)

        is_alloca_probe,        // Does the function at 'ea' behave as __alloca_probe?
                                // args: EffectiveAddress ea
                                // returns: 2-yes, 1-false

        out_3byte,              // Generate text representation of 3byte data
                                // init_out_buffer() is called before this function
                                // and all Out... function can be used.
                                // uFlag contains the flags.
                                // This callback might be implemented by the processor
                                // module to generate custom representation of 3byte data.
                                // args:
                                // EffectiveAddress dataea - address of the data item
                                // uint32 value - value to output
                                // bool analyze_only - only create xrefs if necessary
                                //              do not generate text representation
                                // returns: 2-yes, 1-false

        get_reg_name,           // Generate text representation of a register
                                // int reg        - internal register number as defined in the processor module
                                // size_t width   - register width in bytes
                                // char *buf      - output buffer
                                // size_t bufsize - size of output buffer
                                // int reghi      - if not -1 then this function will return the register pair
                                // returns: -1 if error, strlen(buf)+2 otherwise
                                // Most processor modules do not need to implement this callback
                                // It is useful only if ph.regNames[reg] does not provide
                                // the correct register names
        savebase,               // The database is being saved. Processor module should
                                // save its local data
        gen_asm_or_lst,         // Callback: generating asm or lst file
                                // The kernel calls this callback twice, at the beginning
                                // and at the end of listing generation. Processor
                                // module can intercept this event and adjust its output
                                // bool starting - beginning listing generation
                                // FILE *fp      - output file
                                // bool is_asm   - true:assembler, false:listing
                                // int flags     - flags passed to gen_file()
                                // gen_outline_t **outline - ptr to ptr to outline callback
                                // the outline callback, if defined by gen_asm_or_lst,
                                // will be used by the kernel to output the generated lines
                                // returns: nothing
        out_src_file_lnnum,     // Callback: generate analog of
                                //   #line "file.c" 123
                                // directive.
                                // const char *file - source file (may be NULL)
                                // size_t lnnum     - line number
                                // returns: 2-directive has been generated
        get_autocmt,            // Callback: get dynamic auto comment
                                // Will be called if the autocomments are enabled
                                // and the comment retrieved from ida.int starts with
                                // '$!'. 'cmd' contains valid info.
                                // char *buf  - output buffer
                                // size_t bufsize - output buffer size
                                // returns: 2-new comment has been generated
                                //          1-callback has not been handled
                                //            the buffer must not be changed in this case
        is_insn_table_jump,     // Callback: determine if instruction is a table jump or call
                                // If CF_JUMP bit can not describe all kinds of table
                                // jumps, please define this callback.
                                // It will be called for insns with CF_JUMP bit set.
                                // input: cmd structure contains the current instruction
                                // returns: 1-yes, 0-no
        auto_empty_finally,     // Info: all analysis queues are empty definitively
                                // args: none
                                // returns: none
                                // This callback is called only once.
                                // See also auto_empty.
        loader_finished,        // Event: external file loader finished its work
                                // linput_t *li
                                // uint16 neflags
                                // const char *filetypename
                                // Use this event to augment the existing loader functionality
        loader_elf_machine,     // Event: ELF loader machine type checkpoint
                                // linput_t *li
                                // int machine_type
                                // const char **p_procname
                                // proc_def **p_pd (see ldr\elf.h)
                                // set_elf_reloc_t *set_reloc
                                // A plugin check the machine_type. If it is the desired one,
                                // the the plugin fills p_procname with the processor name.
                                // p_pd is used to handle relocations, otherwise can be left untouched
                                // set_reloc can be later used by the plugin to specify relocations
                                // returns: e_machine value (if it is different from the
                                // original e_machine value, procname and p_pd will be ignored
                                // and the new value will be used)
                                // This event occurs for each loaded ELF file
        is_indirect_jump,       // Callback: determine if instruction is an indrect jump
                                // If CF_JUMP bit can not describe all jump types
                                // jumps, please define this callback.
                                // input: cmd structure contains the current instruction
                                // returns: 1-use CF_JUMP, 2-no, 3-yes
        verify_noreturn,        // The kernel wants to set 'noreturn' flags for a function
                                // func_t *pfn
                                // Returns: 1-ok, any other value-do not set 'noreturn' flag
        verify_sp,              // All function instructions have been analyzed
                                // Now the processor module can analyze the stack pointer
                                // for the whole function
                                // input: func_t *pfn
                                // Returns: 1-ok, 0-bad stack pointer
        renamed,                // The kernel has renamed a byte
                                // args:
                                //   EffectiveAddress ea
                                //   const char *new_name
                                //   bool local_name
                                // Returns: nothing. See also the 'rename' event
        add_func,               // The kernel has added a function
                                // args: func_t *pfn
                                // Returns: nothing
        del_func,               // The kernel is about to delete a function
                                // args: func_t *pfn
                                // Returns: 1-ok,<=0-do not delete
        set_func_start,         // Function chunk start address will be changed
                                // args: func_t *pfn
                                //       EffectiveAddress new_start
                                // Returns: 1-ok,<=0-do not change
        set_func_end,           // Function chunk end address will be changed
                                // args: func_t *pfn
                                //       EffectiveAddress new_end
                                // Returns: 1-ok,<=0-do not change
        treat_hindering_item,   // An item hinders creation of another item
                                // args: EffectiveAddress hindering_item_ea
                                //       flags_t new_item_flags (0 for code)
                                //       EffectiveAddress new_item_ea
                                //       MemoryChunkSize new_item_length
                                // Returns: 1-no reaction, <=0-the kernel may delete the hindering item
        str2reg,                // Convert a register name to a register number
                                // args: const char *regname
                                // Returns: register number + 2
                                // The register number is the register index in the regNames array
                                // Most processor modules do not need to implement this callback
                                // It is useful only if ph.regNames[reg] does not provide
                                // the correct register names
        create_switch_xrefs,    // Create xrefs for a custom jump table
                                // in: EffectiveAddress jumpea;        - address of the jump insn
                                //     switch_info_ex_t *; - switch information
                                // returns: must return 2
        calc_switch_cases,      // Calculate case values and targets for a custom jump table
                                // in:  EffectiveAddress insn_ea - address of the 'indirect jump' instruction
                                //      switch_info_ex_t *si      - switch information
                                //      QVector<QVector<sval_t>> casevec - vector of case values...
                                //      evec_t *targets - ...and corresponding target addresses
                                // casevec and targets may be NULL
                                // returns: 2-ok, 1-failed
        determined_main,        // The main() function has been determined
                                // in:  EffectiveAddress main - address of the main() function
                                // returns: none
        preprocess_chart,       // gui has retrieved a function flow chart
                                // in: qflow_chart_t *fc
                                // returns: none
                                // Plugins may modify the flow chart in this callback
        get_bg_color,           // Get item background color
                                // in: EffectiveAddress ea, bgcolor_t *color
                                // Returns: 1-not implemented, 2-color set
                                // Plugins can hook this callback to color disassembly lines
                                // dynamically
        validate_flirt_func,    // flirt has recognized a library function
                                // this callback can be used by a plugin or proc module
                                // to intercept it and validate such a function
                                // args: EffectiveAddress start_ea
                                //       const char *funcname
                                // returns: -1-do not create a function,
                                //           1-function is validated
                                // the idp module is allowed to modify 'cmd'
        get_operand_string,     // Request text string for operand (cli, java, ...)
                                // args: int opnum
                                //       char *buf
                                //       size_t buflen
                                // (cmd structure must contain info for the desired insn)
                                // opnum is the operand number; -1 means any string operand
                                // returns: 1 - no string (or empty string)
                                //         >1 - original string length with terminating zero

                                // the following 5 events are very low level
                                // take care of possible recursion
        add_cref,               // a code reference is being created
                                // args: EffectiveAddress from, EffectiveAddress to, cref_t type
                                // returns: <0 - cancel cref creation
        add_dref,               // a data reference is being created
                                // args: EffectiveAddress from, EffectiveAddress to, dref_t type
                                // returns: <0 - cancel dref creation
        del_cref,               // a code reference is being deleted
                                // args: EffectiveAddress from, EffectiveAddress to, bool expand
                                // returns: <0 - cancel cref deletion
        del_dref,               // a data reference is being deleted
                                // args: EffectiveAddress from, EffectiveAddress to
                                // returns: <0 - cancel dref deletion
        coagulate_dref,         // data reference is being analyzed
                                // args: EffectiveAddress from, EffectiveAddress to, bool may_define, EffectiveAddress *code_ea
                                // plugin may correct code_ea (e.g. for thumb mode refs, we clear the last bit)
                                // returns: <0 - cancel dref analysis
        custom_fixup,           // mutipurpose notification for FIXUP_CUSTOM
                                // args: cust_fix oper, EffectiveAddress ea, const fixup_data_t*, ... (see cust_fix)
                                // returns: 1 - no accepted (fixup ignored by ida)
                                //         >1 - accepted (see cust_fix)
        off_preproc,            // called from get_offset_expr, when refinfo_t
                                // contain flag REFINFO_PREPROC. Normally this
                                // notification used in a combination with custom_fixup
                                // args: EffectiveAddress ea, int numop, EffectiveAddress* opval, const refinfo_t* ri,
                                //       char* buf, size_t bufsize, EffectiveAddress* target,
                                // EffectiveAddress* fullvalue, EffectiveAddress from, int getn_flags
                                // returns: 2 - buf filled as simple expression
                                //          3 - buf filled as complex expression
                                //          4 - apply standard processing (with - possible - changed values)
                                //     others - can't convert to offset expression

        set_proc_options,       // called if the user specified an option string in the command line:
                                //  -p<processor name>:<options>
                                // can be used for e.g. setting a processor subtype
                                // also called if option string is passed to set_processor_type()
                                // and IDC's SetProcessorType()
                                // args: const char * options
                                // returns: <0 - bad option string

        last_cb_before_debugger,
        // START OF DEBUGGER CALLBACKS
        obsolete_get_operand_info = 100, // Get operand information
                                // This callback is used to calculate the operand
                                // value for double clicking on it, hints, etc
                                // EffectiveAddress ea  - instruction address
                                // int n    - operand number
                                // int thread_id - current thread id
                                // const regval_t &(*__stdcall getreg)(const char *name,
                                //                                  const regval_t *regvalues))
                                //                           - function to get register values
                                // const regval_t *regvalues - register values array
                                // idd_opinfo_t *opinf       - the output buffer
                                // returns: 0-ok, otherwise failed

        get_reg_info,           // Get register information by its name
                                // const char *regname
                                // const char **main_regname (NULL-failed)
                                // uint64 *mask - mask to apply to 'main_regname' value (0-no mask)
                                // returns: 1-unimplemented, 0-implemented
                                // example: "ah" returns main_regname="eax" and mask=0xFF00
                                // this callback might be unimplemented if the register
                                // names are all present in ph.regNames and they all have
                                // the same size
        next_exec_insn,         // Get next address to be executed
                                // EffectiveAddress ea                   - instruction address
                                // int tid                   - current therad id
                                // const regval_t &(*__stdcall getreg)(const char *name,
                                //                                  const regval_t *regvalues))
                                //                           - function to get register values
                                // const regval_t *regvalues - register values array
                                // EffectiveAddress *target              - pointer to the answer
                                // This function must return the next address to be executed.
                                // If the instruction following the current one is executed, then it must return EffectiveAddress.MaxValue
                                // Usually the instructions to consider are: jumps, branches, calls, returns
                                // This function is essential if the 'single step' is not supported in hardware
                                // returns: 1-unimplemented, 0-implemented

        calc_step_over,         // Calculate the address of the instruction which will be
                                // executed after "step over". The kernel will put a breakpoint there.
                                // If the step over is equal to step into or we can not calculate
                                // the address, return EffectiveAddress.MaxValue.
                                // EffectiveAddress ip - instruction address
                                // EffectiveAddress *target - pointer to the answer
                                // returns: 1-unimplemented, 0-implemented

        get_macro_insn_head,    // Calculate the start of a macro instruction
                                // This notification is called if IP points to the middle of an instruction
                                // EffectiveAddress ip - instruction address
                                // EffectiveAddress *head - answer, EffectiveAddress.MaxValue means normal instruction
                                // returns: 1-unimplemented, 0-implemented

        get_dbr_opnum,          // Get the number of the operand to be displayed in the
                                // debugger reference view (text mode)
                                // EffectiveAddress ea - instruction address
                                // int *opnum - operand number (out, -1 means no such operand)
                                // returns: 1-unimplemented, 0-implemented

        insn_sets_tbit,         // Check if the instruction will set the trace bit
                                // given the current memory and register contents
                                // EffectiveAddress ea - instruction address
                                // const regval_t &(*__stdcall getreg)(const char *name,
                                //                                  const regval_t *regvalues))
                                //                           - function to get register values
                                // const regval_t *regvalues - register values array
                                // returns: 1-no, 2-yes, 3-causes bpt exception (int3)
        get_operand_info,       // Get operand information. See the description above
        calc_next_eas,          // Calculate list of addresses the instruction in cmd
                                // may pass control to.
                                // bool over - calculate for step over (ignore call targets)
                                // EffectiveAddress *res - array for the results.
                                //             This array has NEXTEAS_ANSWER_SIZE elements.
                                // int *nsubcalls - number of addresses of called functions
                                //                  in the above array. they must be put
                                //                  at the beginning of the array.
                                //                  if over=true, this answer will be zero.
                                // returns: number of calculated addresses+1
                                // If there are too many addresses or they are
                                // incalculable (indirect jumps, for example), return -1.
                                // This callback is required for source level debugging.

        // END OF DEBUGGER CALLBACKS

        // START OF TYPEINFO CALLBACKS
                                // The codes below will be called only if
                                // PR_TYPEINFO is set
                                // ALL OF THEM UP TO calc_arglocs2 SHOULD BE IMPLEMENTED IN THIS CASE!!!
                                // (setup_til and calc_purged_bytes are optional)

        decorate_name=500,      // Decorate/undecorate a C symbol name
                                // const til_t *ti    - pointer to til
                                // const char *name   - name of symbol
                                // const type_t *type - type of symbol. If NULL then it will try to guess.
                                // char *outbuf       - output buffer
                                // size_t bufsize     - size of the output buffer
                                // bool mangle        - true-mangle, false-unmangle
                                // byte cc            - real calling convention for VOIDARG functions
                                // returns: true if success

        setup_til,              // Setup default type libraries (called after loading
                                // a new file into the database)
                                // The processor module may load tils, setup memory
                                // model and perform other actions required to set up
                                // the type system
                                // args:    none
                                // returns: nothing
                                // Optional callback

        based_ptr,              // get prefix and size of 'segment based' ptr
                                // type (something like char _ss *ptr)
                                // see description in typeinf.hpp
                                // args:  unsigned ptrt
                                //        const char **ptrname (output arg)
                                // returns: size of type

        max_ptr_size,           // get maximal size of a pointer in bytes
                                // args:  none
                                // returns: max possible size of a pointer plus 1

        get_default_enum_size,  // get default enum size
                                // args:  byte cm
                                // returns: sizeof(enum)

        OBSOLETE_calc_arglocs, // See calc_arglocs2

        use_stkarg_type,        // use information about a stack argument
                                // args:    EffectiveAddress ea            - address of the push instruction which
                                //                               pushes the function argument into the stack
                                //          const type_t *type - the function argument type
                                //          const char *name   - the function argument name. may be NULL
                                // returns: true - ok, false - failed, the kernel will create
                                //          a comment with the argument name or type for the instruction

        OBSOLETE_use_regarg_type,
        OBSOLETE_use_arg_types,
        OBSOLETE_get_fastcall_regs,
        OBSOLETE_get_thiscall_regs,
        OBSOLETE_calc_cdecl_purged_bytes,
        OBSOLETE_get_stkarg_offset,

        calc_purged_bytes,      // calculate number of purged bytes by the given function type
                                // args: type_t *type - must be function type
                                // returns: number of bytes purged from the stack + 2
                                // Optional callback

        calc_arglocs2,          // calculate function argument locations
                                // args:    const type_t *type - points to the return type of the function type string
                                //          byte cc        - calling convention
                                //          uint32 *arglocs - the result array
                                // the arglocs array is big enough to store
                                // argument location information (it is at least nargs+1 elements)
                                // This callback supersedes calc_argloc.
                                // returns: 1-not implemented, 2-ok, -1-error
                                // this callback is never called for CM_CC_SPECIAL functions

        calc_retloc,            // calculate return value location
                                // args:   const type_t *rettype
                                //         byte cc
                                //         unit32 *retloc - the result
                                // returns: 1-not implemented, 2-ok, -1-error

        calc_varglocs,          // calculate locations of the arguments that correspond to '...'
                                // args:const func_type_info_t *fti - function type
                                //      int nargs  - number of actual arguments
                                //      const type_t *const *argtypes - type of each actual argument.
                                //                                      may be NULL if types are unknown
                                //      argloc_t *arglocs- in: size of each actual argument
                                //                        out: argloc of each actual argument
                                //                             argloc array contains one extra element
                                //                             to store the total size of all stack arguments
                                //      regobjs_t *regargs - register arguments
                                //      relobj_t *stkargs  - stack arguments
                                // Note: fixed arguments are not present in argtypes and arglocs
                                // returns: 1-not implemented, 2-ok, -1-error

        OBSOLETE_get_varcall_regs,

        use_regarg_type2,       // use information about register argument
                                // args:
                                //      int *retidx          - pointer to the returned value
                                //      EffectiveAddress ea              - address of the instruction
                                //      const type_t * const * - array of argument types
                                //      const char * const * - array of argument names
                                //      const uint32 *       - array of register numbers
                                //      int n                - number of register arguments
                                // at the end, *retidx contains:
                                //   key of the used argument - if the argument is defined in the current instruction
                                //                              a comment will be applied by the kernel
                                //   key|REG_SPOIL            - argument is spoiled by the instruction
                                //   -1                       - if the instruction doesn't change any registers
                                //   -2                       - if the instruction spoils all registers
                                // returns: 2

        use_arg_types2,         // use information about callee arguments
                                // args:EffectiveAddress ea              - address of the call instruction
                                //      const type_t * const * - array of all argument types
                                //      const char * const * - array of all argument names
                                //      const uint32 *       - array of argument locations
                                //      int n                - number of all arguments
                                //      const type_t **      - array of register argument types
                                //      const char **        - array of register argument names
                                //      uint32 *             - array of register numbers
                                //      int *rn              - number of register arguments
                                // returns: 2 (and updates *rn)
                                // this callback will be used only if PR_USE_ARG_TYPES is set

        get_fastcall_regs2,     // get array of registers used in the fastcall calling convention
                                // the array is -1 terminated
                                // args: const int ** - place to put the pointer into the array
                                // returns: number_of_fastcall_regs+2

        get_thiscall_regs2,     // get array of registers used in the thiscall calling convention
                                // the array is -1 terminated
                                // args: const int ** - place to put the pointer into the array
                                // returns: number_of_thiscall_regs+2

        get_varcall_regs2,      // get array of registers used in the ellipsis (...) calling convention
                                // the array is -1 terminated
                                // args: const int ** - place to put the pointer into the array
                                // returns: number_of_varcall_regs+2, *args is filled.

        calc_cdecl_purged_bytes2,// calculate number of purged bytes after call
                                // args: EffectiveAddress - address of the call instruction
                                // returns: number of purged bytes+2 (usually add sp, N)

        get_stkarg_offset2,     // get offset from SP to the first stack argument
                                // args: none
                                // returns: the offset
                                // for example: pc: 0, hppa: -0x34, ppc: 0x38
        
        til_for_file,           // internal notification, do not use

                                // END OF TYPEINFO RELATED NOTIFICATIONS

        // END OF TYPEINFO CALLBACKS

        loader=1000,            // this code and higher ones are reserved
                                // for the loaders.
                                // the arguments and the return values are
                                // defined by the loaders
    }
}
