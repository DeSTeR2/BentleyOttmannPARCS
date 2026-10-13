using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BentleyOttmannModule.Algorithms;
using BentleyOttmannModule.Geometry;
using Parcs;

namespace BentleyOttmannModule.Modules
{
    public class MainBentleyOttmannModule : MainModule
    {
        // Set before calling RunModule().
        public string InputFile  { get; set; } = "segments.txt";
        public int[]  Workers    { get; set; } = { 1, 2, 4, 8 };

        public override void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            List<Segment> segs = LoadSegments(InputFile);
            Console.WriteLine($"Loaded {segs.Count} segments from '{InputFile}'.");
            Console.WriteLine();

            // --- Sequential: Bentley-Ottmann ---
            var sw = Stopwatch.StartNew();
            List<Intersection> boResult = BentleyOttmann.Solve(segs);
            sw.Stop();
            Console.WriteLine($"[Sequential] Bentley-Ottmann : {boResult.Count,8} intersections  {sw.Elapsed.TotalMilliseconds,10:F2} ms");

            // --- Sequential: Brute-Force baseline ---
            sw.Restart();
            List<Intersection> bfResult = BruteForce.Solve(segs);
            sw.Stop();
            long seqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"[Sequential] Brute-Force O(N²): {bfResult.Count,8} intersections  {sw.Elapsed.TotalMilliseconds,10:F2} ms");
            Console.WriteLine();

            // --- Parallel brute-force for each worker count ---
            Console.WriteLine($"{"Workers",-10} {"Intersections",-16} {"Time (ms)",-14} {"Speedup"}");
            Console.WriteLine(new string('-', 52));

            foreach (int numWorkers in Workers)
            {
                sw.Restart();
                List<Intersection> parResult = RunParallel(info, segs, numWorkers);
                sw.Stop();
                double ms      = sw.Elapsed.TotalMilliseconds;
                double speedup = seqMs / ms;
                Console.WriteLine($"{numWorkers,-10} {parResult.Count,-16} {ms,-14:F2} {speedup:F2}x");
            }
        }

        private List<Intersection> RunParallel(ModuleInfo info, List<Segment> segs, int numWorkers)
        {
            long total     = BruteForce.TotalPairs(segs.Count);
            long perWorker = (total + numWorkers - 1) / numWorkers;

            var points   = new IPoint[numWorkers];
            var channels = new IChannel[numWorkers];

            for (int i = 0; i < numWorkers; i++)
            {
                points[i]   = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
            }

            // Dispatch work.
            for (int i = 0; i < numWorkers; i++)
            {
                long start = (long)i * perWorker;
                long end   = Math.Min(start + perWorker, total);

                channels[i].WriteObject(segs);
                channels[i].WriteData(start);
                channels[i].WriteData(end);
                points[i].ExecuteClass(typeof(BentleyOttmannWorker).FullName);
            }

            // Collect results.
            var result = new List<Intersection>();
            for (int i = 0; i < numWorkers; i++)
                result.AddRange(channels[i].ReadObject<List<Intersection>>());

            return result;
        }

        // -----------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------

        private static List<Segment> LoadSegments(string path)
        {
            var segs  = new List<Segment>();
            var lines = File.ReadAllLines(path);
            int idx   = 0;
            int n     = int.Parse(lines[idx++].Trim());
            for (int i = 0; i < n; i++)
            {
                string[] tok = lines[idx++].Trim().Split(
                    new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                double x1 = double.Parse(tok[0]);
                double y1 = double.Parse(tok[1]);
                double x2 = double.Parse(tok[2]);
                double y2 = double.Parse(tok[3]);
                segs.Add(new Segment(x1, y1, x2, y2, i));
            }
            return segs;
        }
    }
}
