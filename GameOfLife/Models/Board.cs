namespace GameOfLife.Models
{
    public class Board
    {
        public int Id { get; set; }
        public required List<Coordinate> coordinates { get; set; }
        public DateTime CreatedAt { get; set; }

        public Board()
        {
            
        }
    }
}
