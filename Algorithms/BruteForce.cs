using System.Collections.Generic;
using BentleyOttmannModule.Geometry;

namespace BentleyOttmannModule.Algorithms
{
    // O(N²) reference implementation — also the basis for parallel work-splitting.
    public static class BruteForce
    {
        public static List<Intersection> Solve(IList<Segment> segs)
        {
            return SolvePairRange(segs, 0, TotalPairs(segs.Count));
        }

        // Called by each PARCS worker with its assigned slice of pairs.
        public static List<Intersection> SolvePairRange(
            IList<Segment> segs, long pairStart, long pairEnd)
        {
            var result = new List<Intersection>();
            int n = segs.Count;
            long idx = 0;

            for (int i = 0; i < n; i++)
            {
                if (idx >= pairEnd) break;
                for (int j = i + 1; j < n; j++, idx++)
                {
                    if (idx < pairStart) continue;
                    if (idx >= pairEnd)  break;

                    if (GeometryHelper.TryIntersect(segs[i], segs[j], out Point p))
                        result.Add(new Intersection(p, i, j));
                }
            }

            return result;
        }

        public static long TotalPairs(int n) => (long)n * (n - 1) / 2;
    }
}
