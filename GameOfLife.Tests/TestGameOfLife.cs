using GameOfLife.Interfaces;
using GameOfLife.Models;
using GameOfLife.Models.Comparers;
using GameOfLife.Services;
using Moq;
namespace GameOfLife.Tests
{
    [TestClass]
    public sealed class TestGameOfLife
    {
        private Mock<IGameRepository>? _gameRepositoryMock;
        private GameService? _gameService;

        [TestInitialize]
        public void Setup()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _gameService = new GameService(_gameRepositoryMock.Object);
        }

        [TestMethod]
        public async Task TestCreateBoardWithCoordinates()
        {
            // Arrange
            var coordinates = new List<Coordinate>
            {
                new Coordinate { X = 0, Y = 0 },
                new Coordinate { X = 1, Y = 1 }
            };

            _gameRepositoryMock.Setup(repo => repo.CreateBoardWithCoordinates(coordinates)).ReturnsAsync(1);

            // Act
            var result = await _gameService.CreateBoardWithCoordinates(coordinates);

            // Assert
            Assert.AreEqual(1, result);
            _gameRepositoryMock.Verify(repo => repo.CreateBoardWithCoordinates(coordinates), Times.Once);
        }

        [TestMethod]
        public async Task TestGetNextState()
        {
            // Arrange
            var boardId = 1;
            var coordinates = new List<Coordinate>
            {
                new Coordinate { X = 0, Y = 0 },
                new Coordinate { X = 1, Y = 1 }
            };

            _gameRepositoryMock.Setup(repo => repo.GetState(boardId)).ReturnsAsync(coordinates);
            _gameRepositoryMock.Setup(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);
            _gameRepositoryMock.Setup(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _gameService.GetNextState(boardId);

            // Assert
            Assert.IsNotNull(result);
            _gameRepositoryMock.Verify(repo => repo.GetState(boardId), Times.Once);
            _gameRepositoryMock.Verify(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>()), Times.Once);
            _gameRepositoryMock.Verify(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>()), Times.Once);
        }

        [TestMethod]
        public async Task TestGetNextStageWithSteps()
        {
            // Arrange
            var boardId = 1;
            var steps = 5;
            var coordinates = new List<Coordinate>
            {
                new Coordinate { X = 0, Y = 0 },
                new Coordinate { X = 1, Y = 1 }
            };
            _gameRepositoryMock.Setup(repo => repo.GetState(boardId)).ReturnsAsync(coordinates);
            _gameRepositoryMock.Setup(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);
            _gameRepositoryMock.Setup(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _gameService.GetNextStateWithSteps(boardId, steps);

            // Assert
            Assert.IsNotNull(result);
            _gameRepositoryMock.Verify(repo => repo.GetState(boardId), Times.Exactly(steps));
            _gameRepositoryMock.Verify(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>()), Times.Exactly(steps));
            _gameRepositoryMock.Verify(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>()), Times.Exactly(steps));
        }

        [TestMethod]
        public async Task TestGetNextState_ReturnsSpecificCoordinates_Blinker()
        {
            // Arrange
            var boardId = 1;
            var initialCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 2, Y = 4 }
            };
            var expectedCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 3, Y = 3 },
                new Coordinate { X = 1, Y = 3 }
            };

            _gameRepositoryMock.Setup(repo => repo.GetState(boardId)).ReturnsAsync(initialCoordinates);
            _gameRepositoryMock.Setup(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);
            _gameRepositoryMock.Setup(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>())).Callback<List<Coordinate>>(coords =>
            {
                foreach (var coord in coords)
                {
                    initialCoordinates.RemoveAll(c => c.X == coord.X && c.Y == coord.Y);
                }
            }).Returns(Task.CompletedTask);

            // Act
            var result = await _gameService.GetNextState(boardId);

            // Assert
            Assert.AreEqual(expectedCoordinates.Count, result.Count);
            foreach (var expectedCoordinate in expectedCoordinates)
            {
                Assert.IsTrue(result.Any(c => c.X == expectedCoordinate.X && c.Y == expectedCoordinate.Y));
            }         
        }

        [TestMethod]
        public async Task TestGetNextState_ReturnsSpecificCoordinates_Glider()
        {
            // Arrange
            var boardId = 1;
            var initialCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 1, Y = 3 },
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 3, Y = 2 },
                new Coordinate { X = 3, Y = 3 },
                new Coordinate { X = 3, Y = 4 }
            };
            var expectedCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 4 },
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 3, Y = 2 },
                new Coordinate { X = 3, Y = 3 },
                new Coordinate { X = 4, Y = 3 }
            };

            _gameRepositoryMock.Setup(repo => repo.GetState(boardId)).ReturnsAsync(initialCoordinates);
            _gameRepositoryMock.Setup(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);
            _gameRepositoryMock.Setup(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>())).Callback<List<Coordinate>>(coords =>
            {
                foreach (var coord in coords)
                {
                    initialCoordinates.RemoveAll(c => c.X == coord.X && c.Y == coord.Y);
                }
            }).Returns(Task.CompletedTask);

            // Act
            var result = await _gameService.GetNextState(boardId);

            // Assert
            Assert.AreEqual(expectedCoordinates.Count, result.Count);
            foreach (var expectedCoordinate in expectedCoordinates)
            {
                Assert.IsTrue(result.Any(c => c.X == expectedCoordinate.X && c.Y == expectedCoordinate.Y));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task TestGetFinalState_ThrowsException()
        {
            // Arrange
            var boardId = 1;
            var steps = 5;
            var initialCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 2, Y = 4 }
            };
            var nextCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 1, Y = 3 },
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 3, Y = 3 }
            };

            _gameRepositoryMock.SetupSequence(repo => repo.GetState(boardId))
                .ReturnsAsync(initialCoordinates)
                .ReturnsAsync(nextCoordinates);

            // Act
            await _gameService.GetFinalState(boardId, steps);

            // Assert
            // Expects CoordinateMismatchException to be thrown
        }

        [TestMethod]
        public async Task TestGetFinalState_ReachedConclusion()
        {
            // Arrange
            var boardId = 1;
            var steps = 5;
            var initialCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 3, Y = 2 },
                new Coordinate { X = 3, Y = 3 }
            };
            var nextCoordinates = new List<Coordinate>
            {
                new Coordinate { X = 2, Y = 2 },
                new Coordinate { X = 2, Y = 3 },
                new Coordinate { X = 3, Y = 2 },
                new Coordinate { X = 3, Y = 3 }
            };

            _gameRepositoryMock.Setup(repo => repo.GetState(boardId)).ReturnsAsync(initialCoordinates);
            _gameRepositoryMock.Setup(repo => repo.InsertCoordinates(boardId, It.IsAny<List<Coordinate>>())).Returns(Task.CompletedTask);
            _gameRepositoryMock.Setup(repo => repo.DeleteCoordinates(It.IsAny<List<Coordinate>>())).Callback<List<Coordinate>>(coords =>
            {
                foreach (var coord in coords)
                {
                    initialCoordinates.RemoveAll(c => c.X == coord.X && c.Y == coord.Y);
                }
            }).Returns(Task.CompletedTask);

            // Act
            var result = await _gameService.GetFinalState(boardId, steps);

            // Assert
            Assert.AreEqual(nextCoordinates.Count, result.Count);
            foreach (var expectedCoordinate in nextCoordinates)
            {
                Assert.IsTrue(result.Any(c => c.X == expectedCoordinate.X && c.Y == expectedCoordinate.Y));
            }            
        }
    }
}
