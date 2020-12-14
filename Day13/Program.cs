using System;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Step 1: {SolvePart1(File.ReadAllLines(fileName))}");
                    return;
                case 2:
                    Console.WriteLine($"Step 2 was solved manually");
                    return;
            }
        }

        private static int SolvePart1(string[] input)
        {
            var timestamp = int.Parse(input[0]);
            var ids = input[1].Split(',').Where(id => id != "x").Select(int.Parse).ToArray();

            var firstBus = ids.Select(id => (busId: id, timeToWait: id - timestamp % id))
                .OrderBy(tuple => tuple.timeToWait)
                .First();

            return firstBus.timeToWait * firstBus.busId;
        }
    }
}