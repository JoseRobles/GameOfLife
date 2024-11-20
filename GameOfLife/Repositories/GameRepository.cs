using GameOfLife.Interfaces;
using GameOfLife.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameOfLife.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateBoardWithCoordinates(List<Coordinate> coordinates)
        {
            if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));

            var newBoard = new Board
            {
                coordinates = coordinates,
                CreatedAt = DateTime.UtcNow
            };

            _context.Boards.Add(newBoard);
            await _context.SaveChangesAsync();

            return newBoard.Id;
        }

        public async Task<List<Coordinate>> GetState(int boardId)
        {
            var board = await _context.Boards
                .Include(b => b.coordinates)
                .FirstOrDefaultAsync(b => b.Id == boardId);
            if (board == null) throw new ArgumentException($"Board with id {boardId} not found");
            return board.coordinates;
        }

        public async Task InsertCoordinates(int boardId, List<Coordinate> newCoordinates)
        {
            var board = await _context.Boards.Include(b => b.coordinates).FirstOrDefaultAsync(b => b.Id == boardId);
            if (board != null)
            {
                // Agregar las nuevas coordenadas
                board.coordinates = newCoordinates;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCoordinate(int coordinateId)
        {
            var coordinate = await _context.Coordinate.FindAsync(coordinateId);
            if (coordinate != null)
            {
                _context.Coordinate.Remove(coordinate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCoordinates(List<Coordinate> coordinates)
        {
            _context.Coordinate.RemoveRange(coordinates);
            await _context.SaveChangesAsync();
        }
    }
}