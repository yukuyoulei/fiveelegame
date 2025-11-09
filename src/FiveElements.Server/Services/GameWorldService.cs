using System.Collections.Concurrent;
using FiveElements.Shared.Models;
using FiveElements.Shared.Services;
using FiveElements.Shared;

namespace FiveElements.Server.Services
{
    public interface IGameWorldService
    {
        PlayerStats CreatePlayer(string connectionId, string name, ElementType mainElement);
        PlayerStats? GetPlayer(string playerId);
        void RemovePlayer(string playerId);
        bool MovePlayer(string playerId, int directionX, int directionY);
        MapView GetMapView(Position centerPosition);
        HarvestResult HarvestMineral(string playerId, Position targetPosition);
        AttackResult AttackMonster(string playerId, Position targetPosition, ElementType attackElement);
        TrainingResult TrainBody(string playerId, ElementType targetElement);
        TrainingResult TrainMind(string playerId, ElementType targetElement);
        int GetTotalPlayerCount();
        void UpdateMonsters();
    }

    public class GameWorldService : IGameWorldService
    {
        private readonly ConcurrentDictionary<string, PlayerStats> _players = new();
        private readonly ConcurrentDictionary<Position, MapCell> _mapCells = new();
        private readonly IGameLogicService _gameLogicService;
        private readonly ILogger<GameWorldService> _logger;

        public GameWorldService(IGameLogicService gameLogicService, ILogger<GameWorldService> logger)
        {
            _gameLogicService = gameLogicService;
            _logger = logger;
            
            // Initialize spawn area
            InitializeSpawnArea();
        }

        private void InitializeSpawnArea()
        {
            var random = new Random();
            
            // Generate objects in 10 cells around spawn point (0,0)
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    if (x == 0 && y == 0) continue; // Skip spawn point
                    
                    var position = new Position(x, y);
                    var cell = new MapCell(position);
                    
                    // 60% chance to generate something in each cell
                    if (random.NextDouble() < 0.6)
                    {
                        cell.Object = _gameLogicService.GenerateMapObject(position);
                    }
                    
                    _mapCells[position] = cell;
                }
            }
        }

        public PlayerStats CreatePlayer(string connectionId, string name, ElementType mainElement)
        {
            var player = new PlayerStats
            {
                PlayerId = connectionId,
                Name = name,
                Elements = new PlayerElementStats
                {
                    MainElement = mainElement,
                    // Give some initial element values
                    MetalValue = 10,
                    WoodValue = 10,
                    WaterValue = 10,
                    FireValue = 10,
                    EarthValue = 10
                },
                Position = new Position(0, 0), // Spawn at origin
                LastLogin = DateTime.UtcNow,
                LastStaminaRegen = DateTime.UtcNow
            };

            _players[connectionId] = player;
            
            // Add player to spawn cell
            var spawnCell = GetOrCreateCell(new Position(0, 0));
            spawnCell.AddPlayer(connectionId);

            _logger.LogInformation($"Player {name} ({connectionId}) joined with main element {mainElement}");
            
            return player;
        }

        public PlayerStats? GetPlayer(string playerId)
        {
            _players.TryGetValue(playerId, out var player);
            return player;
        }

        public void RemovePlayer(string playerId)
        {
            if (_players.TryRemove(playerId, out var player))
            {
                // Remove player from map cell
                var cell = GetCell(player.Position);
                if (cell != null)
                {
                    cell.RemovePlayer(playerId);
                }

                _logger.LogInformation($"Player {player.Name} ({playerId}) left the game");
            }
        }

        public bool MovePlayer(string playerId, int directionX, int directionY)
        {
            var player = GetPlayer(playerId);
            if (player == null) return false;

            if (!_gameLogicService.CanPlayerMove(player)) return false;

            var oldPosition = new Position(player.Position.X, player.Position.Y);
            var newPosition = new Position(player.Position.X + directionX, player.Position.Y + directionY);

            if (_gameLogicService.MovePlayer(player, directionX, directionY))
            {
                // Update map cells
                var oldCell = GetCell(oldPosition);
                if (oldCell != null)
                {
                    oldCell.RemovePlayer(playerId);
                }

                var newCell = GetOrCreateCell(newPosition);
                newCell.AddPlayer(playerId);
                newCell.IsExplored = true;

                // Generate content for new cell if not already generated
                if (newCell.Object == null && !newCell.IsExplored)
                {
                    newCell.Object = _gameLogicService.GenerateMapObject(newPosition);
                    newCell.IsExplored = true;
                }

                // Ensure nearby cells are generated
                EnsureNearbyCellsGenerated(newPosition);

                return true;
            }

            return false;
        }

        public MapView GetMapView(Position centerPosition)
        {
            var mapView = new MapView(centerPosition);

            // Get 3x3 area around center position
            for (int x = centerPosition.X - 1; x <= centerPosition.X + 1; x++)
            {
                for (int y = centerPosition.Y - 1; y <= centerPosition.Y + 1; y++)
                {
                    var position = new Position(x, y);
                    var cell = GetOrCreateCell(position);
                    
                    // Mark as explored and generate content if needed
                    if (!cell.IsExplored)
                    {
                        cell.Object = _gameLogicService.GenerateMapObject(position);
                        cell.IsExplored = true;
                    }

                    mapView.AddCell(cell);
                }
            }

            // Add nearby players
            foreach (var player in _players.Values)
            {
                var distance = Math.Abs(player.Position.X - centerPosition.X) + 
                              Math.Abs(player.Position.Y - centerPosition.Y);
                
                if (distance <= 1)
                {
                    mapView.NearbyPlayers.Add(new PlayerInfo
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Position = player.Position,
                        MainElement = player.Elements.MainElement
                    });
                }
            }

            return mapView;
        }

        public HarvestResult HarvestMineral(string playerId, Position targetPosition)
        {
            var player = GetPlayer(playerId);
            if (player == null) return new HarvestResult { Success = false, Message = "玩家不存在" };

            var cell = GetCell(targetPosition);
            if (cell == null || cell.Object?.Type != MapObjectType.Mineral)
            {
                return new HarvestResult { Success = false, Message = "目标位置没有矿物" };
            }

            var result = _gameLogicService.HarvestMineral(player, cell.Object);
            
            // Remove mineral if depleted
            if (cell.Object.Value <= 0)
            {
                cell.Object = null;
            }

            return result;
        }

        public AttackResult AttackMonster(string playerId, Position targetPosition, ElementType attackElement)
        {
            var player = GetPlayer(playerId);
            if (player == null) return new AttackResult { Success = false, Message = "玩家不存在" };

            var cell = GetCell(targetPosition);
            if (cell == null || cell.Object?.Type != MapObjectType.Monster)
            {
                return new AttackResult { Success = false, Message = "目标位置没有怪物" };
            }

            var monster = cell.Object as Monster;
            var result = _gameLogicService.AttackMonster(player, monster!, attackElement);
            
            // Remove monster if defeated
            if (!monster!.IsAlive())
            {
                cell.Object = null;
            }

            return result;
        }

        public TrainingResult TrainBody(string playerId, ElementType targetElement)
        {
            var player = GetPlayer(playerId);
            if (player == null) return new TrainingResult { Result = "玩家不存在" };

            return _gameLogicService.TrainBody(player, targetElement);
        }

        public TrainingResult TrainMind(string playerId, ElementType targetElement)
        {
            var player = GetPlayer(playerId);
            if (player == null) return new TrainingResult { Result = "玩家不存在" };

            return _gameLogicService.TrainMind(player, targetElement);
        }

        public int GetTotalPlayerCount()
        {
            return _players.Count;
        }

        public void UpdateMonsters()
        {
            var random = new Random();
            
            foreach (var kvp in _mapCells)
            {
                var cell = kvp.Value;
                if (cell.Object is Monster monster && monster.IsAlive())
                {
                    // Move monsters
                    if (monster.CanMove())
                    {
                        var directions = new[]
                        {
                            new Position(-1, 0), new Position(1, 0),
                            new Position(0, -1), new Position(0, 1)
                        };
                        
                        var direction = directions[random.Next(directions.Length)];
                        var newPosition = new Position(
                            monster.Position.X + direction.X,
                            monster.Position.Y + direction.Y
                        );

                        // Check if new position is valid and not occupied by another monster
                        var targetCell = GetCell(newPosition);
                        if (targetCell != null && targetCell.Object?.Type != MapObjectType.Monster)
                        {
                            // Check if there's a mineral to absorb
                            if (targetCell.Object?.Type == MapObjectType.Mineral)
                            {
                                var mineralElement = targetCell.Object.ElementType!.Value;
                                monster = _gameLogicService.EvolveMonster(monster, mineralElement);
                                targetCell.Object = null; // Remove absorbed mineral
                            }

                            // Move monster
                            cell.Object = null;
                            monster.Position = newPosition;
                            monster.LastMove = DateTime.UtcNow;
                            targetCell.Object = monster;
                        }
                    }

                    // Random evolution
                    if (monster.CanEvolve() && random.NextDouble() < 0.1) // 10% chance when evolution is available
                    {
                        var elements = new[] { ElementType.Metal, ElementType.Wood, ElementType.Water, ElementType.Fire, ElementType.Earth };
                        var randomElement = elements[random.Next(elements.Length)];
                        _gameLogicService.EvolveMonster(monster, randomElement);
                    }
                }
            }
        }

        private MapCell? GetCell(Position position)
        {
            _mapCells.TryGetValue(position, out var cell);
            return cell;
        }

        private MapCell GetOrCreateCell(Position position)
        {
            return _mapCells.GetOrAdd(position, pos => new MapCell(pos));
        }

        private void EnsureNearbyCellsGenerated(Position center)
        {
            for (int x = center.X - 1; x <= center.X + 1; x++)
            {
                for (int y = center.Y - 1; y <= center.Y + 1; y++)
                {
                    var position = new Position(x, y);
                    GetOrCreateCell(position);
                }
            }
        }
    }
}