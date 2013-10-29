using System;
using System.Collections.Generic;
using System.Text;

namespace IdaNet.IdaInterop
{
    /// <summary>This enum definition must be kept in sync with constant values
    /// defined in the native loader for use with the native GetExportedData function.
    /// </summary>
    internal enum ExportedDataIdentifier
    {
        ExportedSegmentsId = 1,
        ExportedFunctionsId = 2,
        ExportedCommandId = 3,
        ExportedCurrentProcessor = 4,
    }
}
