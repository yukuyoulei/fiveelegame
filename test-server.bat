@echo off
echo Testing Five Elements Game Server...
echo ==================================

echo Starting server...
cd src\FiveElements.Server
start /B dotnet run
timeout /t 3 /nobreak > nul

echo Testing server endpoint...
curl -f http://localhost:5000/api/game/players > nul 2>&1

if %errorlevel% equ 0 (
    echo ✓ Server is running and responding!
) else (
    echo ✗ Server is not responding correctly
)

echo Stopping server...
taskkill /F /IM dotnet.exe > nul 2>&1

echo Test completed!