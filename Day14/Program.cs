using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14
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
                    Console.WriteLine($"Step 1: {SolvePart1(File.ReadLines(fileName))}");
                    return;
                case 2:
                    Console.WriteLine($"Step 2: {SolvePart2(File.ReadLines(fileName))}");
                    return;
            }
        }

        private static long SolvePart2(IEnumerable<string> lines)
        {
            var maskRegex = new Regex(@"mask = (?<mask>[01X]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var memorySetRegex = new Regex(@"mem\[(?<address>\d*)\] = (?<value>\d*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var memory = new Dictionary<long, long>();
            var mask = new char[36];
            foreach (var line in lines)
            {
                if (maskRegex.IsMatch(line))
                {
                    mask = maskRegex.Match(line).Groups["mask"].Value.ToArray();
                }
                else
                {
                    var match = memorySetRegex.Match(line);
                    var address = Convert.ToString(long.Parse(match.Groups["address"].Value), 2)
                        .PadLeft(36, '0').ToArray();
                    var value = long.Parse(match.Groups["value"].Value);
                    var maskedAddress = mask.Zip(address, (m, a) => m switch
                    {
                        '0' => a,
                        'X' => 'X',
                        _ => m
                    }).ToArray();

                    foreach (var affectedAddress in GetAddresses(maskedAddress))
                    {
                        memory[affectedAddress] = value;
                    }
                }
            }

            return memory.Values.Sum();
        }

        private static IEnumerable<long> GetAddresses(char[] mask)
        {
            // we have bitmask with 0, 1 and X
            // when we met X at position i, we invoke function t(i)
            // t(i) will add 2 new bitmasks to the list, with bits 1 and 0 at position i
            // if list already contain masks, then we override each mask with 1 and 0 bits at position i
            // when we finish processing original mask we should have addresses without any X
            // if we didn't met X, then we just add original mask, which is a valid address, to the list
            // e.g:
            // mask: 000000000000000000000000000000X1101X
            // at position 30 we met first X
            // we add masks: 000000000000000000000000000000(1)1101X
            //               000000000000000000000000000000(0)1101X
            // at position 35 we met second X
            // we override each mask with two, passing 1 and 0 at position 35
            // first one we override with: 000000000000000000000000000000(1)1101(1)
            //                             000000000000000000000000000000(1)1101(0)
            // second one we override with: 000000000000000000000000000000(0)1101(1)
            //                              000000000000000000000000000000(0)1101(0)
            // at the end we will have addresses:
            //    000000000000000000000000000000(1)1101(1)
            //    000000000000000000000000000000(1)1101(0)
            //    000000000000000000000000000000(0)1101(1)
            //    000000000000000000000000000000(0)1101(0)

            var addresses = new List<char[]> {mask};

            for (var bitIdx = 0; bitIdx < mask.Length; bitIdx++)
            {
                switch (mask[bitIdx])
                {
                    case '0':
                    case '1':
                        continue;
                    case 'X':
                        var newAddresses = new List<char[]>();
                        foreach (var address in addresses)
                        {
                            foreach (var bit in new[] {'0', '1'})
                            {
                                var newAddress = new char[address.Length];
                                address.CopyTo(newAddress, 0);
                                newAddress[bitIdx] = bit;
                                newAddresses.Add(newAddress);
                            }
                        }

                        addresses = newAddresses;
                        break;
                }
            }

            return addresses.Select(a => Convert.ToInt64(new string(a), 2));
        }

        private static long SolvePart1(IEnumerable<string> lines)
        {
            var maskRegex = new Regex(@"mask = (?<mask>[01X]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var memorySetRegex = new Regex(@"mem\[(?<address>\d*)\] = (?<value>\d*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var andMask = long.MaxValue;
            var orMask = 0L;
            
            var memory = new Dictionary<long, long>();

            foreach (var line in lines)
            {
                if (maskRegex.IsMatch(line))
                {
                    var mask = maskRegex.Match(line).Groups["mask"].Value;
                    andMask = Convert.ToInt64(mask.Replace('X', '1'), 2);
                    orMask = Convert.ToInt64(mask.Replace('X', '0'), 2);
                }
                else
                {
                    var match = memorySetRegex.Match(line);
                    var address = long.Parse(match.Groups["address"].Value);
                    var value = long.Parse(match.Groups["value"].Value);
                    memory[address] = value & andMask | orMask;
                }
            }

            return memory.Values.Sum();
        }
    }
}