using System;

namespace BentleyOttmannModule.Geometry
{
    [Serializable]
    public class Segment
    {
        public Point P { get; set; }
        public Point Q { get; set; }
        public int Id  { get; set; }

        public Segment() { }

        public Segment(double x1, double y1, double x2, double y2, int id)
        {
            P = new Point(x1, y1);
            Q = new Point(x2, y2);
            Id = id;
            Normalize();
        }

        // Ensures P is the left (or upper) endpoint for the sweep.
        private void Normalize()
        {
            if (Q.CompareTo(P) < 0)
            {
                Point tmp = P; P = Q; Q = tmp;
            }
        }

        public double YAt(double x)
        {
            if (Math.Abs(Q.X - P.X) < Point.Eps) return P.Y;
            double t = (x - P.X) / (Q.X - P.X);
            return P.Y + t * (Q.Y - P.Y);
        }

        public bool IsVertical => Math.Abs(Q.X - P.X) < Point.Eps;

        public double Slope =>
            IsVertical ? double.MaxValue : (Q.Y - P.Y) / (Q.X - P.X);
    }
}
