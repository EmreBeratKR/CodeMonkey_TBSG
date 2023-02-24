using System;

namespace GridSystem
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int y;
        public int z;


        public GridPosition(int x, int z)
        {
            this.x = x;
            this.y = 0;
            this.z = z;
        }
        
        public GridPosition(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public bool Equals(GridPosition other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }
        
        public override string ToString()
        {
            return $"[{x}, {y}, {z}]";
        }


        public static bool operator ==(GridPosition lhs, GridPosition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(GridPosition lhs, GridPosition rhs)
        {
            return !(lhs == rhs);
        }
    }
}