using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day20
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var result = SolvePart1(File.ReadAllText(fileName));
            
            Console.WriteLine($"Part {part}: {result}");
        }

        private static long SolvePart1(string text)
        {
            var tiles = Parse(text);
            var adj = GetAdjacent(tiles);
            var corners = adj.Where(pair => pair.Value.Count == 2);
            return corners.Aggregate(1L, (mul, pair) => mul * pair.Key);
        }

        private static Dictionary<int, HashSet<int>> GetAdjacent(Tile[] tiles)
        {
            var adjacent = new Dictionary<int, HashSet<int>>();

            foreach (var tileA in tiles)
            {
                adjacent[tileA.Id] = new HashSet<int>();
                foreach (var tileB in tiles)
                {
                    if(tileA.Id == tileB.Id) continue;

                    if (TryFindAdjacentPosition(tileA, tileB, out _))
                    {
                        adjacent[tileA.Id].Add(tileB.Id);
                    }
                }    
            }

            return adjacent;
        }
        
        private enum Side
        {
            Left,
            Right,
            Top,
            Bottom
        }

        private record AdjacentTile(Tile Tile, Side Side);

        private static bool TryFindAdjacentPosition(Tile main, Tile possibleAdj, out AdjacentTile adjacent)
        {
            for (var rotation = 0; rotation < 4; rotation++)
            {
                if (CheckAdjacencyOnEachSide(main, out adjacent))
                {
                    return true;
                }

                if (CheckAdjacencyOnEachSide(main.FlipVertically(), out adjacent))
                {
                    return true;
                }
                
                if (CheckAdjacencyOnEachSide(main.FlipHorizontally(), out adjacent))
                {
                    return true;
                }

                main = main.RotateLeft();
            }

            adjacent = default;
            return false;

            bool CheckAdjacencyOnEachSide(Tile checkedTile, out AdjacentTile adj)
            {
                if (checkedTile.CanPlaceTop(possibleAdj))
                {
                    adj = new AdjacentTile(checkedTile, Side.Top);
                    return true;
                }
                
                if (checkedTile.CanPlaceBottom(possibleAdj))
                {
                    adj = new AdjacentTile(checkedTile, Side.Bottom);
                    return true;
                }
                
                if (checkedTile.CanPlaceLeft(possibleAdj))
                {
                    adj = new AdjacentTile(checkedTile, Side.Left);
                    return true;
                }
                
                if (checkedTile.CanPlaceRight(possibleAdj))
                {
                    adj = new AdjacentTile(checkedTile, Side.Right);
                    return true;
                }

                adj = default;
                return false;
            }
        }
        
        private static Tile[] Parse(string text)
        {
            var tileIdRegex = new Regex(@"Tile (?<id>\d+):");

            return text
                .Split(new [] {Environment.NewLine + Environment.NewLine, "\n\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split('\n'))
                .Select(lines =>
                {
                    var id = int.Parse(tileIdRegex.Match(lines[0]).Groups["id"].Value);
                    var map = new char[lines.Length - 1, lines.Length - 1];
                    for (var i = 0; i < map.GetLength(0); i++)
                    {
                        for (var j = 0; j < map.GetLength(1); j++)
                        {
                            map[i, j] = lines[i + 1][j];
                        }
                    }

                    return new Tile(id, map);
                }).ToArray();
        }
    }
}