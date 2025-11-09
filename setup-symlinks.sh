#!/bin/bash

echo "Creating symbolic links for shared code..."

# Get the project root directory
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check if Unity Shared directory exists, if not create it
UNITY_SHARED_DIR="$PROJECT_ROOT/unity/Assets/Scripts/Shared"
if [ ! -d "$UNITY_SHARED_DIR" ]; then
    echo "Creating Unity Shared directory..."
    mkdir -p "$UNITY_SHARED_DIR"
fi

# Remove existing symbolic link or directory if it exists
SHARED_LINK="$UNITY_SHARED_DIR/FiveElements.Shared"
if [ -L "$SHARED_LINK" ]; then
    echo "Removing existing FiveElements.Shared symbolic link..."
    rm "$SHARED_LINK"
elif [ -d "$SHARED_LINK" ]; then
    echo "Removing existing FiveElements.Shared directory..."
    rm -rf "$SHARED_LINK"
fi

# Create symbolic link to the shared project
echo "Creating symbolic link to FiveElements.Shared..."
ln -s "$PROJECT_ROOT/src/FiveElements.Shared" "$SHARED_LINK"

if [ $? -eq 0 ]; then
    echo "Symbolic link created successfully!"
else
    echo "Failed to create symbolic link."
    echo "You can also manually copy the files from src/FiveElements.Shared to unity/Assets/Scripts/Shared/FiveElements.Shared"
    exit 1
fi
