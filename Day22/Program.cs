using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var input = Parse(File.ReadAllText(fileName));

            var result = part switch
            {
                1 => SolvePart1(input),
                2 => SolvePart2(input)
            };
            
            Console.WriteLine($"Part {part}: {result}");
        }

        private enum Winner
        {
            First,
            Second
        }
        
        private static int SolvePart2(Input input)
        {
            var (winner, score) = RecursiveCombat(input.Player1, input.Player2, 1);

            return score;

            (Winner winner, int score) RecursiveCombat(IEnumerable<int> p1, IEnumerable<int> p2, int game)
            {
                var decks = new HashSet<string>();
                Log($"=== Game {game} ===\n");
                var round = 0;
                
                var p1Queue = new Queue<int>(p1);
                var p2Queue = new Queue<int>(p2);
                while (p1Queue.Any() && p2Queue.Any())
                {
                    round++;
                    Log($"-- Round {round} (Game {game}) --");
                    Log($"Player 1's deck: {string.Join(", ", p1Queue)}");
                    Log($"Player 2's deck: {string.Join(", ", p2Queue)}");
                    /*
                     * Before either player deals a card, if there was a previous round in
                     * this game that had exactly the same cards in the same order in the
                     * same players' decks, the game instantly ends in a win for player 1.
                     * Previous rounds from other games are not considered. (This prevents
                     * infinite games of Recursive Combat, which everyone agrees is a bad
                     * idea.)
                     */
                    if (!TrySaveDecks(decks, p1Queue, p2Queue))
                    {
                        Log($"Those decks have been before! Player 1 considered winner!");
                        return (Winner.First, 0);
                    }
                    
                    var p1Card = p1Queue.Dequeue();
                    var p2Card = p2Queue.Dequeue();
                    
                    Log($"Player 1 plays: {p1Card}");
                    Log($"Player 2 plays: {p2Card}");

                    /*
                     * If both players have at least as many cards remaining in their deck
                     * as the value of the card they just drew, the winner of the round is
                     * determined by playing a new game of Recursive Combat (see below).
                     */
                    if (p1Queue.Count >= p1Card && p2Queue.Count >= p2Card)
                    {
                        Log("Playing a sub-game to determine the winner...\n");
                        var w = RecursiveCombat(p1Queue.Take(p1Card), p2Queue.Take(p2Card), game + 1).winner;
                        Log($"\n...anyway, back to game {game}.");
                        switch (w)
                        {
                            case Winner.First:
                                Log($"Player 1 wins round {round} of game {game}!\n");
                                p1Queue.Enqueue(p1Card);
                                p1Queue.Enqueue(p2Card);
                                continue;
                            case Winner.Second:
                                Log($"Player 2 wins round {round} of game {game}!\n");
                                p2Queue.Enqueue(p2Card);
                                p2Queue.Enqueue(p1Card);
                                continue;
                        }
                    }
                    else if (p1Card > p2Card)
                    {
                        Log($"Player 1 wins round {round} of game {game}!\n");
                        p1Queue.Enqueue(p1Card);
                        p1Queue.Enqueue(p2Card);
                    }
                    else if (p2Card > p1Card)
                    {
                        Log($"Player 2 wins round {round} of game {game}!\n");
                        p2Queue.Enqueue(p2Card);
                        p2Queue.Enqueue(p1Card);
                    }
                }

                if (game == 1)
                {
                    Log("== Post-game results ==");
                    Log($"Player 1's deck: {string.Join(", ", p1Queue)}");
                    Log($"Player 2's deck: {string.Join(", ", p2Queue)}");
                }
                
                return p1Queue.Any()
                    ? (Winner.First, game == 1 ? CountScore(p1Queue) : 0)
                    : (Winner.Second, game == 1 ? CountScore(p2Queue) : 0);
            }

            static bool TrySaveDecks(HashSet<string> decks, IEnumerable<int> p1Deck, IEnumerable<int> p2Deck)
            {
                var serializedDeck = string.Join(',', p1Deck) + "_" + string.Join(',', p2Deck);

                if (decks.Contains(serializedDeck))
                    return false;

                decks.Add(serializedDeck);
                return true;
            }
                
            static int CountScore(IEnumerable<int> cards)
            {
                var k = 1;
                return cards.Reverse().Sum(card => card * k++);
            }
        }

        
        private static int SolvePart1(Input input)
        {
            var player1Queue = new Queue<int>(input.Player1);
            var player2Queue = new Queue<int>(input.Player2);

            while (player1Queue.Any() && player2Queue.Any())
            {
                var p1Card = player1Queue.Dequeue();
                var p2Card = player2Queue.Dequeue();

                if (p1Card > p2Card)
                {
                    player1Queue.Enqueue(p1Card);
                    player1Queue.Enqueue(p2Card);
                }
                else if (p2Card > p1Card)
                {
                    player2Queue.Enqueue(p2Card);
                    player2Queue.Enqueue(p1Card);
                }
            }

            return CountScore(player1Queue.Any() ? player1Queue.Reverse() : player2Queue.Reverse());

            static int CountScore(IEnumerable<int> cards)
            {
                int k = 1;
                return cards.Sum(card => card * k++);
            }
        }

        private record Input(List<int> Player1, List<int> Player2);

        private static Input Parse(string text)
        {
            var cards = text.Split(new[] {"\r\n\r\n", "\n\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Regex.Replace(s, "Player [12]", ""))
                .Select(p => Regex.Matches(p, @"\d+"))
                .Select(m => m.Select(i => int.Parse(i.Value)).ToList())
                .ToArray();

            return new Input(cards[0], cards[1]);
        }
        
        private static void Log(string text)
        {
#if DEBUG
            Console.WriteLine(text);
#endif
        }
    }
}