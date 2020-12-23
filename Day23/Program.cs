using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];

            Console.WriteLine($"Part 1: {string.Join("", SolveCrabGame(input, input.Length, 100))}");

            var pair = SolveCrabGame(input, 1_000_000, 10_000_000).Take(2).ToArray();
            var part2 = (long)pair[0] * pair[1]; 
            Console.WriteLine($"Part 2: {part2}");
        }

        private static IEnumerable<int> SolveCrabGame(string input, int cupsCount, int moves)
        {
            // have labels from 1 to 9
            LinkedList<int> cups = new(input.Select(c => c - '0'));
            LinkedList<int> pickedCups = new();
            var nodeIndex = new LinkedListNode<int>[cupsCount + 1];

            var minLabel = cups.Min();
            var maxLabel = cups.Max();
            
            // If should add more values
            if (cupsCount > cups.Count)
            {
                foreach (var label in Enumerable.Range(maxLabel+1, cupsCount - cups.Count))
                {
                    cups.AddLast(label);
                    maxLabel = label;
                }
            }

            // Initialize node's index
            var node = cups.First;
            while (node != null)
            {
                nodeIndex[node.Value] = node;
                node = node.Next;
            }
            
            HashSet<int> availableLabels = new(cups);

            var current = cups.First;
            for (var move = 1; move <= moves; move++)
            {
                // Step 1: Pick up 3 cups
                var pickedCup = current.Next;

                for (var i = 0; i < 3; i++)
                {
                    pickedCup ??= cups.First;
                    var nextCup = pickedCup.Next;
                    
                    availableLabels.Remove(pickedCup.Value);
                    
                    cups.Remove(pickedCup);
                    pickedCups.AddLast(pickedCup);

                    pickedCup = nextCup;
                }


                // Step 2: find destination label
                var destLabel = current.Value - 1;
                while (!availableLabels.Contains(destLabel))
                {
                    if (destLabel < minLabel)
                    {
                        destLabel = maxLabel;
                    }
                    else
                    {
                        destLabel--;
                    }
                }
                
                // Step 3: place picked up cups after the destination node
                var insertAfter = nodeIndex[destLabel];
                var insertedNode = pickedCups.First;
                while (insertedNode != null)
                {
                    var nextNodeToInsert = insertedNode.Next;
                    
                    availableLabels.Add(insertedNode.Value);
                    pickedCups.Remove(insertedNode);
                    cups.AddAfter(insertAfter, insertedNode);

                    insertAfter = insertedNode;
                    insertedNode = nextNodeToInsert;
                }

                current = current.Next ?? cups.First;
            }
            
            var returnedCup = nodeIndex[1].Next ?? cups.First;

            while (returnedCup.Value != 1)
            {
                yield return returnedCup.Value;
                returnedCup = returnedCup.Next ?? cups.First;
            }
        }
    }
}