using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day07
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
                    Console.WriteLine($"Bags: {CountAllGoldBagContainers(CreateBagsGraphPart1(File.ReadLines(fileName)))}");
                    return;
                case 2:
                    Console.WriteLine($"Bags: {CountAllBagsInGoldBag(CreateBagsGraphPart2(File.ReadLines(fileName)))}");
                    return;
            }
        }

        private static int CountAllGoldBagContainers(Dictionary<string, HashSet<string>> bags)
        {
            const string goldBag = "shiny gold";
            var goldBagsCount = 0;

            if (!bags.Any()) return goldBagsCount;

            var visited = new HashSet<string>();
            var stack = new Stack<string>();
            stack.Push(goldBag);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if(!bags.ContainsKey(current))
                    continue;

                foreach (var parent in bags[current])
                {
                    if (visited.Contains(parent)) continue;
                    
                    visited.Add(parent);
                    goldBagsCount++;
                    stack.Push(parent);
                }
            }

            return goldBagsCount;
        }
        
        private static Dictionary<string, HashSet<string>> CreateBagsGraphPart1(IEnumerable<string> lines)
        {
            const string noBags = "no other";
            
            var graph = new Dictionary<string, HashSet<string>>();
            
            var bagsRegex = new Regex(@"(?<quantity>\d+)?[\s]?(?<name>\w+\s\w+) bag[s]?", RegexOptions.Compiled);

            foreach (var line in lines)
            {
                var matches = bagsRegex.Matches(line);
                var parentBag = matches[0].Groups["name"].Value;

                for (var i = 1; i < matches.Count; i++)
                {
                    var childBagName = matches[i].Groups["name"].Value;
                    
                    if (childBagName == noBags) break;
                    
                    if(!graph.ContainsKey(childBagName))
                        graph.Add(childBagName, new HashSet<string>());
                    
                    graph[childBagName].Add(parentBag);
                }
            }

            return graph;
        }
        
        private static int CountAllBagsInGoldBag(Dictionary<string, Dictionary<string, int>> bags)
        {
            const string goldBag = "shiny gold";
            var totalBagsCount = 0;

            if (!bags.Any()) return totalBagsCount;
            
            var stack = new Stack<(string name, int? count)>();
            stack.Push((goldBag, null));

            while (stack.Count > 0)
            {
                var (currentName, currentCount) = stack.Pop();
                totalBagsCount += currentCount ?? 0;

                if(!bags.ContainsKey(currentName))
                    continue;

                foreach (var (childKey, bagsCount) in bags[currentName])
                {
                    stack.Push((childKey, bagsCount * (currentCount ?? 1)));
                }
            }

            return totalBagsCount;
        }
        
        private static Dictionary<string, Dictionary<string, int>> CreateBagsGraphPart2(IEnumerable<string> lines)
        {
            const string noBags = "no other";
            
            var graph = new Dictionary<string, Dictionary<string, int>>();
            
            var bagsRegex = new Regex(@"(?<quantity>\d+)?[\s]?(?<name>\w+\s\w+) bag[s]?", RegexOptions.Compiled);

            foreach (var line in lines)
            {
                var matches = bagsRegex.Matches(line);
                var parentBag = matches[0].Groups["name"].Value;

                for (var i = 1; i < matches.Count; i++)
                {
                    var childBagName = matches[i].Groups["name"].Value;
                    
                    if (childBagName == noBags) break;
                    
                    if(!graph.ContainsKey(parentBag))
                        graph.Add(parentBag, new Dictionary<string, int>());

                    var childBagCount = int.Parse(matches[i].Groups["quantity"].Value);
                    graph[parentBag].Add(childBagName, childBagCount);
                }
            }

            return graph;
        }

    }
}