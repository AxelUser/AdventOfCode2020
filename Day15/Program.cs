using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var position = part switch
            {
                1 => 2020,
                2 => 30000000
            };

            var startingNumbers = File.ReadAllText(fileName).Split(',').Select(int.Parse);
            var result = VanEckSequence(startingNumbers).Skip(position - 1).Take(1).Last();
            
            Console.WriteLine($"{position}th value: {result}");
        }

        private static IEnumerable<int> VanEckSequence(IEnumerable<int> startingSequence)
        {
            var lastSeen = new Dictionary<int, int>();
            var lastValue = 0;
            var totalCount = 0;

            foreach (var next in startingSequence)
            {
                totalCount++;
                lastSeen[next] = totalCount;
                lastValue = next;
                yield return lastValue;
            }

            while (true)
            {
                var vanEckSeqValue = lastSeen.TryGetValue(lastValue, out var lastIdx) ? totalCount - lastIdx : 0;
                lastSeen[lastValue] = totalCount;
                lastValue = vanEckSeqValue;
                yield return lastValue;
                totalCount++;
            }
        }
    }
}