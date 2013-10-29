using System;

namespace IdaNet.IdaInterop
{
    internal enum DataCrossReferenceType
    {
        dr_U, // Unknown -- for compatibility with old versions. Should not be used anymore.
        dr_O, // Offset The reference uses 'offset' of data rather than its value OR The reference appeared because the "OFFSET" flag of instruction is set. The meaning of this type is IDP dependent.
        dr_W, // Write access
        dr_R, // Read access
        dr_T, // Text (for forced operands only) Name of data is used in manual operand
        dr_I, // Informational (a derived java class references its base class informatonally)
    }
}
