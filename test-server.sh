#!/bin/bash

echo "Testing Five Elements Game Server..."
echo "=================================="

# Start server in background
echo "Starting server..."
cd src/FiveElements.Server
timeout 10s dotnet run &
SERVER_PID=$!

# Wait a moment for server to start
sleep 3

# Test if server is responding
echo "Testing server endpoint..."
curl -f http://localhost:5000/api/game/players > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✓ Server is running and responding!"
else
    echo "✗ Server is not responding correctly"
fi

# Kill the server
echo "Stopping server..."
kill $SERVER_PID 2>/dev/null

echo "Test completed!"