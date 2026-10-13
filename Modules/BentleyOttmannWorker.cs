using System.Collections.Generic;
using System.Threading;
using BentleyOttmannModule.Algorithms;
using BentleyOttmannModule.Geometry;
using Parcs;

namespace BentleyOttmannModule.Modules
{
    // PARCS worker — receives a segment list and a pair range, returns intersections.
    public class BentleyOttmannWorker : IModule
    {
        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            IChannel ch = info.Parent;

            var segs      = ch.ReadObject<List<Segment>>();
            long pairStart = ch.ReadLong();
            long pairEnd   = ch.ReadLong();

            List<Intersection> partial = BruteForce.SolvePairRange(segs, pairStart, pairEnd);

            ch.WriteObject(partial);
        }
    }
}
