using System;
using System.Collections.Generic;
using FiveElements.Shared.Models;

namespace FiveElements.Shared.Messages
{
    // Base message class
    public abstract class MessageBase
    {
        public string Type { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Client to Server Messages
    public class PlayerMoveMessage : MessageBase
    {
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
    }

    public class PlayerHarvestMessage : MessageBase
    {
        public Position TargetPosition { get; set; } = new Position(0, 0);
    }

    public class PlayerAttackMessage : MessageBase
    {
        public Position TargetPosition { get; set; } = new Position(0, 0);
        public ElementType AttackElement { get; set; }
    }

    public class PlayerTrainBodyMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
    }

    public class PlayerTrainMindMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
    }

    public class PlayerSelectMainElementMessage : MessageBase
    {
        public ElementType MainElement { get; set; }
        public string PlayerName { get; set; } = string.Empty;
    }

    // Server to Client Messages
    public class PlayerStatsUpdateMessage : MessageBase
    {
        public PlayerStats PlayerStats { get; set; } = new PlayerStats();
    }

    public class MapUpdateMessage : MessageBase
    {
        public MapView MapView { get; set; } = new MapView(new Position(0, 0));
    }

    public class TrainingResultMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
        public bool IsCriticalHit { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } = string.Empty;
    }

    public class HarvestResultMessage : MessageBase
    {
        public bool Success { get; set; }
        public ElementType? ElementType { get; set; }
        public int Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AttackResultMessage : MessageBase
    {
        public bool Success { get; set; }
        public int Damage { get; set; }
        public bool MonsterDefeated { get; set; }
        public ElementType? MonsterElement { get; set; }
        public int ExperienceGained { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorMessage : MessageBase
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class WelcomeMessage : MessageBase
    {
        public PlayerStats PlayerStats { get; set; } = new PlayerStats();
        public bool NeedsMainElementSelection { get; set; }
    }

    public class PlayerJoinedMessage : MessageBase
    {
        public PlayerInfo Player { get; set; } = new PlayerInfo();
    }

    public class PlayerLeftMessage : MessageBase
    {
        public string PlayerId { get; set; } = string.Empty;
    }
}
