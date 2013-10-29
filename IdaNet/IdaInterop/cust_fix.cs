using System;

namespace IdaNet.IdaInterop
{
    internal enum cust_fix
    {
        cf_base, // Get fixup base args: EffectiveAddress *answer
        cf_size, // Get fixup size args: int *answer return 2 if *answer has been filled
        cf_desc, // Describe fixup args: char *buf, size_t bufsize return 2 if buf has been filled
        cf_apply, // Apply a fixup args: EffectiveAddress item_start, int opnum return 2 if fixup has been applied to the database
        cf_move, // Relocate the fixup may be called from loader_t.move_segm() args: AddressDifference delta return: nothing
    };
}
