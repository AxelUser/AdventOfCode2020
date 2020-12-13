using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        private static class Actions
        {
            public const char N = 'N';
            public const char S = 'S';
            public const char E = 'E';
            public const char W = 'W';
            public const char L = 'L';
            public const char R = 'R';
            public const char F = 'F';
        }

        private static class DirectionVectors
        {
            public static readonly (int x, int y) N = (0, 1);
            public static readonly (int x, int y) E = (1, 0);
            public static readonly (int x, int y) S = (0, -1);
            public static readonly (int x, int y) W = (-1, 0);
        }
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            IEnumerable<(char action, int units)> inputs =
                File.ReadLines(fileName).Select(line => (line[0], int.Parse(line.AsSpan(1))));

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Manhattan distance: {CountManhattanDistanceForPart1(inputs)}");
                    return;
                case 2:
                    Console.WriteLine($"Manhattan distance: {CountManhattanDistanceForPart2(inputs)}");
                    return;
            }
        }

        private static int CountManhattanDistanceForPart2(IEnumerable<(char action, int units)> inputs)
        {
            var posX = 0;
            var posY = 0;
            (int x, int y) waypoint = (10, 1); // starting with 10 East and 1 North

            foreach (var (action, units) in inputs)
            {
                switch (action)
                {
                    case Actions.N:
                        waypoint = (waypoint.x, waypoint.y + units);
                        break;
                    case Actions.E:
                        waypoint = (waypoint.x + units, waypoint.y);
                        break;
                    case Actions.S:
                        waypoint = (waypoint.x, waypoint.y - units);
                        break;
                    case Actions.W:
                        waypoint = (waypoint.x - units, waypoint.y);
                        break;
                    case Actions.L:
                        for (var leftRotation = 0; leftRotation < units / 90 % 4; leftRotation++)
                        {
                            waypoint = (-waypoint.y, waypoint.x);
                        }
                        break;
                    case Actions.R:
                        for (var rightRotation = 0; rightRotation < units / 90 % 4; rightRotation++)
                        {
                            waypoint = (waypoint.y, -waypoint.x);
                        }
                        break;
                    case Actions.F:
                        posX += waypoint.x * units;
                        posY += waypoint.y * units;
                        break;
                }
            }

            return Math.Abs(posX) + Math.Abs(posY);
        }

        
        private static int CountManhattanDistanceForPart1(IEnumerable<(char action, int units)> inputs)
        {
            var directions = new List<(int x, int y)> // clockwise list of directions
            {
                DirectionVectors.N,
                DirectionVectors.E,
                DirectionVectors.S,
                DirectionVectors.W,
            };
            
            var posX = 0;
            var posY = 0;
            var directionIdx = 1; // starting facing East

            foreach (var (action, units) in inputs)
            {
                switch (action)
                {
                    case Actions.N:
                        posY += units;
                        break;
                    case Actions.E:
                        posX += units;
                        break;
                    case Actions.S:
                        posY -= units;
                        break;
                    case Actions.W:
                        posX -= units;
                        break;
                    case Actions.L:
                        directionIdx = (directionIdx + 4 - units / 90 % 4) % 4;
                        break;
                    case Actions.R:
                        directionIdx = (directionIdx + units / 90) % 4;
                        break;
                    case Actions.F:
                        posX += directions[directionIdx].x * units;
                        posY += directions[directionIdx].y * units;
                        break;
                }
            }

            return Math.Abs(posX) + Math.Abs(posY);
        }
    }
}