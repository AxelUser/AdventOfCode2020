using System;
using System.Collections.Generic;
using System.IO;
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

            var flags = ParseFlags(File.ReadAllLines(fileName));

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Valid passwords: {Part1(flags)}");
                    return;
                case 2:
                    return;
            }
        }

        private static int Part1(IEnumerable<byte> flags)
        {
            const int validPasswordMask = 0b1111_1110;
            var countValid = 0;

            foreach (var flag in flags)
            {

                if ((flag & validPasswordMask) == validPasswordMask)
                {
                    countValid++;
                }
            }
            
            return countValid;
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