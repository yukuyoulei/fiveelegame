using System;
using System.Collections.Generic;
using FiveElements.Shared;

namespace FiveElements.Shared.Messages
{
    // Base message class
    [System.Serializable]
    public abstract class MessageBase
    {
        public string Type { get; set; } = string.Empty;
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("O");
    }

    // Client to Server Messages
    [System.Serializable]
    public class PlayerMoveMessage : MessageBase
    {
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
    }

    [System.Serializable]
    public class PlayerHarvestMessage : MessageBase
    {
        public Position TargetPosition { get; set; } = new Position(0, 0);
    }

    [System.Serializable]
    public class PlayerAttackMessage : MessageBase
    {
        public Position TargetPosition { get; set; } = new Position(0, 0);
        public ElementType AttackElement { get; set; }
    }

    [System.Serializable]
    public class PlayerTrainBodyMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
    }

    [System.Serializable]
    public class PlayerTrainMindMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
    }

    [System.Serializable]
    public class PlayerSelectMainElementMessage : MessageBase
    {
        public ElementType MainElement { get; set; }
        public string PlayerName { get; set; } = string.Empty;
    }

    // Server to Client Messages
    [System.Serializable]
    public class PlayerStatsUpdateMessage : MessageBase
    {
        public PlayerStats PlayerStats { get; set; } = new PlayerStats();
    }

    [System.Serializable]
    public class MapUpdateMessage : MessageBase
    {
        public MapView MapView { get; set; } = new MapView(new Position(0, 0));
    }

    [System.Serializable]
    public class TrainingResultMessage : MessageBase
    {
        public ElementType TargetElement { get; set; }
        public bool IsCriticalHit { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } = string.Empty;
    }

    [System.Serializable]
    public class HarvestResultMessage : MessageBase
    {
        public bool Success { get; set; }
        public ElementType ElementType { get; set; }
        public int Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    [System.Serializable]
    public class AttackResultMessage : MessageBase
    {
        public bool Success { get; set; }
        public int Damage { get; set; }
        public bool MonsterDefeated { get; set; }
        public ElementType MonsterElement { get; set; }
        public int ExperienceGained { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    [System.Serializable]
    public class ErrorMessage : MessageBase
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    [System.Serializable]
    public class WelcomeMessage : MessageBase
    {
        public PlayerStats PlayerStats { get; set; } = new PlayerStats();
        public bool NeedsMainElementSelection { get; set; }
    }

    [System.Serializable]
    public class PlayerJoinedMessage : MessageBase
    {
        public PlayerInfo Player { get; set; } = new PlayerInfo();
    }

    [System.Serializable]
    public class PlayerLeftMessage : MessageBase
    {
        public string PlayerId { get; set; } = string.Empty;
    }
}
