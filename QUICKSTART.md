# Quick Start Guide

## 🚀 Run the Game Server

### Option 1: Using Development Script (Batch)
```cmd
dev.bat server
```

### Option 2: Using Development Script (PowerShell)
```powershell
.\dev.ps1 server
```

### Option 3: Direct .NET Command
```cmd
cd src\FiveElements.Server
dotnet run
```

## 🎮 Run the Unity Client

1. Open Unity Hub
2. Create new Unity 2022.3 LTS project
3. Follow the setup guide in `unity/UnitySetup.md`
4. Import the provided scripts
5. Configure the server URL in `NetworkManager.cs`
6. Build and run

## 🧪 Test the System

### Test Server API
```cmd
curl http://localhost:5000/api/game/players
```

### Test WebSocket Connection
Use a WebSocket client to connect to: `ws://localhost:5000/ws`

## 📁 Project Structure
```
FiveElements/
├── src/
│   ├── FiveElements.Shared/     # Shared game logic
│   └── FiveElements.Server/    # ASP.NET Core server
├── unity/                     # Unity client assets
├── dev.bat                    # Windows batch script
├── dev.ps1                    # Windows PowerShell script
├── test-server.bat           # Windows test script
└── README.md                  # Full documentation
```

## 🎯 Game Features
- ✅ Five elements system with generation/overcoming
- ✅ Player training (Body/Mind) with critical hits
- ✅ Infinite 2D world with procedural generation
- ✅ Real-time multiplayer via WebSocket
- ✅ Monster AI with evolution
- ✅ Mineral harvesting and resource management
- ✅ Stamina system with auto-regeneration

## 🔧 Development Tips
- Use `dev.bat setup` or `.\dev.ps1 setup` to install dependencies
- Server logs show connection activity
- Unity console shows client-side errors
- Check `appsettings.json` for configuration options

## 🐛 Common Issues
- **Port 5000 in use**: Change port in `appsettings.json`
- **WebSocket connection failed**: Check firewall and URL
- **Build errors**: Run `dotnet clean && dotnet build`
- **Unity package errors**: Reinstall WebSocketSharp
- **PowerShell execution policy**: Run `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

## 📚 Documentation
- Full documentation: `README.md`
- Unity setup: `unity/UnitySetup.md`
- API documentation: Check code comments

Happy gaming! 🎮