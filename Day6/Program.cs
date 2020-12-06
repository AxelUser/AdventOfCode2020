using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
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
                    Console.WriteLine($"Sum: {GetQuestionsAnsweredYes(File.ReadLines(fileName)).Sum(set => set.Count)}");
                    return;
                case 2:
                    Console.WriteLine($"Sum: {GetQuestionsAllAnsweredYes(File.ReadLines(fileName)).Sum(set => set.Count)}");
                    return;
            }
        }

        private static IEnumerable<HashSet<char>> GetQuestionsAnsweredYes(IEnumerable<string> lines)
        {
            HashSet<char> questions = null;
            
            foreach (var line in lines)
            {
                questions ??= new HashSet<char>();
                
                if (line.Length == 0)
                {
                    yield return questions;
                    questions = null;
                    continue;
                }

                foreach (var question in line)
                {
                    questions.Add(question);
                }
            }

            if(questions != null)
                yield return questions;
        }
        
        private static IEnumerable<HashSet<char>> GetQuestionsAllAnsweredYes(IEnumerable<string> lines)
        {
            HashSet<char> positiveForWholeGroup = null;

            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    yield return positiveForWholeGroup;
                    positiveForWholeGroup = null;
                    continue;
                }

                var positiveForCurrentPerson = new HashSet<char>(line);
                if (positiveForWholeGroup != null)
                {
                    positiveForWholeGroup.IntersectWith(positiveForCurrentPerson);
                }
                else
                {
                    positiveForWholeGroup = positiveForCurrentPerson;
                }
            }

            if(positiveForWholeGroup != null)
                yield return positiveForWholeGroup;
        }

    }
}