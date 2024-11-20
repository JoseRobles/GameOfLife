using GameOfLife.Models;

namespace GameOfLife.Interfaces
{
    public interface IGameService
    {
        Task<int> CreateBoardWithCoordinates(List<Coordinate> coordinates);
        Task<List<Coordinate>> GetNextState(int boardId);
        Task<List<Coordinate>> GetNextStageWithSteps(int boardId, int steps);
    }
}
