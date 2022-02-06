using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Task5
    {
        public static void Run()
        {
            var input = Console.ReadLine();
            var t1 = Console.ReadLine();
            var t2 = Console.ReadLine();
            var lettersVariants1 = FindWordsVariants(input, t1);
            var lettersVariants2 = FindWordsVariants(input, t2);
            var compareMatrix = CompareLettersIntersections(lettersVariants1, lettersVariants2);
            var min = int.MaxValue;
            for (int i = 0; i < lettersVariants1.Count; i++)
            {
                for (int j = 0; j < lettersVariants2.Count; j++)
                {

                    min = Math.Min(compareMatrix[i,j], min);
                    if (compareMatrix[i, j] == min)
                    {
                        Console.WriteLine(string.Join("", lettersVariants1[i]));
                        Console.WriteLine(string.Join("", lettersVariants2[j]));
                    }
                }
            }
            Console.WriteLine(min);
        }

        private static int[,] CompareLettersIntersections(List<HashSet<int>> lettersVariants1, List<HashSet<int>> lettersVariants2)
        {
            var matrix = new int[lettersVariants1.Count, lettersVariants2.Count];
            for (int i = 0; i < lettersVariants1.Count; i++)
            {
                var set1 = lettersVariants1[i];
                for (int j = 0; j < lettersVariants2.Count; j++)
                {
                    var counter = 0;
                    var set2 = lettersVariants2[j];
                    foreach (var item in set1)
                    {
                        if (set2.Contains(item))
                        {
                            counter = int.MaxValue;
                            break;
                        }
                        if (set2.Contains(item - 1))
                        {
                            counter++;
                        }

                        if (set2.Contains(item + 1))
                        {
                            counter++;
                        }
                    }
                    matrix[i, j] = counter;
                }
            }

            return matrix;
        }

        private static List<HashSet<int>> FindWordsVariants(string input, string t1)
        {
            var variants = new List<HashSet<int>>();
            FindWordsVariants(variants, input, t1, 0, 0, new HashSet<int>());
            return variants;
        }

        private static void FindWordsVariants(List<HashSet<int>> variants, string input, string t1, int inputIndex, int t1Index, HashSet<int> word)
        {
            var index = inputIndex;
            for (var i = t1Index; i < t1.Length; i++)
            {
                index = input.IndexOf(t1[i], index);
                if (index == -1)
                {
                    break;
                }
                FindWordsVariants(variants, input, t1, index + 1, i, new HashSet<int>(word));
                word.Add(index);
            }

            if (word.Count == t1.Length)
            {
                variants.Add(word);
            }
        }
    }
}