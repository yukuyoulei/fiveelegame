@echo off
REM Five Elements Game Development Scripts for Windows

echo Five Elements Game Development Environment
echo =========================================

if "%1"=="setup" goto setup
if "%1"=="build" goto build
if "%1"=="server" goto server
if "%1"=="test" goto test
goto help

:setup
echo Setting up development environment...
echo Installing .NET dependencies...
dotnet restore
echo Building solution...
dotnet build
echo Development environment setup complete!
goto end

:build
echo Building shared library...
cd src\FiveElements.Shared
dotnet build
cd ..\..
goto end

:server
echo Starting ASP.NET Core server...
cd src\FiveElements.Server
dotnet run
goto end

:test
echo Running tests...
dotnet test
goto end

:help
echo Usage: %0 [COMMAND]
echo.
echo Commands:
echo   setup      - Setup development environment
echo   build      - Build the solution
echo   server     - Start the server
echo   test       - Run tests
echo   help       - Show this help message

:end