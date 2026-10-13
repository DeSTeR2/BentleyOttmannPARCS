using System;

namespace BentleyOttmannModule.Geometry
{
    [Serializable]
    public struct Point : IComparable<Point>
    {
        public const double Eps = 1e-9;

        public double X;
        public double Y;

        public Point(double x, double y) { X = x; Y = y; }

        // Sweep-line order: left-to-right, then top-to-bottom on tie.
        public int CompareTo(Point other)
        {
            if (Math.Abs(X - other.X) >= Eps) return X.CompareTo(other.X);
            if (Math.Abs(Y - other.Y) >= Eps) return Y.CompareTo(other.Y);
            return 0;
        }

        public bool Equals(Point other) =>
            Math.Abs(X - other.X) < Eps && Math.Abs(Y - other.Y) < Eps;

        public override string ToString() => $"({X:F4}, {Y:F4})";
    }
}
