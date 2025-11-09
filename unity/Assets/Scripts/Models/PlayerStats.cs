using System;

namespace FiveElements.Shared.Models
{
    [System.Serializable]
    public class PlayerStats
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PlayerElementStats Elements { get; set; } = new PlayerElementStats();
        public int Stamina { get; set; } = 100;
        public int MaxStamina { get; set; } = 100;
        public Position Position { get; set; } = new Position(0, 0);
        public DateTime LastStaminaRegen { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        public bool CanConsumeStamina(int amount)
        {
            return Stamina >= amount;
        }

        public bool ConsumeStamina(int amount)
        {
            if (!CanConsumeStamina(amount)) return false;
            
            Stamina -= amount;
            return true;
        }

        public void AddStamina(int amount)
        {
            Stamina = Math.Min(Stamina + amount, MaxStamina);
        }

        public void RegenerateStamina()
        {
            var now = DateTime.UtcNow;
            var timeSinceLastRegen = now - LastStaminaRegen;
            
            // Regenerate 1 stamina every 5 seconds
            var regenAmount = (int)(timeSinceLastRegen.TotalSeconds / 5);
            if (regenAmount > 0)
            {
                AddStamina(regenAmount);
                LastStaminaRegen = now;
            }
        }
    }
}
