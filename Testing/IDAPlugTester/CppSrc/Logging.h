#pragma once
#include <windows.h>
#include <tchar.h>

// Path to the plugin log file.
extern LPWSTR logFilePath;

void Log(TCHAR* message, HRESULT lastError);
void LogNewLine(void);
