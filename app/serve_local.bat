@echo off
cd /d "%~dp0"
set PORT=8765
echo.
echo  FlockCall — open in Edge:
echo     http://localhost:%PORT%/
echo.
echo  Press Ctrl+C here to stop the server.
echo.
where py >nul 2>&1
if %errorlevel%==0 (
  py -m http.server %PORT%
  exit /b 0
)
where python >nul 2>&1
if %errorlevel%==0 (
  python -m http.server %PORT%
  exit /b 0
)
echo No Python found. Install from https://www.python.org/ ^(check "Add to PATH"^)
echo or install Node.js and run: npx --yes serve -l %PORT%
pause
exit /b 1
