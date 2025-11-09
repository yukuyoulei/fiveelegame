using System;
using System.Collections.Generic;
using FiveElements.Shared;

namespace FiveElements.Shared.Models
{
    [System.Serializable]
    public class MapView
    {
        public Position CenterPosition { get; set; } = new Position(0, 0);
        public List<MapCell> Cells { get; set; } = new List<MapCell>();
        public List<PlayerInfo> NearbyPlayers { get; set; } = new List<PlayerInfo>();

        private Dictionary<Position, MapCell> _cellDictionary = new Dictionary<Position, MapCell>();

        public MapView(Position center)
        {
            CenterPosition = center;
        }

        public void AddCell(MapCell cell)
        {
            if (!_cellDictionary.ContainsKey(cell.Position))
            {
                _cellDictionary[cell.Position] = cell;
                Cells.Add(cell);
            }
            else
            {
                _cellDictionary[cell.Position] = cell;
                // Update existing cell in list
                var existingCell = Cells.Find(c => c.Position.Equals(cell.Position));
                if (existingCell != null)
                {
                    var index = Cells.IndexOf(existingCell);
                    Cells[index] = cell;
                }
            }
        }

        public MapCell? GetCell(Position position)
        {
            return _cellDictionary.TryGetValue(position, out var cell) ? cell : null;
        }

        public bool HasCell(Position position)
        {
            return _cellDictionary.ContainsKey(position);
        }
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Position Position { get; set; } = new Position(0, 0);
        public ElementType MainElement { get; set; } = ElementType.None;
    }
}
