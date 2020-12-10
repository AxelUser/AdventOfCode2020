using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
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
                    Console.WriteLine($"Multiplied 1 and 3 differences counts: {SolvePart1(File.ReadLines(fileName).Select(int.Parse))}");
                    return;
                case 2:
                    Console.WriteLine($"Total number of ways to solve: {SolvePart2(File.ReadLines(fileName).Select(int.Parse))}");
                    return;
            }
        }

        
        // Top down solution in DP
        private static long SolvePart2(IEnumerable<int> numbers)
        {
            var memorizedWays = new Dictionary<int, long>();

            var jolts = new List<int>(numbers) {0};
            jolts.Add(jolts.Max() + 3);
            
            jolts.Sort();
            
            return CountWaysFrom(0);
            
            
            long CountWaysFrom(int startIndex)
            {
                if (startIndex == jolts.Count - 1) // Already finished
                    return 1;
                
                if (memorizedWays.TryGetValue(startIndex, out var memorized))
                    return memorized;
                
                var count = 0L;
                for (var i = startIndex + 1; i < jolts.Count; i++)
                {
                    // Process all reachable items
                    if (jolts[i] - jolts[startIndex] <= 3)
                    {
                        count += CountWaysFrom(i);
                    }
                }

                memorizedWays[startIndex] = count;
                return count;
            }
        }
        
        private static int SolvePart1(IEnumerable<int> numbers)
        {
            var count1 = 0;
            // this is for the last difference, it's always 3. Now we don't need to find max.
            var count3 = 1;

            var joltsRating = new List<int>(numbers) {0};
            joltsRating.Sort();

            for (var i = 1; i < joltsRating.Count; i++)
            {
                var diff = joltsRating[i] - joltsRating[i - 1];

                if (diff == 1)
                    count1++;

                if (diff == 3)
                    count3++;
            }

            return count1 * count3;
        }
    }
}