#include "Logging.h"

#pragma unmanaged
LPWSTR logFilePath = NULL;
static char UTF16BOM[] = { 0xFF, 0xFE };
static LPWSTR LoggedMessageMissed = TEXT("Ill-formed message failed to be logged.");

// Must set this in an isolated function for exception handling mixed form avoidance (C2713)
static void GuardedWrite(HANDLE hFile, TCHAR* message)
{
	DWORD bytesWriten;

	try { WriteFile(hFile, message, _tcslen(message) * sizeof(TCHAR), &bytesWriten, NULL); }
	catch (...) {
		WriteFile(hFile, LoggedMessageMissed, _tcslen(LoggedMessageMissed), &bytesWriten, NULL);
	}
	return;
}

// Log a message into the user log file.
void Log(TCHAR* message, HRESULT lastError)
{
	if (!logFilePath || !(*logFilePath)) { return; }
	HANDLE hFile = CreateFile(logFilePath, GENERIC_WRITE, 0, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);

	if (INVALID_HANDLE_VALUE == hFile) { return; }
	__try {

		if (0 == SetFilePointer(hFile, 0, NULL, FILE_END)) {
			DWORD bytesWriten;
		
			WriteFile(hFile, UTF16BOM, sizeof(UTF16BOM), &bytesWriten, NULL);
		}
		// This must be guarded otherwise we may incur an error if the message buffer is
		// ill-formed.
		GuardedWrite(hFile, message);

		if (0 != lastError) {
			TCHAR localBuffer[512];

			_stprintf(localBuffer, TEXT("HRESULT = 0x%08X\r\n"), lastError);
			GuardedWrite(hFile, localBuffer);
		}
	}
	__finally { CloseHandle(hFile); }
}

void LogNewLine()
{
	Log(TEXT("\r\n"), 0);
	return;
}
