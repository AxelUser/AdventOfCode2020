using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day16
{
    class Program
    {
        private record RuleRange(int From, int To)
        {
            public bool Contains(int value) => value >= From && value <= To;
        }

        private record Rule(int Index, string Field, RuleRange[] Ranges)
        {
            public bool Match(int value) => Ranges.Any(range => range.Contains(value));
        }

        private record Input(Rule[] Rules, int[] YourTicket, int[][] NearbyTickets);
        
        private enum Section
        {
            None,
            Fields,
            YourTicket,
            NearbyTickets
        }
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var input = ReadInput(File.ReadLines(fileName));
            switch (part)
            {
                case 1:
                    Console.WriteLine($"Step 1: {SolvePart1(input)}");
                    return;
                case 2:
                    Console.WriteLine($"Step 2: {SolvePart2(input)}");
                    return;
            }   
        }

        private static int SolvePart1(Input input)
        {
            return input.NearbyTickets
                .Select(values => values.Where(v => !input.Rules.Any(rule => rule.Match(v))))
                .Sum(invalidValues => invalidValues.Sum());
        }
        
        private static long SolvePart2(Input input)
        {
            var (rules, yourTicket, nearbyTickets) = input;
            var validTickets =
                nearbyTickets.Where(ticketValues => ticketValues.All(v => rules.Any(rule => rule.Match(v))))
                    .ToArray();

            
            // Create array C, where C[i] contains set of field's indices, that match rule R[i] 
            (int fieldRuleId, HashSet<int> positions)[] fieldPossiblePositions = rules
                .Select(rule => (rule.Index, Enumerable
                    .Range(0, rules.Length)
                    .Where(position => validTickets.All(ticket => rule.Match(ticket[position])))
                    .ToHashSet()))
                .ToArray();

            var positionToFieldMap = new Dictionary<int, int>();
            var fieldToPositionMap = new Dictionary<int, int>();

            while (positionToFieldMap.Count < rules.Length)
            {
                var (fieldRuleId, positions) = fieldPossiblePositions
                    .Select(field => (fieldRuleId: field.fieldRuleId, positions: field.positions.Where(p => !positionToFieldMap.ContainsKey(p)).ToArray()))
                    .Where(field => field.positions.Any())
                    .OrderBy(field => field.positions.Count())
                    .First();
                
                positionToFieldMap.Add(positions.First(), fieldRuleId);
                fieldToPositionMap.Add(fieldRuleId, positions.First());
            }

            var sb = new StringBuilder();
            for (var i = 0; i < yourTicket.Length; i++)
            {
                sb.Append($"{rules[positionToFieldMap[i]].Field}:{yourTicket[i]} ");
            }
            Console.WriteLine(sb.ToString());
            
            return rules.Where(rule => rule.Field.StartsWith("departure"))
                .Select(rule => fieldToPositionMap[rule.Index])
                .Aggregate(1L,(mul, position) => mul * yourTicket[position]);
        }
        
        private static Input ReadInput(IEnumerable<string> lines)
        {
            const string yourTicketSection = "your ticket:";
            const string nearbyTicketsSection = "nearby tickets:";
            var fieldsRegex =
                new Regex(@"(?<field>[\w ]*): (?<smallest1>\d+)-(?<largest1>\d+) or (?<smallest2>\d+)-(?<largest2>\d+)");

            var rules = new List<Rule>();
            var nearbyTickets = new List<int[]>();
            int[] yourTicket = null;
            
            var currentSection = Section.Fields;
            
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    currentSection = Section.None;
                
                switch (currentSection)
                {
                    case Section.None:
                        currentSection = line switch
                        {
                            yourTicketSection => Section.YourTicket,
                            nearbyTicketsSection => Section.NearbyTickets,
                            _ => Section.None
                        };
                        break;
                    case Section.Fields:
                        rules.Add(ParseRule(line));
                        break;
                    case Section.YourTicket:
                        yourTicket = ParseTicketValues(line);
                        break;
                    case Section.NearbyTickets:
                        nearbyTickets.Add(ParseTicketValues(line));
                        break;
                }
            }

            return new Input(rules.ToArray(), yourTicket ?? Array.Empty<int>(), nearbyTickets.ToArray());
            
            Rule ParseRule(string rule)
            {
                var m = fieldsRegex.Match(rule);
                return new Rule(rules.Count,
                    m.Groups["field"].Value,
                    new[]
                    {
                        new RuleRange(int.Parse(m.Groups["smallest1"].Value),
                            int.Parse(m.Groups["largest1"].Value)),
                        new RuleRange(int.Parse(m.Groups["smallest2"].Value),
                            int.Parse(m.Groups["largest2"].Value))
                    });
            }

            int[] ParseTicketValues(string values) =>
                values.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
    }
}