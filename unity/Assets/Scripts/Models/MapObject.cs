using System;
using FiveElements.Shared;

namespace FiveElements.Shared.Models
{
    public enum MapObjectType
    {
        Empty,
        Mineral,
        Monster
    }

    [System.Serializable]
    public class MapObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public MapObjectType Type { get; set; } = MapObjectType.Empty;
        public ElementType ElementType { get; set; } = ElementType.None;
        public int Level { get; set; } = 1;
        public int Value { get; set; } = 0;
        public int MaxValue { get; set; } = 100;
        public DateTime LastRegen { get; set; } = DateTime.UtcNow;
        public Position Position { get; set; } = new Position(0, 0);

        public bool CanBeHarvested()
        {
            return Type == MapObjectType.Mineral && Value > 0;
        }

        public bool Consume(int amount)
        {
            if (Value < amount) return false;
            Value -= amount;
            return true;
        }

        public void Regenerate()
        {
            if (Type == MapObjectType.Mineral && Value < MaxValue)
            {
                var now = DateTime.UtcNow;
                var timeSinceLastRegen = now - LastRegen;
                
                // Regenerate 1 value every 30 seconds
                var regenAmount = (int)(timeSinceLastRegen.TotalSeconds / 30);
                if (regenAmount > 0)
                {
                    Value = Math.Min(Value + regenAmount, MaxValue);
                    LastRegen = now;
                }
            }
        }
    }

    [System.Serializable]
    public class Monster : MapObject
    {
        public ElementType MonsterElement { get; set; } = ElementType.None;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public DateTime LastMove { get; set; } = DateTime.UtcNow;
        public DateTime LastEvolution { get; set; } = DateTime.UtcNow;

        public Monster()
        {
            Type = MapObjectType.Monster;
        }

        public bool CanMove()
        {
            var now = DateTime.UtcNow;
            return (now - LastMove).TotalSeconds >= 10; // Move every 10 seconds
        }

        public bool CanEvolve()
        {
            var now = DateTime.UtcNow;
            return (now - LastEvolution).TotalMinutes >= 5; // Can evolve every 5 minutes
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public void Evolve(ElementType newElement)
        {
            MonsterElement = newElement;
            ElementType = newElement;
            Level++;
            MaxHealth = (int)(MaxHealth * 1.2);
            Health = MaxHealth;
            LastEvolution = DateTime.UtcNow;
        }
    }
}
