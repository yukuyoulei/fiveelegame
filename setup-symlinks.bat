@echo off
echo Creating symbolic links for shared code...

REM Get the directory of this batch file
set BATCH_DIR=%~dp0
set PROJECT_ROOT=%BATCH_DIR%.

REM Check if Unity Shared directory exists, if not create it
if not exist "%PROJECT_ROOT%\unity\Assets\Scripts\Shared" (
    echo Creating Unity Shared directory...
    mkdir "%PROJECT_ROOT%\unity\Assets\Scripts\Shared"
)

REM Remove existing symbolic link or directory if it exists
if exist "%PROJECT_ROOT%\unity\Assets\Scripts\Shared\FiveElements.Shared" (
    echo Removing existing FiveElements.Shared link...
    rmdir "%PROJECT_ROOT%\unity\Assets\Scripts\Shared\FiveElements.Shared"
)

REM Create symbolic link to the shared project
echo Creating symbolic link to FiveElements.Shared...
mklink /D "%PROJECT_ROOT%\unity\Assets\Scripts\Shared\FiveElements.Shared" "%PROJECT_ROOT%\src\FiveElements.Shared"

if %ERRORLEVEL% EQU 0 (
    echo Symbolic link created successfully!
) else (
    echo Failed to create symbolic link. Please run as Administrator.
    echo You can also manually copy the files from src\FiveElements.Shared to unity\Assets\Scripts\Shared\FiveElements.Shared
)

pause
