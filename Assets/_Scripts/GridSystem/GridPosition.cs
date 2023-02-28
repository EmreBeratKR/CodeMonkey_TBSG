using System;

namespace GridSystem
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public static readonly GridPosition Zero = new(0, 0, 0);
        public static readonly GridPosition One = new(1, 0, 1);
        
        
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


        public static float Distance(GridPosition a, GridPosition b)
        {
            double x = a.x - b.x;
            double y = a.y - b.y;
            double z = a.z - b.z;
            return (float) Math.Sqrt(x * x + y * y + z * z);
        }

        public static float SqrDistance(GridPosition a, GridPosition b)
        {
            double x = a.x - b.x;
            double y = a.y - b.y;
            double z = a.z - b.z;
            return (float) (x * x + y * y + z * z);
        }
        
        
        public static bool operator ==(GridPosition lhs, GridPosition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(GridPosition lhs, GridPosition rhs)
        {
            return !(lhs == rhs);
        }

        public static GridPosition operator +(GridPosition lhs, GridPosition rhs)
        {
            return new GridPosition(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        
        public static GridPosition operator -(GridPosition lhs, GridPosition rhs)
        {
            return new GridPosition(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static GridPosition operator *(GridPosition lhs, int rhs)
        {
            return new GridPosition(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }
    }
}