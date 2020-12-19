using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day19
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var count = Solve(File.ReadAllLines(fileName));
            Console.WriteLine($"Part {part}: {count}");
        }

        private static int Solve(string[] lines)
        {
            var ruleRegex = new Regex(@"(?<id>\d+):(?<rule>.+)");

            var rules = lines
                .Select(l => ruleRegex.Match(l))
                .TakeWhile(m => m.Success)
                .ToDictionary(match => match.Groups["id"].Value, match => match.Groups["rule"].Value);

            var targetRegex = new Regex($"^{GetRegExPatternForRuleId("0", rules)}$");
            return lines
                .SkipWhile(l => ruleRegex.IsMatch(l) || l.Length == 0)
                .Count(l => targetRegex.IsMatch(l));
        }

        private static readonly Regex CharRegex = new("\"(?<char>\\w)\"");
        private static readonly Regex RuleIdRegex = new(@"(?<ruleId>\d+)");
        
        private static string GetRegExPatternForRuleId(string ruleId, Dictionary<string,string > rules)
        {
            return GetRegExPatternForRule(rules[ruleId]);

            string GetRegExPatternForRule(string rule)
            {
                // Handle terminal character
                if (CharRegex.IsMatch(rule))
                {
                    return CharRegex.Match(rule).Groups["char"].Value;
                }
                
                // Handle OR
                if (rule.Contains('|'))
                {
                    // Should have 2 rule groups
                    var opts = rule.Split('|');
                    return $"({GetRegExPatternForRule(opts[0])}|{GetRegExPatternForRule(opts[1])})";
                }
                
                // Handle rule ids
                var rulePatterns = RuleIdRegex
                    .Matches(rule)
                    .Select(m => m.Groups["ruleId"].Value)
                    .Select(id => GetRegExPatternForRuleId(id, rules));

                return string.Concat(rulePatterns);
            }
        }
    }
}