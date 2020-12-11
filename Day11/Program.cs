using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    class Program
    {
        private const char Empty = 'L';
        private const char Occupied = '#';
        private const char Floor = '.';
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            switch (part)
            {
                case 1:
                    var part1 = CountOccupiedSeats(ReadGrid(File.ReadLines(fileName)),
                        (g, pos) => CheckSeatsPart1(g, pos.y, pos.x, 0),
                        (g, pos) => !CheckSeatsPart1(g, pos.y, pos.x, 3));
                    Console.WriteLine(
                        $"Occupied seats: {part1}");
                    return;
                case 2:
                    var part2 = CountOccupiedSeats(ReadGrid(File.ReadLines(fileName)),
                        (g, pos) => CheckSeatsPart2(g, pos.y, pos.x, 0),
                        (g, pos) => !CheckSeatsPart2(g, pos.y, pos.x, 4));
                    Console.WriteLine(
                        $"Occupied seats: {part2}");
                    return;
            }
        }

        private static int CountOccupiedSeats(char[][] grid,
            Func<char[][], (int y, int x), bool> shouldBeOccupied,
            Func<char[][], (int y, int x), bool> shouldBeFreed)
        {
            var width = grid[0].Length;
            var height = grid.Length;
            
            while (true)
            {
                var (changed, occupiedCount) = ApplyRules();
                if (!changed)
                    return occupiedCount;
            }

            (bool changed, int occupiedCount) ApplyRules()
            {
                var changed = false;
                var swapChangeMask = new bool[height, width];
                
                for (var yPos = 0; yPos < height; yPos++)
                {
                    for (var xPos = 0; xPos < width; xPos++)
                    {
                        swapChangeMask[yPos, xPos] = grid[yPos][xPos] switch
                        {
                            Occupied => shouldBeFreed(grid, (yPos, xPos)),
                            Empty => shouldBeOccupied(grid, (yPos, xPos)),
                            Floor => false
                        };
                        
                        if (swapChangeMask[yPos, xPos] && !changed)
                            changed = true;
                    }
                }

                var occupied = 0;
                for (var yPos = 0; yPos < height; yPos++)
                {
                    for (var xPos = 0; xPos < width; xPos++)
                    {
                        if (swapChangeMask[yPos, xPos])
                        {
                            grid[yPos][xPos] = grid[yPos][xPos] switch
                            {
                                Empty => Occupied,
                                Occupied => Empty,
                                Floor => Floor
                            };
                        }

                        if (grid[yPos][xPos] == Occupied) occupied++;
                    }
                }

                return (changed, occupied);
            }
        }
        
        private static bool CheckSeatsPart2(char[][] grid, int seatY, int seatX, int seatsOccupiedLimit)
        {
            var width = grid[0].Length;
            var height = grid.Length;
            
            var count = 0;
            var checkVectors = new (int y, int x)[]
            {
                (-1, 0),  // ↑
                (-1, 1),  // ↗
                (0, 1),   // →
                (1, 1),   // ↘
                (1, 0),   // ↓
                (1, -1),  // ↙
                (0, -1),  // ←
                (-1, -1)  // ↖
            };

            foreach (var (shiftY, shiftX) in checkVectors)
            {
                (int y, int x) current = (seatY, seatX);
                while (true)
                {
                    current = (current.y + shiftY, current.x + shiftX);
                    if (current.x < 0 || current.y < 0 || current.x >= width || current.y >= height)
                        break;
                        
                    if (grid[current.y][current.x] == Floor)
                        continue;
                        
                    if (grid[current.y][current.x] == Empty)
                        break;
                        
                    if (grid[current.y][current.x] == Occupied)
                    {
                        if (++count > seatsOccupiedLimit)
                            return false;
                        break;
                    }
                }
            }

            return true;
        }
        
        private static bool CheckSeatsPart1(char[][] grid, int seatY, int seatX, int seatsOccupiedLimit)
        {
            var width = grid[0].Length;
            var height = grid.Length;
            
            var count = 0;
            for (var y = seatY - 1; y <= seatY + 1; y++)
            {
                if (y < 0 || y >= height)
                    continue;
                    
                for (var x = seatX - 1; x <= seatX + 1; x++)
                {
                    if(x < 0 || x >= width || x == seatX && y == seatY)
                        continue;

                    if (grid[y][x] == Occupied && ++count > seatsOccupiedLimit)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private static char[][] ReadGrid(IEnumerable<string> lines)
        {
            return lines.Select(s => s.ToArray()).ToArray();
        } 
    }
}