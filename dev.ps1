# Five Elements Game Development Scripts for PowerShell

Write-Host "Five Elements Game Development Environment" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("setup", "build", "server", "test", "help")]
    [string]$Command = "help"
)

switch ($Command) {
    "setup" {
        Write-Host "Setting up development environment..." -ForegroundColor Green
        Write-Host "Installing .NET dependencies..." -ForegroundColor Yellow
        dotnet restore
        
        Write-Host "Building solution..." -ForegroundColor Yellow
        dotnet build
        
        Write-Host "Development environment setup complete!" -ForegroundColor Green
    }
    
    "build" {
        Write-Host "Building shared library..." -ForegroundColor Green
        Set-Location "src\FiveElements.Shared"
        dotnet build
        Set-Location "..\.."
    }
    
    "server" {
        Write-Host "Starting ASP.NET Core server..." -ForegroundColor Green
        Set-Location "src\FiveElements.Server"
        dotnet run
    }
    
    "test" {
        Write-Host "Running tests..." -ForegroundColor Green
        dotnet test
    }
    
    "help" {
        Write-Host "Usage: .\dev.ps1 [COMMAND]" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Commands:" -ForegroundColor White
        Write-Host "  setup      - Setup development environment" -ForegroundColor Gray
        Write-Host "  build      - Build the solution" -ForegroundColor Gray
        Write-Host "  server     - Start the server" -ForegroundColor Gray
        Write-Host "  test       - Run tests" -ForegroundColor Gray
        Write-Host "  help       - Show this help message" -ForegroundColor Gray
    }
}