#include "BridgeToNative.h"
#include "Logging.h"

#pragma managed
BridgeToNative::BridgeToNative()
{
	return;
}

BridgeToNative::~BridgeToNative()
{
	return;
}

// Retrieve the one and only instance of this class.
BridgeToNative^ BridgeToNative::get_Singleton(void)
{
	if (!_singleton) {
		// TODO : Make this thread-safe.
		_singleton = gcnew BridgeToNative();
	}
	return _singleton;
}

void BridgeToNative::Beep()
{
	this->_trampoline((ui_notification_t)22, 0);
	return;
}

void BridgeToNative::DisplayMessage(System::String^ message)
{
	const char* nativeMessage = (const char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(message).ToPointer();

	this->_trampoline((ui_notification_t)23, nativeMessage);
	return;
}

void BridgeToNative::Initialize(PFNTRAMPOLINE trampoline)
{
	this->_trampoline = trampoline;
	return;
}
