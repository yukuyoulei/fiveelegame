using System;
using System.Collections.Generic;

namespace FiveElements.Shared.Models
{
    [System.Serializable]
    public class MapCell
    {
        public Position Position { get; set; } = new Position(0, 0);
        public MapObject Object { get; set; }
        public List<string> PlayerIds { get; set; } = new List<string>();
        public bool IsExplored { get; set; } = false;

        public MapCell(Position position)
        {
            Position = position;
        }

        public bool HasPlayer(string playerId)
        {
            return PlayerIds.Contains(playerId);
        }

        public void AddPlayer(string playerId)
        {
            if (!PlayerIds.Contains(playerId))
            {
                PlayerIds.Add(playerId);
            }
        }

        public void RemovePlayer(string playerId)
        {
            PlayerIds.Remove(playerId);
        }

        public bool IsOccupied()
        {
            return Object != null || PlayerIds.Count > 0;
        }
    }
}
