@echo off
setlocal
set "ROOT=%~dp0"
set "OUT=%ROOT%dist"
set "PROJ=%ROOT%tools\FlockCallTestServer\FlockCallTestServer.csproj"

where dotnet >nul 2>&1
if errorlevel 1 (
  echo Install the .NET SDK from https://dotnet.microsoft.com/download
  echo Then run this script again.
  pause
  exit /b 1
)

echo Publishing self-contained test server to:
echo   %OUT%
echo.

dotnet publish "%PROJ%" -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSingleFile=true ^
  -p:EnableCompressionInSingleFile=true ^
  -o "%OUT%"

if errorlevel 1 (
  echo Publish failed.
  pause
  exit /b 1
)

echo.
echo Done. Run FlockCall with:
echo   StartFlockCallTest.bat
echo or
echo   %OUT%\FlockCallTestServer.exe
echo.
pause
