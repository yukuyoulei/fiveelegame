using Microsoft.AspNetCore.Mvc;
using FiveElements.Server.Services;
using FiveElements.Shared.Models;

namespace FiveElements.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IConnectionManager _connectionManager;

        public GameController(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        [HttpGet("players")]
        public IActionResult GetOnlinePlayers()
        {
            var players = _connectionManager.GetConnectedPlayers();
            return Ok(players);
        }

        [HttpGet("world/stats")]
        public IActionResult GetWorldStats()
        {
            var stats = _connectionManager.GetWorldStats();
            return Ok(stats);
        }
    }
}