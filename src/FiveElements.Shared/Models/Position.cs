using System;

namespace FiveElements.Shared.Models
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Position other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public double DistanceToOrigin()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Position other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(Position? left, Position? right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }
    }
}