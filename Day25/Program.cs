using System;
using System.IO;
using System.Linq;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var pks = File.ReadLines(fileName).Select(int.Parse).ToArray();

            var part1 = SolvePart1(pks[0], pks[1]);
            
            Console.WriteLine($"Part 1: {part1}");
        }

        private static long SolvePart1(long cardPk, long doorPk)
        {
            var cardLoopSize = FindLoopSize(cardPk, 7);
            var doorLoopSize = FindLoopSize(doorPk, 7);

            var cardEk = GetEncryptionKey(doorPk, cardLoopSize);
            var doorEk = GetEncryptionKey(cardPk, doorLoopSize);

            return cardEk == doorEk ? cardEk : throw new Exception($"Card EK: {cardEk}, Door EK: {doorEk}");

            static long FindLoopSize(long publicKey, long subjectNumber)
            {
                var count = 0;
                var value = 1L;
                while (value != publicKey)
                {
                    value = Calculate(value, subjectNumber);
                    count++;
                }

                return count;
            }

            static long GetEncryptionKey(long subjectNumber, long loopSize)
            {
                var value = 1L;
                
                for (var i = 0; i < loopSize; i++)
                {
                    value = Calculate(value, subjectNumber);
                }

                return value;
            }
        }

        private static long Calculate(long value, long subjectNumber) => value * subjectNumber % 20201227;
    }
}