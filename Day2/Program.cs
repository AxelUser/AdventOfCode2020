using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var inputs = ParseInput(File.ReadAllText(fileName));

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Valid passwords: {Part1(inputs)}");
                    return;
                case 2:
                    Console.WriteLine($"Valid passwords: {Part2(inputs)}");
                    return;
            }
        }

        private static IEnumerable<(int firstNumber, int secondNumber, char symbol, string password)> ParseInput(string values)
        {
            const string pattern = @"(?<firstNumber>\d+)-(?<secondNumber>\d+)\s(?<symbol>\w):\s(?<password>\w+)";
            foreach (Match match in Regex.Matches(values, pattern, RegexOptions.Compiled))
            {
                var firstNumber = int.Parse(match.Groups["firstNumber"].Value);
                var secondNumber = int.Parse(match.Groups["secondNumber"].Value);
                var symbol = match.Groups["symbol"].Value[0];
                var password = match.Groups["password"].Value;

                yield return (firstNumber, secondNumber, symbol, password);
            }
        }

        private static int Part1(IEnumerable<(int min, int max, char symbol, string password)> inputs)
        {
            var validCount = 0;

            foreach (var (min, max, symbol, password) in inputs)
            {
                var charCount = 0;
                foreach (var c in password)
                {
                    if (c == symbol)
                        charCount++;
                    
                    if(charCount > max) break;
                }

                if (charCount >= min && charCount <= max)
                    validCount++;
            }
            
            return validCount;
        }
        
        private static int Part2(IEnumerable<(int firstPosition, int secondPosition, char symbol, string password)> inputs)
        {
            var validCount = 0;

            foreach (var (firstPosition, secondPosition, symbol, password) in inputs)
            {
                var wasFoundAtFirst = password[firstPosition - 1] == symbol;
                var wasFoundAtSecond = password[secondPosition - 1] == symbol;

                if (wasFoundAtFirst ^ wasFoundAtSecond)
                    validCount++;
            }
            
            return validCount;
        }
    }
}