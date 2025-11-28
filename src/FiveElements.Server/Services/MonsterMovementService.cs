using FiveElements.Server.Services;
using System.Threading;

namespace FiveElements.Server.Services
{
    public class MonsterMovementService : BackgroundService
    {
        private readonly IGameWorldService _gameWorldService;
        private readonly ILogger<MonsterMovementService> _logger;

        public MonsterMovementService(IGameWorldService gameWorldService, ILogger<MonsterMovementService> logger)
        {
            _gameWorldService = gameWorldService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _gameWorldService.UpdateMonsters();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating monsters");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Update every 10 seconds
            }
        }
    }
}