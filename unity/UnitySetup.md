# Unity Client Setup Guide

## Prerequisites
- Unity 2022.3 LTS or later
- WebSocketSharp package (install via Package Manager)

## Installation Steps

### 1. Import WebSocketSharp
1. Open Unity Package Manager
2. Click "+" button → "Add package from git URL"
3. Enter: `https://github.com/sta/websocket-sharp.git`
4. Wait for package to import

### 2. Project Structure
```
Assets/
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── NetworkManager.cs
│   │   ├── MapManager.cs
│   │   └── UIManager.cs
│   ├── UI/
│   │   ├── MainElementSelectionPanel.cs
│   │   └── GameHUD.cs
│   └── Models/
│       └── (Shared models will be copied here)
├── Scenes/
│   └── MainGame.unity
├── Prefabs/
│   ├── Tile.prefab
│   └── Player.prefab
└── Materials/
    └── TileMaterials/
```

### 3. Scene Setup

1. **Create Main Scene**
   - Create new scene: `MainGame.unity`
   - Save in `Assets/Scenes/`

2. **Create GameManager Object**
   - Create empty GameObject named "GameManager"
   - Attach `GameManager.cs` script
   - Attach `NetworkManager.cs` script
   - Attach `MapManager.cs` script
   - Attach `UIManager.cs` script

3. **Create UI Canvas**
   - Create Canvas: "UI Canvas"
   - Add Canvas Scaler component
   - Set UI Scale Mode: "Scale With Screen Size"

4. **Create Main Element Selection Panel**
   - Create Panel: "MainElementSelectionPanel"
   - Add InputField for player name
   - Add 5 Buttons for elements (Metal, Wood, Water, Fire, Earth)
   - Add Text for element descriptions
   - Attach `MainElementSelectionPanel.cs` script

5. **Create Game HUD**
   - Create Panel: "GameHUD"
   - Add Text elements for stats display
   - Add Buttons for movement (Up, Down, Left, Right)
   - Add Buttons for training (Train Body/Mind for each element)
   - Attach `GameHUD.cs` script

6. **Create Map Container**
   - Create empty GameObject: "MapContainer"
   - This will hold the map tiles

7. **Create Tile Prefab**
   - Create Sprite: "TileSprite"
   - Create GameObject with SpriteRenderer
   - Add Collider2D for click detection
   - Save as Prefab: "Tile.prefab"

### 4. Configure GameManager References

In the GameManager inspector:
- Set MainElementSelectionPanel reference
- Set GameHUD reference
- Set StaminaText, MetalText, etc. references
- Set Move buttons references
- Set MapContainer reference
- Set TilePrefab reference

### 5. Network Configuration

Update `NetworkManager.cs` to use your server URL:
```csharp
private const string ServerUrl = "ws://your-server-url:5000/ws";
```

### 6. Build Settings

1. Go to File → Build Settings
2. Add MainGame scene to build
3. Select target platform (Windows, Mac, Linux, etc.)
4. Configure build settings
5. Build and run

## Testing

1. Start the server: `./dev.sh server`
2. Run the Unity client
3. Enter player name and select main element
4. Test movement, harvesting, and combat

## Troubleshooting

### Connection Issues
- Check server is running on correct port
- Verify firewall settings
- Check WebSocket URL in NetworkManager

### UI Issues
- Ensure all UI references are set in GameManager
- Check Canvas hierarchy
- Verify RectTransform settings

### Build Issues
- Make sure all required packages are installed
- Check .NET compatibility settings
- Verify script compilation errors

## Next Steps

1. Add visual effects for abilities
2. Implement sound effects
3. Add particle systems for combat
4. Create more detailed UI
5. Add player avatar customization
6. Implement chat system