#!/bin/bash

# Five Elements Game Development Scripts

echo "Five Elements Game Development Environment"
echo "========================================="

# Function to start server
start_server() {
    echo "Starting ASP.NET Core server..."
    cd src/FiveElements.Server
    dotnet run
}

# Function to build shared library
build_shared() {
    echo "Building shared library..."
    cd src/FiveElements.Shared
    dotnet build
}

# Function to run tests
run_tests() {
    echo "Running tests..."
    dotnet test
}

# Function to setup development environment
setup_dev() {
    echo "Setting up development environment..."
    
    # Install .NET dependencies
    echo "Installing .NET dependencies..."
    dotnet restore
    
    # Build solution
    echo "Building solution..."
    dotnet build
    
    echo "Development environment setup complete!"
}

# Function to show help
show_help() {
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  setup      - Setup development environment"
    echo "  build      - Build the solution"
    echo "  server     - Start the server"
    echo "  test       - Run tests"
    echo "  help       - Show this help message"
}

# Main script logic
case "$1" in
    "setup")
        setup_dev
        ;;
    "build")
        build_shared
        ;;
    "server")
        start_server
        ;;
    "test")
        run_tests
        ;;
    "help"|*)
        show_help
        ;;
esac