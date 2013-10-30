SET LogFile=TestSetup.log
MSIEXEC /i .\bin\Debug\IdaPlug.msi /lvx!* %LogFile%
NOTEPAD %LogFile%
