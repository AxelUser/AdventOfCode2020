using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];

            var inputs = File.ReadAllLines(fileName).Select(int.Parse).ToArray();

            var (first, second) = FindPair(inputs);
            Console.WriteLine($"Result is {first} * {second} = {first * second}");
        }

        private static (int first, int second) FindPair(int[] values)
        {
            var complementaryValues = new Dictionary<int, int>();

            foreach (var value in values)
            {
                complementaryValues[2020 - value] = value;
            }
            
            foreach (var value in values)
            {
                if (complementaryValues.TryGetValue(value, out var complementaryValue))
                {
                    return (value, complementaryValue);
                }
            }

            throw new Exception("Could not find pair which sum is equal 2020");
        }
    }
}