using System;
using System.IO;

namespace Day03
{
    public enum SquareType
    {
        Empty,
        Tree
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var matrix = Parse(File.ReadAllLines(fileName));

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Found trees: {Part1(matrix)}");
                    return;
                case 2:
                    Console.WriteLine($"Found trees: {Part2(matrix)}");
                    return;
            }
        }
        
        private static long CountTrees(SquareType[,] pattern, int rightMove, int downMove)
        {
            int x = 0, y = 0;
            var treesCount = 0L;

            var patternLength = pattern.GetLength(0);
            var patternWidth = pattern.GetLength(1);

            while (y < patternLength - 1)
            {
                x = (x + rightMove) % patternWidth;
                y += downMove;

                if (pattern[y, x] == SquareType.Tree)
                {
                    treesCount++;
                }
            }

            return treesCount;
        }


        private static long Part1(SquareType[,] pattern)
        {
            return CountTrees(pattern, 3, 1);
        }
        
        private static long Part2(SquareType[,] pattern)
        {
            return CountTrees(pattern, 1, 1) *
                   CountTrees(pattern, 3, 1) *
                   CountTrees(pattern, 5, 1) *
                   CountTrees(pattern, 7, 1) *
                   CountTrees(pattern, 1, 2);
        }

        private static SquareType[,] Parse(string[] map)
        {
            var matrix = new SquareType[map.Length, map[0].Length];
            for (var i = 0; i < map.Length; i++)
            {
                for (var j = 0; j < map[i].Length; j++)
                {
                    matrix[i, j] = map[i][j] == '#' ? SquareType.Tree : SquareType.Empty;
                }
            }

            return matrix;
        }
    }
}