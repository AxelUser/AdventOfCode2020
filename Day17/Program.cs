using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day17
{
    class Program
    {
        private record Cube(int X, int Y, int Z);
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);
            
            switch (part)
            {
                case 1:
                    Console.WriteLine($"Step 1: {SolvePart1(File.ReadAllLines(fileName))}");
                    return;
                case 2:
                    return;
            } 
        }

        private static int SolvePart1(string[] map)
        {
            var state = ReadActiveCubes(map);

            for (var cycle = 0; cycle < 6; cycle++)
            {
                state = CalculateNewState(state);
            }

            return state.Count;
        }

        private static HashSet<Cube> CalculateNewState(HashSet<Cube> prev)
        {
            var active = new HashSet<Cube>();
            var processed = new HashSet<Cube>();

            foreach (var activeCube in prev)
            {
                var activeCount = GetNeighbors(activeCube).Count(prev.Contains);

                if (activeCount == 2 || activeCount == 3)
                    active.Add(activeCube);

                processed.Add(activeCube);
            }

            foreach (var neighbor in prev.SelectMany(GetNeighbors))
            {
                if(processed.Contains(neighbor)) continue;

                if (GetNeighbors(neighbor).Count(prev.Contains) == 3)
                    active.Add(neighbor);

                processed.Add(neighbor);
            }

            return active;
        }

        private static IEnumerable<Cube> GetNeighbors(Cube current)
        {
            for (var y = current.Y - 1; y <= current.Y + 1; y++)
            {
                for (var x = current.X - 1; x <= current.X + 1; x++)
                {
                    for (var z = current.Z - 1; z <= current.Z + 1; z++)
                    {
                        var neighbor = new Cube(x, y, z);

                        if (neighbor != current)
                            yield return neighbor;
                    }
                }                
            }
        }

        private static HashSet<Cube> ReadActiveCubes(string[] map)
        {
            var active = new HashSet<Cube>();
            
            for (var y = 0; y < map.Length; y++)
            {
                for (var x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == '#')
                        active.Add(new Cube(x, y, 0));
                }
            }

            return active;
        }
    }
}