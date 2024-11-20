using GameOfLife.Models;

namespace GameOfLife.Interfaces
{
    public interface IGameService
    {
        Task<int> CreateBoardWithCoordinates(List<Coordinate> coordinates);
        Task<List<Coordinate>> GetNextState(int boardId);
        Task<List<Coordinate>> GetNextStateWithSteps(int boardId, int steps);
        Task<List<Coordinate>> GetFinalState(int boardId, int steps);
    }
}
