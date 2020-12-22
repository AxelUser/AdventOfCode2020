using System;
using System.IO;
using System.Linq;

namespace Day09
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);
            var valueForStep2 = args.Length == 3 ? long.Parse(args[2]) : (long?) null;

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Elements: {FindWeakness(File.ReadLines(fileName).Select(long.Parse).ToArray(), 25)}");
                    return;
                case 2:
                    var expectedSum = valueForStep2 ??
                                      FindWeakness(File.ReadLines(fileName).Select(long.Parse).ToArray(), 25);

                    var (min, max) =
                        FindMinAndMaxOfContinuousSet(File.ReadLines(fileName).Select(long.Parse).ToArray(), expectedSum);
                    Console.WriteLine($"Encryption weakness: {min + max}");
                    return;
            }
        }

        private static (long min, long max) FindMinAndMaxOfContinuousSet(long[] numbers, long expectedSum)
        {
            if(numbers.Length < 2)
                throw new Exception("List should contain at lease 2 numbers");
            
            var left = 0;
            var right = 1;
           
            var sum = numbers[left] + numbers[right];

            while (sum != expectedSum && right < numbers.Length)
            {
                while (sum < expectedSum && right < numbers.Length)
                {
                    sum += numbers[++right];
                }
                
                while (sum > expectedSum && left < right)
                {
                    sum -= numbers[left++];
                }
            }

            if (sum == expectedSum)
            {
                var range = numbers.Skip(left).Take(right - left + 1);
                return (range.Min(), range.Max());
            }
            
            throw new Exception("Could not found encryption weakness");
        }
        
        private static long FindWeakness(long[] numbers, int preambleLength)
        {
            if(numbers.Length <= preambleLength)
                throw new Exception($"List contains less than {preambleLength} records");

            (int, int)? foundPair = null; 
            for (var numberIdx = preambleLength; numberIdx < numbers.Length; numberIdx++)
            {
                foundPair = null;
                for (var firstIdx = numberIdx - preambleLength; firstIdx < numberIdx && foundPair == null; firstIdx++)
                {
                    for (var secondIdx = firstIdx + 1; secondIdx < numberIdx; secondIdx++)
                    {
                        if (numbers[numberIdx] != numbers[firstIdx] + numbers[secondIdx]) continue;
                        foundPair = (firstIdx, secondIdx);
                        break;
                    }
                }

                if (foundPair == null)
                    return numbers[numberIdx];
            }
            
            throw new Exception("Could not found weak element");
        }
    }
}