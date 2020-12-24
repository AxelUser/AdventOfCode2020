using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day24
{
    class Program
    {
        private record Point(int X, int Y, int Z)
        {
            public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            public static readonly Point Zero = new Point(0, 0, 0);
        }

        // Great explanation here - https://www.redblobgames.com/grids/hexagons/
        private static readonly Point[] GridCubeCoordinatesOffsets = {
            new(0, 1, -1), // NW
            new(1, 0, -1), // NE
            new(1, -1, 0), // E
            new(0, -1, 1), // SE
            new(-1, 0, 1), // SW
            new(-1, 1, 0)  // W
        };
        
        [Flags]
        private enum Direction
        {
            None      = 0,
            South     = 1 << 0,
            North     = 1 << 1,
            East      = 1 << 2,
            West      = 1 << 3,
            SouthEast = South | East,
            SouthWest = South | West,
            NorthEast = North | East,
            NorthWest = North | West,
            IsIncomplete = South | North
        }
        
        static void Main(string[] args)
        {
            var fileName = args[0];
           
            var blackTiles = SolvePart1(File.ReadAllLines(fileName));
            Console.WriteLine("Part 1: " + blackTiles.Count);
            Console.WriteLine("Part 2: " + SolvePart2(blackTiles, 100).Count);
        }

        private static HashSet<Point> SolvePart2(IEnumerable<Point> initBlackPoints, int days)
        {
            var blackPoints = new HashSet<Point>(initBlackPoints);

            for (var day = 1; day <= days; day++)
            {
                var updatedPoints = new HashSet<Point>(blackPoints);
                
                foreach (var flippedToWhite in from blackPoint in blackPoints 
                                           let adjacentBlackCount = GetNeighbors(blackPoint).Count(blackPoints.Contains)
                                           where adjacentBlackCount == 0 || adjacentBlackCount > 2
                                           select blackPoint)
                {
                    updatedPoints.Remove(flippedToWhite);
                }

                foreach (var flippedToBlack in from adjacentWhite in blackPoints.SelectMany(GetNeighbors).Where(p => !blackPoints.Contains(p))
                                                let adjacentBlackCount = GetNeighbors(adjacentWhite).Count(blackPoints.Contains)
                                                where adjacentBlackCount == 2
                                                select adjacentWhite)           
                {
                    updatedPoints.Add(flippedToBlack);
                }

                blackPoints = updatedPoints;
            }

            return blackPoints;

            static IEnumerable<Point> GetNeighbors(Point point) =>
                GridCubeCoordinatesOffsets.Select(offset => point + offset);
        }
        
        private static HashSet<Point> SolvePart1(IEnumerable<string> lines)
        {
            var blackTiles = new HashSet<Point>();
            
            foreach (var line in lines)
            {
                var tile = Point.Zero;
                var direction = Direction.None;

                foreach (var c in line)
                {
                    direction |= c switch
                    {
                        'n' => Direction.North,
                        's' => Direction.South,
                        'e' => Direction.East,
                        'w' => Direction.West,
                        _ => Direction.None
                    };

                    if(Direction.IsIncomplete.HasFlag(direction)) continue;
                    
                    tile += direction switch
                    {
                        Direction.NorthWest => GridCubeCoordinatesOffsets[0],
                        Direction.NorthEast => GridCubeCoordinatesOffsets[1],
                        Direction.East => GridCubeCoordinatesOffsets[2],
                        Direction.SouthEast => GridCubeCoordinatesOffsets[3],
                        Direction.SouthWest => GridCubeCoordinatesOffsets[4],
                        Direction.West => GridCubeCoordinatesOffsets[5],
                        _ => Point.Zero
                    };

                    direction = Direction.None;
                }

                if (!blackTiles.Add(tile))
                    blackTiles.Remove(tile);
            }

            return blackTiles;
        }
    }
}