using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4
{
    class Program
    {
        const byte BYR = 0b_1000_0000;
        const byte IYR = 0b_0100_0000;
        const byte EYR = 0b_0010_0000;
        const byte HGT = 0b_0001_0000;
        const byte HCL = 0b_0000_1000;
        const byte ECL = 0b_0000_0100;
        const byte PID = 0b_0000_0010;
        const byte CID = 0b_0000_0001;
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Valid passwords: {CountValid(ParseFlags(File.ReadAllLines(fileName)))}");
                    return;
                case 2:
                    Console.WriteLine($"Valid passwords: {CountValid(ParseFlagsWithValidation(File.ReadAllLines(fileName)))}");
                    return;
            }
        }

        private static List<byte> ParseFlagsWithValidation(string[] lines)
        {
            var flags = new List<byte> {0};

            var regex = new Regex(@"(?<key>\w\w\w):(?<value>[#\w\d]+)", RegexOptions.Compiled);

            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    flags.Add(0);
                    continue;
                }
                
                foreach (Match match in regex.Matches(line))
                {
                    var key = match.Groups["key"].Value;
                    var value = match.Groups["value"].Value;
                    switch (key)
                    {
                        case "byr" when int.TryParse(value, out var byr) && byr >= 1920 && byr <= 2002:
                            flags[^1] |= BYR;
                            break;
                        case "iyr" when int.TryParse(value, out var iyr) && iyr >= 2010 && iyr <= 2020:
                            flags[^1] |= IYR;
                            break;
                        case "eyr" when int.TryParse(value, out var eyr) && eyr >= 2020 && eyr <= 2030:
                            flags[^1] |= EYR;
                            break;
                        case "hgt":
                            var heightMatch = Regex.Match(value, @"(?<height>\d+)(?<unit>cm|in)");
                            if (heightMatch.Success)
                            {
                                var height = int.Parse(heightMatch.Groups["height"].Value);
                                var unit = heightMatch.Groups["unit"].Value;
                                switch (unit)
                                {
                                    case "cm" when height >= 150 && height <= 193:
                                        flags[^1] |= HGT;
                                        break;
                                    case "in" when height >= 59 && height <= 76:
                                        flags[^1] |= HGT;
                                        break;
                                }
                            }
                            break;
                        case "hcl" when Regex.IsMatch(value, @"^#[0-9a-f]{6}$"):
                            flags[^1] |= HCL;
                            break;
                        case "ecl" when Regex.IsMatch(value, @"(amb|blu|brn|gry|grn|hzl|oth)"):
                            flags[^1] |= ECL;
                            break;
                        case "pid" when Regex.IsMatch(value, @"^\d{9}$"):
                            flags[^1] |= PID;
                            break;
                        case "cid":
                            flags[^1] |= CID;
                            break;
                    }
                }
            }

            return flags;
        }

        private static int CountValid(IEnumerable<byte> flags)
        {
            const int validPasswordMask = 0b1111_1110;

            return flags.Count(flag => (flag & validPasswordMask) == validPasswordMask);
        }
        
        private static List<byte> ParseFlags(string[] lines)
        {
            var flags = new List<byte> {0};

            var regex = new Regex(@"(?<key>\w\w\w):(?<value>[#\w\d]+)", RegexOptions.Compiled);

            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    flags.Add(0);
                    continue;
                }
                
                foreach (Match match in regex.Matches(line))
                {
                    switch (match.Groups["key"].Value)
                    {
                        case "byr":
                            flags[^1] |= BYR;
                            break;
                        case "iyr":
                            flags[^1] |= IYR;
                            break;
                        case "eyr":
                            flags[^1] |= EYR;
                            break;
                        case "hgt":
                            flags[^1] |= HGT;
                            break;
                        case "hcl":
                            flags[^1] |= HCL;
                            break;
                        case "ecl":
                            flags[^1] |= ECL;
                            break;
                        case "pid":
                            flags[^1] |= PID;
                            break;
                        case "cid":
                            flags[^1] |= CID;
                            break;
                    }
                }
            }

            return flags;
        }
    }
}