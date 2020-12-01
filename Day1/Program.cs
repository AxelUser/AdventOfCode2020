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
            var part = int.Parse(args[1]);

            var inputs = File.ReadAllLines(fileName).Select(int.Parse).ToArray();

            switch (part)
            {
                case 1:
                    var pair = FindPair(inputs);
                    Console.WriteLine($"Result is {pair.first} * {pair.second} = {pair.first * pair.second}");
                    return;
                case 2:
                    var triplet = FindTriplet(inputs);
                    Console.WriteLine(
                        $"Result is {triplet.first} * {triplet.second} * {triplet.third} = {triplet.first * triplet.second * triplet.third}");
                    return;
            }

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

        private static (int first, int second, int third) FindTriplet(int[] values)
        {
            var complementaryValues = new Dictionary<int, int>();

            foreach (var value in values)
            {
                complementaryValues[2020 - value] = value;
            }

            for (var i = 0; i < values.Length; i++)
            {
                for (var j = 0; j < values.Length; j++)
                {
                    if(i == j) continue;
                    
                    var first = values[i];
                    var second = values[j];
                    if (complementaryValues.TryGetValue(first + second, out var third))
                    {
                        return (first, second, third);
                    }
                }
            }

            throw new Exception("Could not find triplet which sum is equal 2020");            
        }
    }
}