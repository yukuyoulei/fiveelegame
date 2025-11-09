# Quick Start Guide

## 🚀 Run the Game Server

### Option 1: Using Development Script
```bash
./dev.sh server
```

### Option 2: Direct .NET Command
```bash
cd src/FiveElements.Server
dotnet run
```

### Option 3: Docker
```bash
docker-compose up
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
```bash
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
├── dev.sh                     # Development helper script
├── docker-compose.yml          # Docker deployment
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
- Use `./dev.sh setup` to install dependencies
- Server logs show connection activity
- Unity console shows client-side errors
- Check `appsettings.json` for configuration options

## 🐛 Common Issues
- **Port 5000 in use**: Change port in `appsettings.json`
- **WebSocket connection failed**: Check firewall and URL
- **Build errors**: Run `dotnet clean && dotnet build`
- **Unity package errors**: Reinstall WebSocketSharp

## 📚 Documentation
- Full documentation: `README.md`
- Unity setup: `unity/UnitySetup.md`
- API documentation: Check code comments

Happy gaming! 🎮