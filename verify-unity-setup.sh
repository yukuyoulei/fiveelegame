# Five Elements Unity Setup Verification

echo "========================================="
echo "Five Elements Unity Setup Verification"
echo "========================================="

# Check if symbolic link exists
if [ -L "unity/Assets/Scripts/Shared/FiveElements.Shared" ]; then
    echo "✅ Symbolic link exists"
    echo "   Link target: $(readlink unity/Assets/Scripts/Shared/FiveElements.Shared)"
else
    echo "❌ Symbolic link missing"
    exit 1
fi

# Check if shared models are accessible
if [ -f "unity/Assets/Scripts/Shared/FiveElements.Shared/Models/PlayerStats.cs" ]; then
    echo "✅ PlayerStats.cs accessible via symlink"
else
    echo "❌ PlayerStats.cs not accessible"
    exit 1
fi

if [ -f "unity/Assets/Scripts/Shared/FiveElements.Shared/Models/MapView.cs" ]; then
    echo "✅ MapView.cs accessible via symlink"
else
    echo "❌ MapView.cs not accessible"
    exit 1
fi

# Check if messages are accessible
if [ -f "unity/Assets/Scripts/Shared/FiveElements.Shared/Messages/GameMessages.cs" ]; then
    echo "✅ GameMessages.cs accessible via symlink"
else
    echo "❌ GameMessages.cs not accessible"
    exit 1
fi

# Check Unity assembly definition files
if [ -f "unity/Assets/Scripts/FiveElements.Unity.asmdef" ]; then
    echo "✅ Unity assembly definition exists"
else
    echo "❌ Unity assembly definition missing"
    exit 1
fi

if [ -f "unity/Assets/Scripts/Shared/FiveElements.Shared/AssemblyDefinition.json" ]; then
    echo "✅ Shared assembly definition exists"
else
    echo "❌ Shared assembly definition missing"
    exit 1
fi

# Check WebSocketSharp package configuration
if grep -q "websocket-sharp" "unity/Packages/manifest.json"; then
    echo "✅ WebSocketSharp package configured"
else
    echo "❌ WebSocketSharp package not configured"
    exit 1
fi

# Check setup scripts
if [ -f "setup-symlinks.bat" ]; then
    echo "✅ Windows symlink script exists"
else
    echo "❌ Windows symlink script missing"
    exit 1
fi

if [ -f "setup-symlinks.sh" ]; then
    echo "✅ Linux symlink script exists"
else
    echo "❌ Linux symlink script missing"
    exit 1
fi

echo ""
echo "🎉 All verification checks passed!"
echo "Unity should now be able to compile successfully."
echo ""
echo "Next steps:"
echo "1. Open unity/ directory in Unity Hub"
echo "2. Unity will automatically create symlinks if needed"
echo "3. Compile and run the project"
