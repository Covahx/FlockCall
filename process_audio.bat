@echo off
REM ============================================================
REM FlockCall audio cleanup
REM Cleans and level-matches merganser recordings.
REM Filter chain:
REM   highpass=f=180   removes water/wind rumble below vocal range
REM   afftdn=nr=12     moderate FFT noise reduction
REM   loudnorm         EBU R128 loudness target, matches volumes
REM ============================================================
setlocal

set "ROOT=%~dp0"
set "WORK=%ROOT%app\assets\audio\Work"
set "OUT=%WORK%\Processed"

if not exist "%WORK%" (
  echo Could not find the Work folder at:
  echo   %WORK%
  echo Make sure this .bat file lives in the FlockCall root.
  pause & exit /b 1
)

if not exist "%OUT%" mkdir "%OUT%"

REM ---- Locate ffmpeg -----------------------------------------
set "FFMPEG=ffmpeg"
where ffmpeg >nul 2>&1
if errorlevel 1 (
  if not exist "%ROOT%ffmpeg.exe" (
    echo.
    echo ffmpeg.exe was not found in PATH or in this folder.
    echo.
    echo One-time install:
    echo   1. Open https://www.gyan.dev/ffmpeg/builds/ in a browser
    echo   2. Download "ffmpeg-release-essentials.zip"
    echo   3. Unzip it, find ffmpeg.exe inside the bin folder
    echo   4. Copy ffmpeg.exe into this folder:
    echo        %ROOT%
    echo   5. Double-click this .bat file again
    echo.
    pause & exit /b 1
  )
  set "FFMPEG=%ROOT%ffmpeg.exe"
)

set "FILTERS=highpass=f=180,afftdn=nr=12,loudnorm=I=-16:LRA=7:TP=-1.5"

echo Using ffmpeg: %FFMPEG%
echo Output folder: %OUT%
echo.

echo [1/2] Processing Follow_Me.mp3 ...
"%FFMPEG%" -y -hide_banner -loglevel warning -i "%WORK%\Follow_Me.mp3" -af "%FILTERS%" -ar 44100 -ac 1 -c:a libmp3lame -b:a 128k "%OUT%\Follow_Me_clean.mp3"
if errorlevel 1 goto :err

echo [2/2] Processing Catch_Up_Feed.mp3 ...
"%FFMPEG%" -y -hide_banner -loglevel warning -i "%WORK%\Catch_Up_Feed.mp3" -af "%FILTERS%" -ar 44100 -ac 1 -c:a libmp3lame -b:a 128k "%OUT%\Catch_Up_Feed_clean.mp3"
if errorlevel 1 goto :err

echo.
echo ============================================================
echo Done. Cleaned files are in:
echo   %OUT%
echo.
echo Listen to them first to check they sound right.
echo Then to use them in the app, copy them into:
echo   %ROOT%app\assets\audio\
echo and rename to one of the slot names the app expects.
echo.
echo Suggested mapping:
echo   Follow_Me_clean.mp3       use as  gather_01.mp3
echo   Catch_Up_Feed_clean.mp3   use as  feeding_01.mp3
echo ============================================================
echo.
pause
exit /b 0

:err
echo.
echo Something went wrong. Scroll up to see the ffmpeg error.
pause
exit /b 1
