using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day17
{
    class Program
    {
        private record Cube(int X, int Y, int Z = 0, int W = 0);

        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            Func<Cube, IEnumerable<Cube>> gerNeighbors = part switch
            {
                1 => GetNeighborsIn3D,
                2 => GetNeighborsIn4D
            };
            
            Console.WriteLine($"Part {part}: {CountActive(File.ReadAllLines(fileName), gerNeighbors)}");
        }

        private static int CountActive(string[] map, Func<Cube, IEnumerable<Cube>> getNeighbors)
        {
            var state = ReadActiveCubes(map);

            for (var cycle = 0; cycle < 6; cycle++)
            {
                state = CalculateNewState(state, getNeighbors);
            }

            return state.Count;
        }

        private static HashSet<Cube> CalculateNewState(HashSet<Cube> prev, Func<Cube, IEnumerable<Cube>> getNeighbors)
        {
            var active = new HashSet<Cube>();
            var processed = new HashSet<Cube>();

            foreach (var activeCube in prev)
            {
                var activeCount = getNeighbors(activeCube).Count(prev.Contains);

                if (activeCount == 2 || activeCount == 3)
                    active.Add(activeCube);

                processed.Add(activeCube);
            }

            foreach (var neighbor in prev.SelectMany(getNeighbors))
            {
                if(processed.Contains(neighbor)) continue;

                if (getNeighbors(neighbor).Count(prev.Contains) == 3)
                    active.Add(neighbor);

                processed.Add(neighbor);
            }

            return active;
        }

        private static IEnumerable<Cube> GetNeighborsIn3D(Cube current)
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
        
        private static IEnumerable<Cube> GetNeighborsIn4D(Cube current)
        {
            for (var y = current.Y - 1; y <= current.Y + 1; y++)
            {
                for (var x = current.X - 1; x <= current.X + 1; x++)
                {
                    for (var z = current.Z - 1; z <= current.Z + 1; z++)
                    {
                        for (var w = current.W - 1; w <= current.W + 1; w++)
                        {
                            var neighbor = new Cube(x, y, z, w);

                            if (neighbor != current)
                                yield return neighbor;
                        }
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
                        active.Add(new Cube(x, y));
                }
            }

            return active;
        }
    }
}