using System;

namespace BentleyOttmannModule.Geometry
{
    [Serializable]
    public class Intersection
    {
        public Point P  { get; set; }
        public int  S1 { get; set; }
        public int  S2 { get; set; }

        public Intersection() { }

        public Intersection(Point p, int s1, int s2)
        {
            P  = p;
            S1 = Math.Min(s1, s2);
            S2 = Math.Max(s1, s2);
        }

        public override string ToString() => $"[{S1}∩{S2} @ {P}]";
    }
}
