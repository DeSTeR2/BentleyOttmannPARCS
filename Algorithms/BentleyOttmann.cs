using System;
using System.Collections.Generic;
using BentleyOttmannModule.Geometry;

namespace BentleyOttmannModule.Algorithms
{
    // O((N + K) log N) sweep-line algorithm.
    // C# port of the C++ implementation in the OGKG project.
    public static class BentleyOttmann
    {
        private const double Eps = 1e-9;

        private enum EvType { Start = 0, Inter = 1, End = 2 }

        private sealed class Event : IComparable<Event>
        {
            public Point  P;
            public EvType Type;
            public int    S1, S2;
            public int    Uid; // tie-breaker for SortedSet uniqueness

            public int CompareTo(Event o)
            {
                if (Math.Abs(P.X - o.P.X) >= Eps) return P.X.CompareTo(o.P.X);
                if (Math.Abs(P.Y - o.P.Y) >= Eps) return P.Y.CompareTo(o.P.Y);
                int tc = ((int)Type).CompareTo((int)o.Type);
                if (tc != 0) return tc;
                return Uid.CompareTo(o.Uid);
            }
        }

        private struct SegKey : IComparable<SegKey>
        {
            public double Y, Slope;

            public int CompareTo(SegKey o)
            {
                if (Math.Abs(Y - o.Y) >= Eps) return Y.CompareTo(o.Y);
                return Slope.CompareTo(o.Slope);
            }
        }

        public static List<Intersection> Solve(IList<Segment> input)
        {
            int n = input.Count;
            var segs = new Segment[n];
            for (int i = 0; i < n; i++)
                segs[i] = new Segment(input[i].P.X, input[i].P.Y,
                                      input[i].Q.X, input[i].Q.Y, i);

            int uidCounter = 0;
            Event MakeEvent(Point p, EvType t, int s1, int s2)
                => new Event { P = p, Type = t, S1 = s1, S2 = s2, Uid = uidCounter++ };

            var Q = new SortedSet<Event>();
            for (int i = 0; i < n; i++)
            {
                Q.Add(MakeEvent(segs[i].P, EvType.Start, i, -1));
                Q.Add(MakeEvent(segs[i].Q, EvType.End,   i, -1));
            }

            // Status: segment IDs ordered by y at sweepX.
            var T       = new List<int>();
            double sweepX = double.MinValue;

            SegKey KeyOf(int s)
            {
                double slope = segs[s].IsVertical ? double.MaxValue : segs[s].Slope;
                return new SegKey { Y = segs[s].YAt(sweepX), Slope = slope };
            }

            int PosOf(int s)
            {
                for (int i = 0; i < T.Count; i++)
                    if (T[i] == s) return i;
                return -1;
            }

            var result         = new List<Intersection>();
            var reportedPairs  = new HashSet<long>();
            var scheduledInter = new HashSet<long>();

            long PairKey(int a, int b)
            {
                if (a > b) { int t = a; a = b; b = t; }
                return (long)a * 200003L + b;
            }

            long QuantKey(Point p, int a, int b)
            {
                long kx = (long)Math.Round(p.X * 1e6);
                long ky = (long)Math.Round(p.Y * 1e6);
                return kx * 1_000_003L + ky + PairKey(a, b) * 7_000_000_000L;
            }

            void TrySchedule(int a, int b)
            {
                if (a < 0 || b < 0 || a == b) return;
                long pk = PairKey(a, b);
                if (reportedPairs.Contains(pk)) return;
                if (!GeometryHelper.TryIntersect(segs[a], segs[b], out Point ip)) return;
                if (ip.X < sweepX - Eps) return;
                if (!scheduledInter.Add(QuantKey(ip, a, b))) return;
                Q.Add(MakeEvent(ip, EvType.Inter, a, b));
            }

            while (Q.Count > 0)
            {
                Event e = Q.Min;
                Q.Remove(e);
                sweepX = e.P.X;

                if (e.Type == EvType.Start)
                {
                    int s  = e.S1;
                    SegKey ks = KeyOf(s);
                    int pos = 0;
                    while (pos < T.Count && KeyOf(T[pos]).CompareTo(ks) < 0) pos++;
                    T.Insert(pos, s);
                    if (pos + 1 < T.Count) TrySchedule(s, T[pos + 1]);
                    if (pos > 0)            TrySchedule(s, T[pos - 1]);
                }
                else if (e.Type == EvType.End)
                {
                    int s   = e.S1;
                    int pos = PosOf(s);
                    if (pos < 0) continue;
                    int prev = pos > 0           ? T[pos - 1] : -1;
                    int next = pos + 1 < T.Count ? T[pos + 1] : -1;
                    T.RemoveAt(pos);
                    if (prev >= 0 && next >= 0) TrySchedule(prev, next);
                }
                else // Inter
                {
                    int  a  = e.S1, b = e.S2;
                    long pk = PairKey(a, b);
                    if (!reportedPairs.Add(pk)) continue;
                    result.Add(new Intersection(e.P, a, b));

                    int pa = PosOf(a), pb = PosOf(b);
                    if (pa < 0 || pb < 0) continue;
                    T[pa] = b; T[pb] = a;
                    int lo = Math.Min(pa, pb);
                    int hi = Math.Max(pa, pb);
                    if (lo > 0)            TrySchedule(T[lo], T[lo - 1]);
                    if (hi + 1 < T.Count)  TrySchedule(T[hi], T[hi + 1]);
                }
            }

            return result;
        }
    }
}
