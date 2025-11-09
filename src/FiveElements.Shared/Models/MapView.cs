using System;
using System.Collections.Generic;

namespace FiveElements.Shared.Models
{
    public class MapView
    {
        public Position CenterPosition { get; set; } = new Position(0, 0);
        public Dictionary<Position, MapCell> Cells { get; set; } = new Dictionary<Position, MapCell>();
        public List<PlayerInfo> NearbyPlayers { get; set; } = new List<PlayerInfo>();

        public MapView(Position center)
        {
            CenterPosition = center;
        }

        public void AddCell(MapCell cell)
        {
            Cells[cell.Position] = cell;
        }

        public MapCell? GetCell(Position position)
        {
            return Cells.TryGetValue(position, out var cell) ? cell : null;
        }

        public bool HasCell(Position position)
        {
            return Cells.ContainsKey(position);
        }
    }

    public class PlayerInfo
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Position Position { get; set; } = new Position(0, 0);
        public ElementType MainElement { get; set; }
    }
}