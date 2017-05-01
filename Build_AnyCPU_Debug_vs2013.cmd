@echo off
setlocal

set VCVARSALL_BAT=%ProgramFiles%\Microsoft Visual Studio 12.0\VC\vcvarsall.bat
if not "%ProgramFiles(x86)%"=="" set VCVARSALL_BAT=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\VC\vcvarsall.bat
call "%VCVARSALL_BAT%" x86
msbuild /m /fl  SpitOut.sln /p:Configuration=Debug /p:Platform="Any CPU"  /t:Build
endlocal
