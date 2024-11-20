using GameOfLife.Interfaces;
using GameOfLife.Models;
using GameOfLife.Models.Comparers;
using System.Collections.Concurrent;
using System.Drawing;

namespace GameOfLife.Services
{
    public class GameService: IGameService
    {
        private readonly IGameRepository _gameRepository;
        private List<(int x, int y)> _adjacentOffsets;
        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
            _adjacentOffsets = new List<(int x, int y)>
               {
                   (-1, -1), (-1, 0), (-1, 1),
                   (0, -1), (0, 1), (1, -1),
                   (1, 0), (1, 1)
               };
        }

        public async Task<int> CreateBoardWithCoordinates(List<Coordinate> coordinates)
        {
            return await _gameRepository.CreateBoardWithCoordinates(coordinates);
        }

        //Method that return next state by applying the rules of the game to the current state of the board
        public async Task<List<Coordinate>> GetNextState(int boardId)
        {
            var markedForDeletion = new List<Coordinate>();
            var currentState = await _gameRepository.GetState(boardId);
            var (adjacents, checkedCoordinates) = CountAdjacentCells(currentState);

            foreach (var adjacent in adjacents)
            {
                var coordinate = adjacent.Key;
                var liveNeighbours = adjacent.Value;

                // Rule 1: Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                // Rule 2: Any live cell with two or three live neighbours lives on to the next generation.
                // This is implicit on the way algorithm is written so no need to take and action here. 
                // Rule 3: Any live cell with more than three live neighbours dies, as if by overpopulation.
                if (liveNeighbours < 2 || liveNeighbours > 3)
                {
                    markedForDeletion.Add(coordinate);
                }               
            }

            // In order to get the new cells that will live we need to get the list of cells that were adjacent and remove the ones that are already in the current state
            // Also we need to remove duplicates from the list
            checkedCoordinates.RemoveAll(coord => currentState.Any(c => c.X == coord.X && c.Y == coord.Y));            
            var uniqueList = checkedCoordinates.Distinct(new CoordinateComparer()).ToList();
            
            var revisedCheckedCells = CountAdjacentCellsWithOriginalList(uniqueList, currentState);

            foreach (var checkedCell in revisedCheckedCells)
            {
                var coordinate = checkedCell.Key;
                var liveNeighbours = checkedCell.Value;
                // Rule 4: Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                if (liveNeighbours == 3)
                {
                    currentState.Add(coordinate);
                }
            }

            await _gameRepository.InsertCoordinates(boardId, currentState);
            await _gameRepository.DeleteCoordinates(markedForDeletion);
            return currentState;
        }

        public async Task<List<Coordinate>> GetNextStateWithSteps(int boardId, int steps)
        {
            var returnedCoordinates = new List<Coordinate>();
            for (int i = 0; i < steps-1; i++)
            {
                await GetNextState(boardId);
            }
            return await GetNextState(boardId);
        }

        public async Task<List<Coordinate>> GetFinalState(int boardId, int steps)
        {
            var previousState = await _gameRepository.GetState(boardId);
            var currentState = new List<Coordinate>(previousState);

            for (int i = 0; i < steps; i++)
            {
                currentState = await GetNextState(boardId);

                // Validate if the board in Step X and Step X-1 have the same coordinates
                if (!previousState.SequenceEqual(currentState, new CoordinateComparer()))
                {
                    throw new Exception("The board in Step X and Step X-1 haven't come to conclusion.");
                }

                previousState = new List<Coordinate>(currentState);
            }

            return currentState;
        }

        //Method that counts the number of live neighbours for each cell in the board and returns a dictionary with the count of live neighbours for each cell
        private (Dictionary<Coordinate, int> counter, List<Coordinate> checkedCoordinates) CountAdjacentCells(List<Coordinate> coordinates)
        {

            var coordinateSet = new HashSet<Coordinate>(coordinates);
            var adjacentList = new ConcurrentDictionary<Coordinate, int>();
            var checkedCoordinates = new ConcurrentBag<Coordinate>();

            foreach (var coordinate in coordinates)
            {
                adjacentList[coordinate] = 0;
            }

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(coordinates, parallelOptions, coordinate =>
            {
                foreach (var (dx, dy) in _adjacentOffsets)
                {
                    var adjacentCoordinate = new Coordinate
                    {
                        X = coordinate.X + dx,
                        Y = coordinate.Y + dy
                    };

                    checkedCoordinates.Add(adjacentCoordinate);                    

                    if (coordinateSet.Where(coord => coord.X == adjacentCoordinate.X && coord.Y == adjacentCoordinate.Y).Count() == 1)
                    {
                        adjacentList.AddOrUpdate(coordinate, 1, (key, oldValue) => oldValue + 1);
                    }
                }
            });

            return (adjacentList.ToDictionary(adjacent => adjacent.Key, adjacent => adjacent.Value), checkedCoordinates.ToList());
        }

        //Method that counts the number of live neighbours for each cell in the board and compares it with the original list of coordinates
        private Dictionary<Coordinate, int> CountAdjacentCellsWithOriginalList(List<Coordinate> coordinates, List<Coordinate> currenState)
        {            
            var adjacentList = new ConcurrentDictionary<Coordinate, int>();

            foreach (var coordinate in coordinates)
            {
                adjacentList[coordinate] = 0;
            }

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(coordinates, parallelOptions, coordinate =>
            {
                foreach (var (dx, dy) in _adjacentOffsets)
                {
                    var adjacentCoordinate = new Coordinate
                    {
                        X = coordinate.X + dx,
                        Y = coordinate.Y + dy
                    };                    

                    if (currenState.Where(coord => coord.X == adjacentCoordinate.X && coord.Y == adjacentCoordinate.Y).Count() == 1)
                    {
                        adjacentList.AddOrUpdate(coordinate, 1, (key, oldValue) => oldValue + 1);
                    }
                }
            });

            return adjacentList.ToDictionary(adjacent => adjacent.Key, adjacent => adjacent.Value);
        }
    }
}
