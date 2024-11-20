using System.Collections.Generic;

namespace GameOfLife.Models.Comparers
{    
    public class CoordinateComparer : IEqualityComparer<Coordinate>
    {
        public bool Equals(Coordinate c1, Coordinate c2)
        {
            if (c1 == null && c2 == null)
                return true;
            if (c1 == null || c2 == null)
                return false;
            return c1.X == c2.X && c1.Y == c2.Y;
        }

        public int GetHashCode(Coordinate c)
        {
            return HashCode.Combine(c.X, c.Y);
        }
    }
}
