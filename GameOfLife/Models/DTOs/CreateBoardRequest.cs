namespace GameOfLife.Models.DTOs
{
    public class CreateBoardRequest
    {
        public required List<Coordinate> Coordinates { get; set; }
    }
}
