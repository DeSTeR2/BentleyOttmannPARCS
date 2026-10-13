using System;

namespace BentleyOttmannModule.Geometry
{
    public static class GeometryHelper
    {
        // 2D cross product of (b-a) × (c-a).
        public static double Cross(Point a, Point b, Point c) =>
            (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

        public static int Sign(double v)
        {
            if (v >  Point.Eps) return  1;
            if (v < -Point.Eps) return -1;
            return 0;
        }

        // Returns true if s1 and s2 intersect (proper or at endpoint).
        // Writes the intersection point into `result` when true.
        public static bool TryIntersect(Segment s1, Segment s2, out Point result)
        {
            result = default;
            Point a = s1.P, b = s1.Q, c = s2.P, d = s2.Q;

            double d1 = Cross(c, d, a), d2 = Cross(c, d, b);
            double d3 = Cross(a, b, c), d4 = Cross(a, b, d);

            int sd1 = Sign(d1), sd2 = Sign(d2);
            int sd3 = Sign(d3), sd4 = Sign(d4);

            // Proper intersection.
            if (((sd1 > 0 && sd2 < 0) || (sd1 < 0 && sd2 > 0)) &&
                ((sd3 > 0 && sd4 < 0) || (sd3 < 0 && sd4 > 0)))
            {
                double t = d1 / (d1 - d2);
                result = new Point(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
                return true;
            }

            // Collinear / endpoint touching.
            if (sd1 == 0 && OnSegment(c, d, a)) { result = a; return true; }
            if (sd2 == 0 && OnSegment(c, d, b)) { result = b; return true; }
            if (sd3 == 0 && OnSegment(a, b, c)) { result = c; return true; }
            if (sd4 == 0 && OnSegment(a, b, d)) { result = d; return true; }

            return false;
        }

        private static bool OnSegment(Point p, Point q, Point r) =>
            Math.Min(p.X, q.X) - Point.Eps <= r.X && r.X <= Math.Max(p.X, q.X) + Point.Eps &&
            Math.Min(p.Y, q.Y) - Point.Eps <= r.Y && r.Y <= Math.Max(p.Y, q.Y) + Point.Eps;
    }
}
