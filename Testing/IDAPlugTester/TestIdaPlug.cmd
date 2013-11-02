SET LogFile=%TEMP%\IdaPlugTester.LOG
REM "%IDAPATH%\idaw.exe" -A -B -P+ -a "-L%LogFile%" "-S.\IdaPlugTester.IDC" "-o.\NOTEPAD.IDB" -z00030028 "%WinDir%\system32\NOTEPAD.EXE"
XCOPY ".\bin\Debug\IdaPlugTester.plw" "%IDAPATH%\plugins" /Y
"%IDAPATH%\idaw.exe" -A -B -P+ -a "-L%LogFile%" "-S.\IdaPlugTester.IDC" -t -z00030028
NOTEPAD "%LogFile%"
NOTEPAD "%LOCALAPPDATA%\idaplug\logs\IdaPlugTester.LOG"
