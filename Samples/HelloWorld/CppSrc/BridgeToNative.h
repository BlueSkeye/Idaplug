#pragma once

#include <vcclr.h>

// Those thre includes ar from the IDA SDK.
#include "idp.hpp"
#include "ida.hpp"
#include "loader.hpp"

using namespace std;

#pragma unmanaged
typedef callui_t (__cdecl *PFNTRAMPOLINE)(ui_notification_t what, ...);

#pragma managed
// A definition for a pointer to the function that will forward its call to the
// hosting IDA call_ui exported function. The call_ui function itself is not
// directly exported, thus a little trick is required within the trampoline.
delegate System::Int32 CallUIDelegate(System::Int32 what);

// A class that allow for the managed classes to talk with the unmanaged world.
public ref class BridgeToNative
{
public:
	// Retrieve the one and only instance from this class.
	static BridgeToNative^ get_Singleton(void);

	void Beep();
	// Display the given message in the IDA UI.
	virtual void DisplayMessage(System::String^ message);
	// This is intended to be invoked once by the native bootstrap. At present
	// time this rule is not enforced.
	// TODO : Trigger an error on any invocation but the first one.
	void Initialize(PFNTRAMPOLINE trampoline);

protected:

private:
	BridgeToNative();
	virtual ~BridgeToNative();

	// The one and only instance from this class.
	static BridgeToNative^ _singleton;
	// A pointer at our trampoline function in the Native.cpp source code.
	PFNTRAMPOLINE _trampoline;
};
