using System;
using System.Collections.Generic;
using FiveElements.Shared.Models;

namespace FiveElements.Shared.Services
{
    public interface IGameLogicService
    {
        TrainingResult TrainBody(PlayerStats player, ElementType targetElement);
        TrainingResult TrainMind(PlayerStats player, ElementType targetElement);
        bool CanPlayerMove(PlayerStats player);
        bool MovePlayer(PlayerStats player, int directionX, int directionY);
        HarvestResult HarvestMineral(PlayerStats player, MapObject mineral);
        AttackResult AttackMonster(PlayerStats player, Monster monster, ElementType attackElement);
        MapObject GenerateMapObject(Position position);
        Monster EvolveMonster(Monster monster, ElementType absorbedElement);
    }

    public class GameLogicService : IGameLogicService
    {
        private readonly Random _random = new Random();

        public TrainingResult TrainBody(PlayerStats player, ElementType targetElement)
        {
            var isMainElement = player.Elements.MainElement == targetElement;
            var criticalChance = isMainElement ? 0.3 : 0.05; // 30% for main element, 5% for others
            var isCritical = _random.NextDouble() < criticalChance;
            
            var baseIncrease = 10;
            var increase = isCritical ? baseIncrease * 2 : baseIncrease;
            
            var currentMax = player.Elements.GetMax(targetElement);
            var newMax = currentMax + increase;
            
            player.Elements.SetMax(targetElement, newMax);
            
            return new TrainingResult
            {
                TargetElement = targetElement,
                IsCriticalHit = isCritical,
                Amount = increase,
                Result = isCritical 
                    ? $"暴击！{targetElement}上限增加了{increase}点！" 
                    : $"{targetElement}上限增加了{increase}点"
            };
        }

        public TrainingResult TrainMind(PlayerStats player, ElementType targetElement)
        {
            var isMainElement = player.Elements.MainElement == targetElement;
            var criticalChance = isMainElement ? 0.3 : 0.05;
            var isCritical = _random.NextDouble() < criticalChance;
            
            var baseIncrease = 5;
            var increase = isCritical ? baseIncrease * 2 : baseIncrease;
            
            player.Elements.AddValue(targetElement, increase);
            
            return new TrainingResult
            {
                TargetElement = targetElement,
                IsCriticalHit = isCritical,
                Amount = increase,
                Result = isCritical 
                    ? $"暴击！{targetElement}值增加了{increase}点！" 
                    : $"{targetElement}值增加了{increase}点"
            };
        }

        public bool CanPlayerMove(PlayerStats player)
        {
            player.RegenerateStamina();
            return player.CanConsumeStamina(1); // Moving costs 1 stamina
        }

        public bool MovePlayer(PlayerStats player, int directionX, int directionY)
        {
            if (!CanPlayerMove(player)) return false;
            
            if (player.ConsumeStamina(1))
            {
                player.Position.X += directionX;
                player.Position.Y += directionY;
                return true;
            }
            
            return false;
        }

        public HarvestResult HarvestMineral(PlayerStats player, MapObject mineral)
        {
            if (mineral.Type != MapObjectType.Mineral || !mineral.CanBeHarvested())
            {
                return new HarvestResult
                {
                    Success = false,
                    Message = "无法采集此矿物"
                };
            }

            if (!player.CanConsumeStamina(2)) // Harvesting costs 2 stamina
            {
                return new HarvestResult
                {
                    Success = false,
                    Message = "体力不足"
                };
            }

            player.ConsumeStamina(2);
            
            var harvestAmount = Math.Min(5, mineral.Value);
            if (mineral.Consume(harvestAmount))
            {
                player.Elements.AddValue(mineral.ElementType!.Value, harvestAmount);
                
                return new HarvestResult
                {
                    Success = true,
                    ElementType = mineral.ElementType,
                    Amount = harvestAmount,
                    Message = $"成功采集了{harvestAmount}点{mineral.ElementType}"
                };
            }

            return new HarvestResult
            {
                Success = false,
                Message = "采集失败"
            };
        }

        public AttackResult AttackMonster(PlayerStats player, Monster monster, ElementType attackElement)
        {
            if (!player.Elements.CanConsume(attackElement, 3))
            {
                return new AttackResult
                {
                    Success = false,
                    Message = $"{attackElement}值不足"
                };
            }

            player.Elements.Consume(attackElement, 3);
            
            // Calculate damage based on element relationships
            var baseDamage = 10;
            var damage = baseDamage;
            
            if (attackElement.Overcomes(monster.MonsterElement))
            {
                damage = (int)(baseDamage * 1.5); // 50% more damage
            }
            else if (monster.MonsterElement.Overcomes(attackElement))
            {
                damage = (int)(baseDamage * 0.5); // 50% less damage
            }
            
            monster.TakeDamage(damage);
            
            return new AttackResult
            {
                Success = true,
                Damage = damage,
                MonsterDefeated = !monster.IsAlive(),
                MonsterElement = monster.MonsterElement,
                ExperienceGained = monster.IsAlive() ? 0 : monster.Level * 10,
                Message = monster.IsAlive() 
                    ? $"对{monster.MonsterElement}怪物造成了{damage}点伤害" 
                    : $"击败了{monster.MonsterElement}怪物，获得{monster.Level * 10}点经验"
            };
        }

        public MapObject GenerateMapObject(Position position)
        {
            var distance = position.DistanceToOrigin();
            var level = Math.Max(1, (int)(distance / 10) + 1); // Higher level further from origin
            
            var rand = _random.NextDouble();
            
            if (rand < 0.6) // 60% chance for mineral
            {
                var element = (ElementType)_random.Next(0, 5);
                var maxValue = 20 + level * 5;
                
                return new MapObject
                {
                    Type = MapObjectType.Mineral,
                    ElementType = element,
                    Level = level,
                    Value = maxValue,
                    MaxValue = maxValue,
                    Position = position
                };
            }
            else // 40% chance for monster
            {
                var element = (ElementType)_random.Next(0, 5);
                var health = 30 + level * 10;
                
                return new Monster
                {
                    MonsterElement = element,
                    ElementType = element,
                    Level = level,
                    Health = health,
                    MaxHealth = health,
                    Position = position
                };
            }
        }

        public Monster EvolveMonster(Monster monster, ElementType absorbedElement)
        {
            if (!monster.CanEvolve()) return monster;
            
            // Evolution logic based on five elements relationships
            ElementType newElement;
            
            if (absorbedElement.Generates(monster.MonsterElement))
            {
                // Absorbed element generates monster's element - evolve to generated element
                newElement = monster.MonsterElement;
            }
            else if (monster.MonsterElement.Generates(absorbedElement))
            {
                // Monster's element generates absorbed element - evolve to absorbed element
                newElement = absorbedElement;
            }
            else if (absorbedElement.Overcomes(monster.MonsterElement))
            {
                // Absorbed element overcomes monster's element - evolve to absorbed element
                newElement = absorbedElement;
            }
            else if (monster.MonsterElement.Overcomes(absorbedElement))
            {
                // Monster's element overcomes absorbed element - stay the same but get stronger
                newElement = monster.MonsterElement;
            }
            else
            {
                // Random evolution
                var elements = new[] { ElementType.Metal, ElementType.Wood, ElementType.Water, ElementType.Fire, ElementType.Earth };
                newElement = elements[_random.Next(elements.Length)];
            }
            
            monster.Evolve(newElement);
            return monster;
        }
    }

    public class TrainingResult
    {
        public ElementType TargetElement { get; set; }
        public bool IsCriticalHit { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } = string.Empty;
    }

    public class HarvestResult
    {
        public bool Success { get; set; }
        public ElementType? ElementType { get; set; }
        public int Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AttackResult
    {
        public bool Success { get; set; }
        public int Damage { get; set; }
        public bool MonsterDefeated { get; set; }
        public ElementType? MonsterElement { get; set; }
        public int ExperienceGained { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}