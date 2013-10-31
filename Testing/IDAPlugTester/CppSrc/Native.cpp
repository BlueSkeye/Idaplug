#include <vcclr.h>
#include <windows.h>
#include <stdio.h>
#include <tchar.h>
#include <objbase.h>
#include <knownfolders.h>
#include <Shobjidl.h>
#include <Shlobj.h>
#include "idp.hpp"
#include "ida.hpp"
#include "loader.hpp"

#include "Logging.h"
#include "Native.h"
#include "BridgeToNative.h"

#using <ManagedPlugin.netmodule>
#using <IdaNet32.dll>
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace IDAPlugTester;
using namespace IdaNet;

#pragma unmanaged
// Define unmanaged function pointers for methods exposed by the IIdaPlugin managed interface.
typedef int (*PfnInitialize)(void);
typedef void (*PfnRun)(void);
typedef void (*PfnTerminate)(void);

// Group abvove function pointers into a structure
typedef struct _UNMANAGED_PLUGIN_WRAPPER UNMANAGED_PLUGIN_WRAPPER, *PUNMANAGED_PLUGIN_WRAPPER;
struct _UNMANAGED_PLUGIN_WRAPPER
{
	PfnInitialize pfnInitialize;
	PfnRun pfnRun;
	PfnTerminate pfnTerminate;
};

// And define an instance of this structure.
UNMANAGED_PLUGIN_WRAPPER _unmanagedPluginWrapper;

/* To set the callback function. The address in ptr2F will be the
address of a method in the Managed Wrapper class and will be assigned
there. In this case it will be address of ActualMethodInWrapper(int );*/
static void InitializeUnmanagedPluginWrapper(PUNMANAGED_PLUGIN_WRAPPER unmanagedPluginWrapper)
{
	memcpy(&_unmanagedPluginWrapper, unmanagedPluginWrapper, sizeof(UNMANAGED_PLUGIN_WRAPPER));
	return;
}

#pragma managed
// Delegates to be invoked from unmanaged code.
public delegate int InitializeDelegate(void);
public delegate void RunDelegate(void);
public delegate void TerminateDelegate(void);

// A structure that wraps the methods defined in the IIdaPlugin interface.
// Delegates within this structure are intended to be invoked from unmanaged
// code. They will actually point at the Plugin object implementation.
[StructLayoutAttribute(LayoutKind::Sequential, CharSet = CharSet::Ansi)]
public ref struct PluginWrapper
{
	[MarshalAsAttribute(UnmanagedType::FunctionPtr)]
	InitializeDelegate^ Initialize;
	[MarshalAsAttribute(UnmanagedType::FunctionPtr)]
	RunDelegate^ Run;
	[MarshalAsAttribute(UnmanagedType::FunctionPtr)]
	TerminateDelegate^ Terminate;
};

// A class that is used by the native code to talk with the managed classes.
class CBridgeToManaged
{
public:
	CBridgeToManaged()
	{
		_plugin = gcnew Plugin();
		/* define the wrapping struct instance declared in Managed_Wrapper_Class */
		pluginWrapper = gcnew PluginWrapper();

		// These are the actual delegates that are contained in the wraping struct
		pluginWrapper->Initialize = gcnew InitializeDelegate(_plugin, &Plugin::Initialize);
		pluginWrapper->Run = gcnew RunDelegate(_plugin, &Plugin::Run);
		pluginWrapper->Terminate = gcnew TerminateDelegate(_plugin, &Plugin::Terminate);

		// The unmanaged structure grouping function pointers for invocation by unmanaged code.
		System::IntPtr unmanagedWrapper = System::Runtime::InteropServices::Marshal::AllocHGlobal(
			System::Runtime::InteropServices::Marshal::SizeOf(PluginWrapper::typeid));
		// PUNMANAGED_PLUGIN_WRAPPER unmanagedWrapper;

		// Convert methods within the plugin wrapper into function pointers.
		Marshal::StructureToPtr(pluginWrapper, unmanagedWrapper, false);
		InitializeUnmanagedPluginWrapper((PUNMANAGED_PLUGIN_WRAPPER)(unmanagedWrapper.ToPointer()));
		return;
	}

	virtual ~CBridgeToManaged()
	{
		if (!_plugin) { return; }
		delete _plugin;
		_plugin = NULL;
		return;
	}

private:
	// The real plugin
	gcroot<Plugin^> _plugin;
	// An instance of the wrapping struct
	gcroot<PluginWrapper^> pluginWrapper;
};

// Initialze the BridgeToNative singleton instance.
// bool InitializeBridgeToNative(void* trampoline)
bool InitializeBridgeToNative(PFNTRAMPOLINE trampoline)
{
	BridgeToNative::get_Singleton()->Initialize(trampoline);
	return true;
}

#pragma unmanaged
// Forward declaration.
static bool Capture(HMODULE from, LPWSTR exportedName, LPVOID& target, bool indirect);
static bool CaptureExportedData();
static LPWSTR GetKnownFolderPath(const GUID folderId);
bool InitializeLogFilePath();

static LPWSTR bootLoader32FileName = TEXT("IdaNet.dll");
static LPWSTR bootLoader64FileName = TEXT("IdaNet64.dll");
void* callUITarget = NULL;
static LPVOID currentProcessor = NULL;
static LPWSTR FailoverLogFilePath = TEXT("C:\\TEMP\\IDAPlugDefaultLog.log");
static LPVOID functions = NULL;
static LPWSTR ida32LibraryName = TEXT("ida.wll");
static LPWSTR ida64LibraryName = TEXT("ida64.wll");
static LPVOID lastAnalyzedInstruction = NULL;
// static LPWSTR logRelativeFilePath = TEXT("\\idaplug\\logs\\IdaNetLoader.log");
static LPWSTR logRelativeFilePath = TEXT("\\MixedPlugin.log");
static LPVOID segments = NULL;
// Name of the event used for synchronization with the .Net plugin.
// TODO : Check whether .Net supports local namespace events.
static TCHAR syncEventName[] = TEXT("Global\\fa480748-250a-4bc3-adbf-2a020eb8fe64");
static LPWSTR targetIDALibraryName = NULL;
static HMODULE thisModule;
static CBridgeToManaged* toManagedBridge = NULL;

#pragma unmanaged
extern "C" void TraceCallUITrampolineInvoke()
{
	TCHAR localBuffer[1024];

	wsprintf(localBuffer, TEXT("CallUI at 0x%X8.\r\n"), callUITarget);
	Log(localBuffer, 0);
	return;
}

// A trampoline to the callui function. This is because the function is not exported
// by itself. What we have is an exported data that is a pointer at the real function.
// This MUST be a pure C function.
extern "C" __declspec(naked) callui_t __cdecl CallUITrampoline(ui_notification_t what,...)
{
	__asm mov eax, callUITarget
	__asm jmp eax
}

// Capture a single exported data from the IDA DLL. The captured data can be an indirect addres
// (such as callui). On return the target value is either the captured data itself or the
// indirect value if the indirect parameter is true.
static bool Capture(HMODULE from, LPWSTR exportedName, LPVOID& target, bool indirect)
{
	TCHAR localBuffer[512];
	char asciiExportedName[512];

	WideCharToMultiByte(CP_ACP, 0, exportedName, -1, asciiExportedName, sizeof(asciiExportedName),
		NULL, NULL);
	target = GetProcAddress(from, asciiExportedName);
	if (NULL == target) {
		_stprintf(localBuffer, TEXT("Can't find '%s' exported data address\r\n"), exportedName);
		Log(localBuffer, GetLastError());
		return false;
	}

	if (!indirect) { _stprintf(localBuffer, TEXT("%s = 0x%08X\r\n"), exportedName, target); }
	else {
		_stprintf(localBuffer, TEXT("%s = 0x%08X --> 0x%08X\r\n"), exportedName, target, *((LPDWORD)target));
		target = (LPVOID)(*((LPDWORD)target));
	}
	Log(localBuffer, 0);
	return true;
}

// Capture a set of exported data from IDA.
static bool CaptureExportedData()
{
	Log(TEXT("Capturing exported data from :\r\n"), 0);
	Log(targetIDALibraryName, 0);
	LogNewLine();
	HMODULE hIdaDll = LoadLibrary(targetIDALibraryName);

	if (NULL == hIdaDll) {
		Log(TEXT("Can't load IDA library\r\n"), GetLastError());
		return false;
	}
	if (!Capture(hIdaDll, TEXT("callui"), callUITarget, true)) { return false; }
	if (!Capture(hIdaDll, TEXT("ph"), currentProcessor, false)) { return false; }
	if (!Capture(hIdaDll, TEXT("funcs"), functions, true)) { return false; }
	if (!Capture(hIdaDll, TEXT("cmd"), lastAnalyzedInstruction, false)) { return false; }
	if (!Capture(hIdaDll, TEXT("segs"), segments, true)) { return false; }
	return true;
}

// Discover whether we are working with the 32 bits or 64 bits version.
static bool DiscoverTargetIDALibrary()
{
	const int bufferSize = MAX_PATH + 1;
	wchar_t* buffer = new wchar_t[bufferSize];
	int moduleNameLength = GetModuleFileName(thisModule, buffer, bufferSize);

	if (!moduleNameLength) {
		Log(TEXT("Failed to retrieve current module path.\r\n"), GetLastError());
		return false;
	}

	// Detect 32 bits / 64 bits flavor.
	if (5 >= moduleNameLength) {
		Log(TEXT("Current module file path length too short.\r\n"), 0);
		return false;
	}
	bool _32bitsFlavor = false;

	if (!_tcscmp(&buffer[moduleNameLength - 4], TEXT(".plw"))) {
		_32bitsFlavor = true;
		targetIDALibraryName = ida32LibraryName;
	}
	else if (!_tcscmp(&buffer[moduleNameLength - 4], TEXT(".p64"))) {
		_32bitsFlavor = false;
		targetIDALibraryName = ida64LibraryName;
	}
	else {
		Log(TEXT("Unrecognized module file name extension.\r\n"), 0);
		return false;
	}
	return true;
}

// Dll entry point.
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID /* lpReserved */)
{
	TCHAR myDllName[MAX_PATH + 1];

	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		thisModule = hModule;
		if (CoInitializeEx(NULL, COINIT_MULTITHREADED)) { return FALSE; }
		if (!InitializeLogFilePath()) { return FALSE; }
		Log(TEXT("DllMain process attach -----------------------------------------\r\n"), 0);
		memset(myDllName, 0, sizeof(myDllName));
		if (!GetModuleFileName(thisModule, myDllName, MAX_PATH)) {
			_tcscpy(myDllName, TEXT("<UNKNOWN>"));
		}
		Log(TEXT("Attaching Dll named :\r\n"), 0);
		Log(myDllName, 0);
		LogNewLine();
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		memset(myDllName, 0, sizeof(myDllName));
		if (!GetModuleFileName(thisModule, myDllName, MAX_PATH)) {
			_tcscpy(myDllName, TEXT("<UNKNOWN>"));
		}
		Log(TEXT("Detaching Dll named :\r\n"), 0);
		Log(myDllName, 0);
		LogNewLine();
		break;
	}
	return TRUE;
}

// Initialize the glocal variable logFilePath with the full path to this plugin log file.
bool InitializeLogFilePath()
{
	TCHAR pathSuffix[] = TEXT("\\idaplug\\logs\\helloworld.log");
	int logFilePathLength = MAX_PATH + 1;
	PWSTR localAppDataPath;

	logFilePath = new TCHAR[logFilePathLength];
	memset(logFilePath, 0, logFilePathLength * sizeof(TCHAR));
	HRESULT hResult = SHGetKnownFolderPath(FOLDERID_LocalAppData, 0, NULL, &localAppDataPath);
	if (S_OK != hResult) {
		_tcscpy(logFilePath, FailoverLogFilePath);
		return true;
	}
	int totalLength = _tcslen(pathSuffix) + _tcslen(localAppDataPath); 
	if (MAX_PATH <= totalLength) {
		_tcscpy(logFilePath, FailoverLogFilePath);
		return true;
	}
	_tcscpy(logFilePath, localAppDataPath);
	_tcscat(logFilePath, pathSuffix);
	return true;
}

// Invoked by IDA once the plugin has been discovered and must be loaded. We initialize
// the .Net framework, starts it.
int idaapi InitializePlugin()
{
	if (!DiscoverTargetIDALibrary()) {
		Log(TEXT("Failed to retrieve IDA library flavor.\r\n"), 0);
		return PLUGIN_SKIP;
	}
	if (!CaptureExportedData()) {
		Log(TEXT("Exports capture failed\r\n"), 0);
		return PLUGIN_SKIP;
	}
	// Initialize the managed to native bridge singleton.
	if (!InitializeBridgeToNative(CallUITrampoline)) {
		Log(TEXT("Bridge to native initialization failed.\r\n"), 0);
		return PLUGIN_SKIP;
	}
	toManagedBridge = new CBridgeToManaged();
	if (!toManagedBridge) {
		Log(TEXT("Bridge to managed world creation failed\r\n"), 0);
		return PLUGIN_SKIP;
	}
	int result = _unmanagedPluginWrapper.pfnInitialize();
	TCHAR localBuffer[256];

	_stprintf(localBuffer, TEXT("Plugin initialized. Answer : %d\r\n", result));
	Log(localBuffer, 0);
	return result;
}

// Effectively runs the plugin. This implies transfering control to the bootloader
// C# class.
void idaapi RunPlugin(int /* arg */)
{
	Log(TEXT("Running HelloWorld plugin\r\n"), 0);
	if (!toManagedBridge) {
		Log(TEXT("No bridge found. Ignoring.\r\n"), 0);
		return;
	}

	// Create an event that will be used for synchronization purpose with the .Net plugin.
	HANDLE hEvent = CreateEvent(NULL, FALSE, FALSE, syncEventName);
	TCHAR localBuffer[1024];
	_stprintf(localBuffer, TEXT("Native sync event %s'.\r\n"), syncEventName);
	Log(localBuffer, 0);
	// TODO : See how to pass arguments.
	_unmanagedPluginWrapper.pfnRun();

	// The current function must not return until the plugin is done. We synchronize using a
	// system event.
	if (WAIT_OBJECT_0 == WaitForSingleObject(hEvent, 300000)) {
		Log(TEXT("Plugin method signaled completion.\r\n"), 0);
	}
	else { Log(TEXT("Plugin method timedout.\r\n"), 0); }

	if (NULL != hEvent) { CloseHandle(hEvent); }
	return;
}

void idaapi TerminatePlugin()
{
	// TODO : We should invoke the plugin to let it know it's terminating.
	if (toManagedBridge) {
		Log(TEXT("Terminating plugin :\r\n"), 0);
		_unmanagedPluginWrapper.pfnTerminate();
		delete toManagedBridge;
	}
	Log(TEXT("Terminating plugin :\r\n"), 0);
}

#pragma managed(pop)
