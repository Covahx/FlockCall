@echo off
setlocal
set "ROOT=%~dp0"
set "EXE=%ROOT%dist\FlockCallTestServer.exe"
if exist "%EXE%" (
  start "" "%EXE%"
  exit /b 0
)
echo.
echo  FlockCallTestServer.exe was not found at:
echo     %EXE%
echo.
echo  Build it once: double-click build_FlockCallTestServer.bat
echo  ^(requires the .NET 9 SDK: https://dotnet.microsoft.com/download^)
echo.
pause
exit /b 1
