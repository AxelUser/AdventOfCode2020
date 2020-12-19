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

            var matchProvider = part switch
            {
                1 => CreateMatchingFunctionProviderForPart1(),
                2 => CreateMatchingFunctionProviderForPart2()
            };
            
            var count = Solve(File.ReadAllLines(fileName), matchProvider);
            Console.WriteLine($"Part {part}: {count}");
        }

        private static Func<Dictionary<string, string>, Func<string, bool>> CreateMatchingFunctionProviderForPart1()
        {
            return rules =>
            {
                var targetRegex = new Regex($"^{GetRegExPatternForRuleId("0", rules)}$");

                return s => targetRegex.IsMatch(s);
            };
        }
        
        // 0: 8 11 == 42+ 42 (42 (42 31) 31) 31
        // 8: 42 | 42 8 == 42+
        // 11: 42 31 | 42 11 31 == 42 (42 (42 31) 31) 31
        private static Func<Dictionary<string, string>, Func<string, bool>> CreateMatchingFunctionProviderForPart2()
        {
            return rules =>
            {
                var fullRegex42 = new Regex($"^({GetRegExPatternForRuleId("42", rules)}+)");
                var regex42 = new Regex($"{GetRegExPatternForRuleId("42", rules)}");
                var fullRegex31 = new Regex($"^({GetRegExPatternForRuleId("31", rules)}+)");
                var regex31 = new Regex($"{GetRegExPatternForRuleId("31", rules)}");

                return s =>
                {
                    var matches42 = regex42.Matches(s);
                    if (!matches42.Any())
                        return false;
                    
                    var count42 = matches42.Count;
                    var fullMatch42 = fullRegex42.Match(s);
                    var end42 = fullMatch42.Length;

                    var matches31 = regex31.Matches(s[end42..]);
                    if (!matches31.Any())
                        return false;
                    
                    var count31 = matches31.Count;
                    var fullMatch31 = fullRegex31.Match(s[end42..]);
                    var end31 = fullMatch31.Length;
                    
                    if (end42 + end31 != s.Length) return false;
                    var res = count31 >= 1 && count42 > count31;

                    if (res)
                    {
                        Console.WriteLine(s);
                    }
                    return res;
                };
            };
        }

        
        private static int Solve(string[] lines, Func<Dictionary<string, string>, Func<string, bool>> matchingProvider)
        {
            var ruleRegex = new Regex(@"(?<id>\d+):(?<rule>.+)");

            var rules = lines
                .Select(l => ruleRegex.Match(l))
                .TakeWhile(m => m.Success)
                .ToDictionary(match => match.Groups["id"].Value, match => match.Groups["rule"].Value);

            var createMatchFunction = matchingProvider(rules);
            return lines
                .SkipWhile(l => ruleRegex.IsMatch(l) || l.Length == 0)
                .Count(createMatchFunction);
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