using GameOfLife.Models;

namespace GameOfLife.Interfaces
{
    public interface IGameRepository
    {
        Task<int> CreateBoardWithCoordinates(List<Coordinate> coordinates);
        Task<List<Coordinate>> GetState(int boardId);
        Task InsertCoordinates(int boardId, List<Coordinate> newCoordinates);
        Task DeleteCoordinate(int coordinateId);
        Task DeleteCoordinates(List<Coordinate> coordinates);
    }
}
