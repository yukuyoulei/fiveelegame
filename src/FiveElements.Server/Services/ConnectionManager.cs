using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using FiveElements.Server.Services;
using FiveElements.Shared.Messages;
using FiveElements.Shared.Models;

namespace FiveElements.Server.Services
{
    public interface IConnectionManager
    {
        Task HandleConnectionAsync(WebSocket webSocket, HttpContext context);
        Task BroadcastToAllAsync(MessageBase message);
        Task SendToPlayerAsync(string playerId, MessageBase message);
        Task SendToNearbyPlayersAsync(Position position, MessageBase message, string? excludePlayerId = null);
        List<PlayerInfo> GetConnectedPlayers();
        object GetWorldStats();
        void RemoveConnection(string playerId);
    }

    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocketConnection> _connections = new();
        private readonly IGameWorldService _gameWorldService;
        private readonly ILogger<ConnectionManager> _logger;

        public ConnectionManager(IGameWorldService gameWorldService, ILogger<ConnectionManager> logger)
        {
            _gameWorldService = gameWorldService;
            _logger = logger;
        }

        public async Task HandleConnectionAsync(WebSocket webSocket, HttpContext context)
        {
            var connectionId = Guid.NewGuid().ToString();
            var connection = new WebSocketConnection(webSocket, connectionId);
            
            _connections[connectionId] = connection;

            try
            {
                await connection.StartReceiving(async (message) =>
                {
                    await HandleMessageAsync(connectionId, message);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WebSocket connection");
            }
            finally
            {
                await DisconnectAsync(connectionId);
            }
        }

        private async Task HandleMessageAsync(string connectionId, string message)
        {
            try
            {
                var messageObj = JsonSerializer.Deserialize<MessageBase>(message);
                if (messageObj == null) return;

                switch (messageObj.Type)
                {
                    case nameof(PlayerSelectMainElementMessage):
                        await HandleSelectMainElement(connectionId, JsonSerializer.Deserialize<PlayerSelectMainElementMessage>(message)!);
                        break;
                    case nameof(PlayerMoveMessage):
                        await HandlePlayerMove(connectionId, JsonSerializer.Deserialize<PlayerMoveMessage>(message)!);
                        break;
                    case nameof(PlayerHarvestMessage):
                        await HandlePlayerHarvest(connectionId, JsonSerializer.Deserialize<PlayerHarvestMessage>(message)!);
                        break;
                    case nameof(PlayerAttackMessage):
                        await HandlePlayerAttack(connectionId, JsonSerializer.Deserialize<PlayerAttackMessage>(message)!);
                        break;
                    case nameof(PlayerTrainBodyMessage):
                        await HandlePlayerTrainBody(connectionId, JsonSerializer.Deserialize<PlayerTrainBodyMessage>(message)!);
                        break;
                    case nameof(PlayerTrainMindMessage):
                        await HandlePlayerTrainMind(connectionId, JsonSerializer.Deserialize<PlayerTrainMindMessage>(message)!);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message: {Message}", message);
            }
        }

        private async Task HandleSelectMainElement(string connectionId, PlayerSelectMainElementMessage message)
        {
            var player = _gameWorldService.CreatePlayer(connectionId, message.PlayerName, message.MainElement);
            var connection = _connections[connectionId];
            connection.PlayerId = player.PlayerId;

            var welcomeMessage = new WelcomeMessage
            {
                PlayerStats = player,
                NeedsMainElementSelection = false
            };

            await SendToPlayerAsync(player.PlayerId, welcomeMessage);
            
            // Send initial map view
            await SendMapUpdate(player);
            
            // Notify nearby players
            await NotifyNearbyPlayers(player);
        }

        private async Task HandlePlayerMove(string connectionId, PlayerMoveMessage message)
        {
            var connection = _connections[connectionId];
            if (connection.PlayerId == null) return;

            var player = _gameWorldService.GetPlayer(connection.PlayerId);
            if (player == null) return;

            var oldPosition = new Position(player.Position.X, player.Position.Y);
            
            if (_gameWorldService.MovePlayer(connection.PlayerId, message.DirectionX, message.DirectionY))
            {
                await SendMapUpdate(player);
                await NotifyNearbyPlayers(player);
                
                // Notify players who can no longer see this player
                await NotifyOutOfRangePlayers(oldPosition, player.Position, player.PlayerId);
            }
            else
            {
                var errorMessage = new ErrorMessage
                {
                    ErrorCode = "MOVE_FAILED",
                    Message = "无法移动，体力不足"
                };
                await SendToPlayerAsync(player.PlayerId, errorMessage);
            }
        }

        private async Task HandlePlayerHarvest(string connectionId, PlayerHarvestMessage message)
        {
            var connection = _connections[connectionId];
            if (connection.PlayerId == null) return;

            var player = _gameWorldService.GetPlayer(connection.PlayerId);
            if (player == null) return;

            var result = _gameWorldService.HarvestMineral(connection.PlayerId, message.TargetPosition);
            
            var harvestMessage = new HarvestResultMessage
            {
                Success = result.Success,
                ElementType = result.ElementType,
                Amount = result.Amount,
                Message = result.Message
            };

            await SendToPlayerAsync(player.PlayerId, harvestMessage);
            
            if (result.Success)
            {
                await SendPlayerStats(player.PlayerId);
                await SendMapUpdate(player);
            }
        }

        private async Task HandlePlayerAttack(string connectionId, PlayerAttackMessage message)
        {
            var connection = _connections[connectionId];
            if (connection.PlayerId == null) return;

            var player = _gameWorldService.GetPlayer(connection.PlayerId);
            if (player == null) return;

            var result = _gameWorldService.AttackMonster(connection.PlayerId, message.TargetPosition, message.AttackElement);
            
            var attackMessage = new AttackResultMessage
            {
                Success = result.Success,
                Damage = result.Damage,
                MonsterDefeated = result.MonsterDefeated,
                MonsterElement = result.MonsterElement,
                ExperienceGained = result.ExperienceGained,
                Message = result.Message
            };

            await SendToPlayerAsync(player.PlayerId, attackMessage);
            
            if (result.Success)
            {
                await SendPlayerStats(player.PlayerId);
                await SendMapUpdate(player);
            }
        }

        private async Task HandlePlayerTrainBody(string connectionId, PlayerTrainBodyMessage message)
        {
            var connection = _connections[connectionId];
            if (connection.PlayerId == null) return;

            var player = _gameWorldService.GetPlayer(connection.PlayerId);
            if (player == null) return;

            var result = _gameWorldService.TrainBody(connection.PlayerId, message.TargetElement);
            
            var trainingMessage = new TrainingResultMessage
            {
                TargetElement = result.TargetElement,
                IsCriticalHit = result.IsCriticalHit,
                Amount = result.Amount,
                Result = result.Result
            };

            await SendToPlayerAsync(player.PlayerId, trainingMessage);
            await SendPlayerStats(player.PlayerId);
        }

        private async Task HandlePlayerTrainMind(string connectionId, PlayerTrainMindMessage message)
        {
            var connection = _connections[connectionId];
            if (connection.PlayerId == null) return;

            var player = _gameWorldService.GetPlayer(connection.PlayerId);
            if (player == null) return;

            var result = _gameWorldService.TrainMind(connection.PlayerId, message.TargetElement);
            
            var trainingMessage = new TrainingResultMessage
            {
                TargetElement = result.TargetElement,
                IsCriticalHit = result.IsCriticalHit,
                Amount = result.Amount,
                Result = result.Result
            };

            await SendToPlayerAsync(player.PlayerId, trainingMessage);
            await SendPlayerStats(player.PlayerId);
        }

        private async Task SendMapUpdate(PlayerStats player)
        {
            var mapView = _gameWorldService.GetMapView(player.Position);
            var mapMessage = new MapUpdateMessage
            {
                MapView = mapView
            };

            await SendToPlayerAsync(player.PlayerId, mapMessage);
        }

        private async Task SendPlayerStats(string playerId)
        {
            var player = _gameWorldService.GetPlayer(playerId);
            if (player == null) return;

            var statsMessage = new PlayerStatsUpdateMessage
            {
                PlayerStats = player
            };

            await SendToPlayerAsync(playerId, statsMessage);
        }

        private async Task NotifyNearbyPlayers(PlayerStats player)
        {
            var playerInfo = new PlayerInfo
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Position = player.Position,
                MainElement = player.Elements.MainElement
            };

            var joinMessage = new PlayerJoinedMessage
            {
                Player = playerInfo
            };

            await SendToNearbyPlayersAsync(player.Position, joinMessage, player.PlayerId);
        }

        private async Task NotifyOutOfRangePlayers(Position oldPosition, Position newPosition, string playerId)
        {
            // Get players who could see the old position but can't see the new position
            var oldRange = GetPositionRange(oldPosition);
            var newRange = GetPositionRange(newPosition);

            foreach (var connection in _connections.Values)
            {
                if (connection.PlayerId == playerId || connection.PlayerId == null) continue;

                var player = _gameWorldService.GetPlayer(connection.PlayerId);
                if (player == null) continue;

                var couldSeeOld = IsInRange(player.Position, oldRange);
                var canSeeNew = IsInRange(player.Position, newRange);

                if (couldSeeOld && !canSeeNew)
                {
                    var leaveMessage = new PlayerLeftMessage
                    {
                        PlayerId = playerId
                    };

                    await SendToPlayerAsync(connection.PlayerId, leaveMessage);
                }
            }
        }

        private (Position min, Position max) GetPositionRange(Position center)
        {
            return (new Position(center.X - 1, center.Y - 1), new Position(center.X + 1, center.Y + 1));
        }

        private bool IsInRange(Position position, (Position min, Position max) range)
        {
            return position.X >= range.min.X && position.X <= range.max.X &&
                   position.Y >= range.min.Y && position.Y <= range.max.Y;
        }

        public async Task BroadcastToAllAsync(MessageBase message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(messageJson);

            var tasks = _connections.Values.Select(async connection =>
            {
                if (connection.WebSocket.State == WebSocketState.Open)
                {
                    await connection.WebSocket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            });

            await Task.WhenAll(tasks);
        }

        public async Task SendToPlayerAsync(string playerId, MessageBase message)
        {
            var connection = _connections.Values.FirstOrDefault(c => c.PlayerId == playerId);
            if (connection?.WebSocket.State != WebSocketState.Open) return;

            var messageJson = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(messageJson);

            await connection.WebSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        public async Task SendToNearbyPlayersAsync(Position position, MessageBase message, string? excludePlayerId = null)
        {
            var nearbyConnections = _connections.Values.Where(c => 
                c.PlayerId != null && 
                c.PlayerId != excludePlayerId &&
                IsPlayerNearby(c.PlayerId, position));

            var messageJson = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(messageJson);

            var tasks = nearbyConnections.Select(async connection =>
            {
                if (connection.WebSocket.State == WebSocketState.Open)
                {
                    await connection.WebSocket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            });

            await Task.WhenAll(tasks);
        }

        private bool IsPlayerNearby(string playerId, Position targetPosition)
        {
            var player = _gameWorldService.GetPlayer(playerId);
            if (player == null) return false;

            var distance = Math.Abs(player.Position.X - targetPosition.X) + 
                          Math.Abs(player.Position.Y - targetPosition.Y);
            return distance <= 1; // Within 1 tile (3x3 area)
        }

        public List<PlayerInfo> GetConnectedPlayers()
        {
            return _connections.Values
                .Where(c => c.PlayerId != null)
                .Select(c => _gameWorldService.GetPlayer(c.PlayerId!))
                .Where(p => p != null)
                .Select(p => new PlayerInfo
                {
                    PlayerId = p!.PlayerId,
                    Name = p.Name,
                    Position = p.Position,
                    MainElement = p.Elements.MainElement
                })
                .ToList();
        }

        public object GetWorldStats()
        {
            return new
            {
                OnlinePlayers = GetConnectedPlayers().Count,
                TotalPlayers = _gameWorldService.GetTotalPlayerCount(),
                WorldSize = "Infinite"
            };
        }

        private async Task DisconnectAsync(string connectionId)
        {
            if (_connections.TryRemove(connectionId, out var connection))
            {
                if (connection.PlayerId != null)
                {
                    var player = _gameWorldService.GetPlayer(connection.PlayerId);
                    if (player != null)
                    {
                        var leaveMessage = new PlayerLeftMessage
                        {
                            PlayerId = connection.PlayerId
                        };

                        await SendToNearbyPlayersAsync(player.Position, leaveMessage, connection.PlayerId);
                    }

                    _gameWorldService.RemovePlayer(connection.PlayerId);
                }

                if (connection.WebSocket.State == WebSocketState.Open)
                {
                    await connection.WebSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Connection closed",
                        CancellationToken.None);
                }
            }
        }

        public void RemoveConnection(string playerId)
        {
            var connection = _connections.Values.FirstOrDefault(c => c.PlayerId == playerId);
            if (connection != null)
            {
                _connections.TryRemove(connection.ConnectionId, out _);
            }
        }
    }

    public class WebSocketConnection
    {
        public WebSocket WebSocket { get; }
        public string ConnectionId { get; }
        public string? PlayerId { get; set; }

        public WebSocketConnection(WebSocket webSocket, string connectionId)
        {
            WebSocket = webSocket;
            ConnectionId = connectionId;
        }

        public async Task StartReceiving(Func<string, Task> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (WebSocket.State == WebSocketState.Open)
            {
                var result = await WebSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await handleMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await WebSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        CancellationToken.None);
                    break;
                }
            }
        }
    }
}