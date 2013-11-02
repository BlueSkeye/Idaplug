#include "idp.hpp"
#include "ida.hpp"
#include "loader.hpp"

#include "Native.h"

#pragma unmanaged
// The global exported structure for use by IDA to load our plugin.
// The all-important exported PLUGIN object
__declspec(dllexport) plugin_t PLUGIN =
{
	IDP_INTERFACE_VERSION, // IDA version plug-in is written for
	PLUGIN_FIX | PLUGIN_HIDE, // Flags (see below)
	InitializePlugin, // Initialisation function
	TerminatePlugin, // Clean-up function
	RunPlugin, // Main plug-in body
	"IDA Plug tester", // Comment – unused
	"IDA Plug tester", // As above – unused
	"IDAPlugTester", // Plug-in name shown in Edit->Plugins menu
	"Alt-X" // Hot key to run the plug-in
};
