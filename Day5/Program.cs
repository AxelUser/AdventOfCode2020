using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
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
                    Console.WriteLine($"Max boarding pass id: {GetBoardingPassIds(File.ReadLines(fileName)).Max()}");
                    return;
                case 2:
                    Console.WriteLine($"Your boarding pass id: {FindBoardingPass(File.ReadLines(fileName))}");
                    return;
            }
        }

        private static IEnumerable<ushort> GetBoardingPassIds(IEnumerable<string> lines, int lowerBound = -1, int upperBound = 0b100_0000_0000)
        {
            foreach (var partitioning in lines)
            {
                ushort id = 0;
                ushort mask = 0b10_0000_0000;
                
                foreach (var sym in partitioning)
                {
                    if (sym == 'B' || sym == 'R')
                        id |= mask;
                    mask >>= 1;
                }
                
                if(id > lowerBound && id < upperBound)
                    yield return id;
            }
        }
        
        private static int FindBoardingPass(IEnumerable<string> lines)
        {
            var savedId = -1;
            foreach (int passId in GetBoardingPassIds(lines, 0b00_0000_0111, 0b11_1111_1000).OrderBy(id => id))
            {
                if (savedId == -1)
                {
                    savedId = passId;
                    continue;
                }

                if (passId - savedId > 2)
                    return passId - 1;

                savedId = passId;
            }

            throw new InvalidOperationException("Could not find Boarding Pass ID");
        }

    }
}