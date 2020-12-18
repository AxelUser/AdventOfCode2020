using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            // Higher number - higher priority
            var precedence = part switch
            {
                1 => new Dictionary<char, int> 
                {
                    {Operations.Add, 0},
                    {Operations.Mul, 0},
                },
                2 => new Dictionary<char, int>
                {
                    {Operations.Add, 1},
                    {Operations.Mul, 0},
                },
            };

            var result = Solve(File.ReadLines(fileName), precedence);
            
            Console.WriteLine($"Part {part}: {result}");
        }
        
        private static class Operations
        {
            public const char Add = '+';
            public const char Mul = '*';
        }

        private static long Solve(IEnumerable<string> lines, Dictionary<char, int> precedence)
        {
            return lines
                .Select(l => ReadRPN(l, precedence))
                /*.Select(p =>
                {
                    var sb = new StringBuilder();
                    foreach (var term in p)
                    {
                        sb.Append(term);
                    }
                    Console.WriteLine(sb);
                    return p;
                })*/
                .Select(EvalPostfix)
                .Sum();
        }

        private record Term(int Number, char Operation, bool IsNumber) // Weird, but I'm lazy
        {
            public override string ToString()
            {
                return IsNumber ? Number.ToString() : Operation.ToString();
            }
        }

        private static long EvalPostfix(List<Term> rpn)
        {
            var operandsStack = new Stack<long>();

            foreach (var (number, operation, isNumber) in rpn)
            {
                if (isNumber)
                {
                    operandsStack.Push(number);
                }
                else
                {
                    var right = operandsStack.Pop();
                    var left = operandsStack.Pop();

                    var result = operation switch
                    {
                        '*' => left * right,
                        '+' => left + right
                    };
                    
                    operandsStack.Push(result);
                }
            }
            
            return operandsStack.Pop();
        }
        
        private static List<Term> ReadRPN(ReadOnlySpan<char> infix, Dictionary<char, int> precedence)
        {
            var postfix = new List<Term>();
            var operationsStack = new Stack<char>();
            
            var numberStart = 0;
            var isParsingNumber = false;

            for (var i = 0; i < infix.Length; i++)
            {
                var c = infix[i];
                if (char.IsDigit(c))
                {
                    if (isParsingNumber) continue;

                    // begin to parse number
                    isParsingNumber = true;
                    numberStart = i;
                }
                else
                {
                    // Handle operator character
                    
                    if (isParsingNumber)
                    {
                        // First finish parsing number
                        var number = int.Parse(infix.Slice(numberStart, i - numberStart));
                        postfix.Add(new Term(number, default, true));
                        isParsingNumber = false;
                    }

                    // Second check which operator is it
                    switch (c)
                    {
                        case Operations.Add or Operations.Mul:
                            while (operationsStack.TryPeek(out var topOp) &&
                                   topOp != '(' &&
                                   precedence[topOp] >= precedence[c])
                            {
                                postfix.Add(new Term(default, operationsStack.Pop(), false));
                            }
                            operationsStack.Push(c);
                            continue;
                        case '(':
                            operationsStack.Push(c);
                            continue;
                        case ')':
                            while (operationsStack.Peek() != '(')
                            {
                                postfix.Add(new Term(default, operationsStack.Pop(), false));
                            }

                            operationsStack.Pop(); // remove parenthesis
                            continue;
                    }
                }
            }
            
            // handle last terms
            if (isParsingNumber)
                postfix.Add(new Term(int.Parse(infix.Slice(numberStart, infix.Length - numberStart)), default, true));

            while (operationsStack.TryPop(out var op))
            {
                postfix.Add(new Term(default, op, false));
            }

            return postfix;
        }
    }
}