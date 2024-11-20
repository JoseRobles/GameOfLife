using GameOfLife.Interfaces;
using GameOfLife.Models;
using GameOfLife.Models.Configurations;
using GameOfLife.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameOfLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {

        private readonly ILogger<ProcessController> _logger;
        private readonly IGameService _gameService;
        private readonly DefaultStepsConfig _defaultStepsConfig;

        public ProcessController(ILogger<ProcessController> logger, IGameService gameService,
            IOptions<DefaultStepsConfig> defaultStepsConfig)
        {
            _logger = logger;
            _gameService = gameService;
            _defaultStepsConfig = defaultStepsConfig.Value;
        }

        [HttpPost("Board")]
        public async Task<int> Post(CreateBoardRequest board)
        {
            return await _gameService.CreateBoardWithCoordinates(board.Coordinates);
        }

        [HttpGet("State/Next/{boardId}")]
        public async Task<List<Coordinate>> Get(int boardId)
        {
            return await _gameService.GetNextState(boardId);
        }

        [HttpGet("State/{boardId}/Steps/{steps}")]
        public async Task<List<Coordinate>> Get(int boardId, int steps)
        {
            return await _gameService.GetNextStateWithSteps(boardId, steps);
        }

        [HttpGet("State/{boardId}/Final")]
        public async Task<List<Coordinate>> GetFinalState(int boardId)
        {
            int steps = _defaultStepsConfig.DefaultSteps;
            return await _gameService.GetFinalState(boardId, steps);
        }
    }
}
