using System;
using BentleyOttmannModule.Modules;

// Usage:
//   BentleyOttmannModule.exe <input_file> [workers_csv] [server_ip]
//
// Examples:
//   BentleyOttmannModule.exe segments.txt
//   BentleyOttmannModule.exe segments.txt 1,2,4,8
//   BentleyOttmannModule.exe segments.txt 1,2,4,8 10.0.0.5

namespace BentleyOttmannModule
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string inputFile = args.Length > 0 ? args[0] : "segments.txt";
            int[]  workers   = ParseWorkers(args.Length > 1 ? args[1] : "1,2,4,8");
            string serverIp  = args.Length > 2 ? args[2] : null;

            var module = new MainBentleyOttmannModule
            {
                InputFile = inputFile,
                Workers   = workers
            };

            module.RunModule(new ModuleOptions { ServerIp = serverIp });
        }

        private static int[] ParseWorkers(string csv)
        {
            var parts  = csv.Split(',');
            var result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = int.Parse(parts[i].Trim());
            return result;
        }
    }
}
