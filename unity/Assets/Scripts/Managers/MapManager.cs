using UnityEngine;
using System.Collections.Generic;
using FiveElements.Shared;

namespace FiveElements.Unity.Managers
{
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        public GameObject TilePrefab;
        public Transform MapContainer;
        public float TileSize = 1f;
        
        [Header("Tile Colors")]
        public Color EmptyColor = Color.gray;
        public Color MineralMetalColor = Color.yellow;
        public Color MineralWoodColor = Color.green;
        public Color MineralWaterColor = Color.blue;
        public Color MineralFireColor = Color.red;
        public Color MineralEarthColor = new Color(0.5f, 0.3f, 0.1f);
        public Color MonsterColor = Color.magenta;
        public Color PlayerColor = Color.white;

        private Dictionary<Position, GameObject> _tiles = new Dictionary<Position, GameObject>();

        public void UpdateMap(MapView mapView)
        {
            ClearMap();
            
            var centerPosition = mapView.CenterPosition;
            
            // Create 3x3 grid around center position
            for (int x = centerPosition.X - 1; x <= centerPosition.X + 1; x++)
            {
                for (int y = centerPosition.Y - 1; y <= centerPosition.Y + 1; y++)
                {
                    var position = new Position(x, y);
                    var cell = mapView.GetCell(position);
                    
                    if (cell != null)
                    {
                        CreateTile(cell, centerPosition);
                    }
                }
            }
            
            // Draw nearby players
            foreach (var player in mapView.NearbyPlayers)
            {
                DrawPlayer(player, centerPosition);
            }
        }

        private void CreateTile(MapCell cell, Position centerPosition)
        {
            var worldX = (cell.Position.X - centerPosition.X) * TileSize;
            var worldY = (cell.Position.Y - centerPosition.Y) * TileSize;
            
            var tile = Instantiate(TilePrefab, MapContainer);
            tile.transform.position = new Vector3(worldX, worldY, 0);
            
            var tileRenderer = tile.GetComponent<SpriteRenderer>();
            if (tileRenderer != null)
            {
                tileRenderer.color = GetTileColor(cell);
            }
            
            // Add tile script for interaction
            var tileScript = tile.AddComponent<MapTile>();
            tileScript.Initialize(cell.Position, cell);
            
            _tiles[cell.Position] = tile;
        }

        private Color GetTileColor(MapCell cell)
        {
            if (cell.Object != null)
            {
                if (cell.Object.Type == MapObjectType.Mineral)
                {
                    return cell.Object.ElementType switch
                    {
                        ElementType.Metal => MineralMetalColor,
                        ElementType.Wood => MineralWoodColor,
                        ElementType.Water => MineralWaterColor,
                        ElementType.Fire => MineralFireColor,
                        ElementType.Earth => MineralEarthColor,
                        _ => EmptyColor
                    };
                }
                else if (cell.Object.Type == MapObjectType.Monster)
                {
                    return MonsterColor;
                }
            }
            
            return EmptyColor;
        }

        private void DrawPlayer(PlayerInfo player, Position centerPosition)
        {
            var worldX = (player.Position.X - centerPosition.X) * TileSize;
            var worldY = (player.Position.Y - centerPosition.Y) * TileSize;
            
            var playerObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            playerObj.transform.position = new Vector3(worldX, worldY, -0.5f);
            playerObj.transform.localScale = Vector3.one * 0.3f;
            playerObj.transform.SetParent(MapContainer);
            
            var renderer = playerObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = PlayerColor;
            }
            
            // Add player info
            var textObj = new GameObject("PlayerName");
            textObj.transform.position = new Vector3(worldX, worldY + 0.3f, -0.6f);
            textObj.transform.SetParent(MapContainer);
            
            var textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = player.Name;
            textMesh.fontSize = 20;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            
            _tiles[player.Position] = playerObj;
        }

        private void ClearMap()
        {
            foreach (var tile in _tiles.Values)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
            
            _tiles.Clear();
        }
    }

    public class MapTile : MonoBehaviour
    {
        public Position Position { get; private set; }
        public MapCell Cell { get; private set; }

        public void Initialize(Position position, MapCell cell)
        {
            Position = position;
            Cell = cell;
        }

        private void OnMouseDown()
        {
            // Handle tile click
            if (Cell.Object != null)
            {
                if (Cell.Object.Type == MapObjectType.Mineral)
                {
                    GameManager.Instance.OnHarvestClicked(Position);
                }
                else if (Cell.Object.Type == MapObjectType.Monster)
                {
                    // For now, use player's main element for attack
                    var player = GameManager.Instance.PlayerStats;
                    GameManager.Instance.OnAttackClicked(Position, player.Elements.MainElement);
                }
            }
        }
    }
}