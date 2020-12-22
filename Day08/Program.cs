using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day08
{
    class Program
    {
        private const string Nop = "nop";
        private const string Acc = "acc";
        private const string Jmp = "jmp";
        
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            switch (part)
            {
                case 1:
                    Console.WriteLine($"Accumulator before looping: {GetAccumulatorBeforeLooping(ReadCommands(File.ReadLines(fileName)))}");
                    return;
                case 2:
                    Console.WriteLine($"Accumulator before terminating: {GetAccumulatorForFixedProgram(ReadCommands(File.ReadLines(fileName)))}");
                    return;
            }
        }

        private static int GetAccumulatorForFixedProgram(List<Command> commands)
        {
            var isFixingMode = false;
            var pointer = 0;
            var accumulator = 0;
            var possibleFixes = new Queue<(int pointer, int accumulator)>();
            
            while (pointer < commands.Count)
            {
                if (commands[pointer].Visited)
                {
                    isFixingMode = true;
                    
                    if(!possibleFixes.TryDequeue(out var fix))
                        throw new Exception("Failed to interrupt loop");
                    
                    commands[fix.pointer].Name = commands[fix.pointer].Name switch
                    {
                        Nop => Jmp,
                        Jmp => Nop,
                        _ => commands[fix.pointer].Name
                    };

                    pointer = fix.pointer;
                    accumulator = fix.accumulator;
                }
                
                var current = commands[pointer];
                current.Visited = true;
                
                switch (current.Name)
                {
                    case Nop:
                        CheckPossibleFix(pointer + current.Argument);
                        pointer++;
                        break;
                    case Acc:
                        accumulator += commands[pointer].Argument;
                        pointer++;
                        break;
                    case Jmp:
                        CheckPossibleFix(pointer + 1);
                        pointer += commands[pointer].Argument;
                        break;
                    default:
                        throw new Exception($"Unsupported command {commands[pointer].Name}");
                }
            }

            return accumulator;

            void CheckPossibleFix(int possiblePointer)
            {
                if (isFixingMode) return;
                
                if (possiblePointer >= commands.Count || !commands[possiblePointer].Visited)
                {
                    possibleFixes.Enqueue((pointer, accumulator));
                }
            }
        }
        
        private static int GetAccumulatorBeforeLooping(List<Command> commands)
        {
            var pointer = 0;
            var accumulator = 0;
            
            while (!commands[pointer].Visited)
            {
                commands[pointer].Visited = true;
                switch (commands[pointer].Name)
                {
                    case Nop:
                        pointer++;
                        break;
                    case Acc:
                        accumulator += commands[pointer].Argument;
                        pointer++;
                        break;
                    case Jmp:
                        pointer += commands[pointer].Argument;
                        break;
                    default:
                        throw new Exception($"Unsupported command {commands[pointer].Name}");
                }
            }

            return accumulator;
        }

        private static List<Command> ReadCommands(IEnumerable<string> lines) =>
            lines
                .Select(line => line.Split(' '))
                .Select(v => new Command(v[0], int.Parse(v[1]), false))
                .ToList();
        
        private class Command
        {
            public Command(string name, int argument, bool visited)
            {
                Name = name;
                Argument = argument;
                Visited = visited;
            }

            public string Name { get; set; }
            public int Argument { get; set; }
            public bool Visited { get; set; }
        }
    }
}