using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day21
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var part = int.Parse(args[1]);

            var input = Parse(File.ReadLines(fileName));
            var result = part switch
            {
                1 => SolvePart1(input).ToString(),
                2 => SolvePart2(input)
            };
            
            Console.WriteLine($"Part {part}: {result}");
        }

        private static int SolvePart1(Input input)
        {
            var (ingredientsPerAllergen, ingredientsCount) = input;
            
            var allPossible = ingredientsPerAllergen
                .Aggregate(new HashSet<string>(), (all, pair) =>
                {
                    all.UnionWith(pair.Value);
                    return all;
                });

            return ingredientsCount
                .Where(pair => !allPossible.Contains(pair.Key))
                .Sum(pair => pair.Value);
        }
        
        private static string SolvePart2(Input input)
        {
            var (ingredientsPerAllergen, ingredientsCount) = input;

            var allergicIngredients = ingredientsPerAllergen
                .Aggregate(new HashSet<string>(), (all, pair) =>
                {
                    all.UnionWith(pair.Value);
                    return all;
                });

            var shouldBeChecked = ingredientsPerAllergen
                .Where(pair => pair.Value.Any(allergicIngredients.Contains))
                .ToDictionary(p => p.Key, p => p.Value);
            
            // ingredient - allergen
            var guessed = new List<(string ingredient, string allergen)>();

            var ingredientForRemoval = new string[1];
            while (shouldBeChecked.Any())
            {
                var (allergen, ingredients) = shouldBeChecked
                    .First(p => p.Value.Count == 1);
                guessed.Add((ingredients.Single(), allergen));
                shouldBeChecked.Remove(allergen);

                ingredientForRemoval[0] = ingredients.Single();
                foreach (var pair in shouldBeChecked)
                {
                    pair.Value.ExceptWith(ingredientForRemoval);
                }
            }

            return string.Join(',', guessed.OrderBy(tuple => tuple.allergen).Select(tuple => tuple.ingredient));
        }

        private record Input(Dictionary<string, HashSet<string>> IngredientsPerAllergen,
            Dictionary<string, int> IngredientsCount);
        
        private static Input Parse(IEnumerable<string> lines)
        {
            Dictionary<string, HashSet<string>> ingredientsPerAllergen = new();
            Dictionary<string, int> ingredientsCount = new();
            
            foreach (var line in lines)
            {
                var parts = line
                    .Split(" (contains ");

                var ingredients = parts[0]
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                var allergens = parts[1]
                    .Replace(")", "")
                    .Replace(",", "")
                    .Split(' ')
                    .ToArray();

                AggregateState(allergens, ingredients);
            }
            
            return new Input(ingredientsPerAllergen, ingredientsCount);

            void AggregateState(string[] allergens, string[] ingredients)
            {
                foreach (var allergen in allergens)
                {
                    if (ingredientsPerAllergen.TryGetValue(allergen, out var ing))
                    {
                        ing.IntersectWith(ingredients);
                    }
                    else
                    {
                        ingredientsPerAllergen[allergen] = ingredients.ToHashSet();
                    }
                }

                foreach (var ingredient in ingredients)
                {
                    if (ingredientsCount.ContainsKey(ingredient))
                    {
                        ingredientsCount[ingredient]++;
                    }
                    else
                    {
                        ingredientsCount[ingredient] = 1;
                    }
                }
            }
        }
    }
}