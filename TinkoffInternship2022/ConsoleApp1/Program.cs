using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
    [Flags]
    public enum Status
    {
        None = 0b000_000_0,
        FirstShould = 0b000_001_0,
        SecondShould = 0b000_010_0,
        FirstProbably = 0b000_100_0,
        SecondProbably = 0b001_000_0,
        Both = FirstProbably | SecondProbably
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            //Task1.Run();
            //Task2.Run();
            Task3.Run();
            //Task4.Run();
            //Task5.Run();
            //Task6.Run();
            //Run();    
        }

        public static void Run()
        {
            var input = Console.ReadLine();
            var t1 = Console.ReadLine();
            var t2 = Console.ReadLine();
            var s = new Stopwatch();
            s.Start();
            var lettersVariants1 = FindLettersVariants(input, t1);
            var lettersVariants2 = FindLettersVariants(input, t2);
            var compareMatrix = CompareLettersIntersections(lettersVariants1, lettersVariants2, input.Length);
            var min = int.MaxValue;
            for (int i = 0; i < lettersVariants1.Count; i++)
            {
                for (int j = 0; j < lettersVariants2.Count; j++)
                {

                    min = Math.Min(compareMatrix[i, j], min);
                    if (compareMatrix[i, j] == min)
                    {
                        Console.WriteLine(string.Join("", lettersVariants1[i]));
                        Console.WriteLine(string.Join("", lettersVariants2[j]));
                    }
                }
            }
            Console.WriteLine(s.Elapsed.TotalSeconds);
            Console.WriteLine(min);
        }

        private static int[,] CompareLettersIntersections(List<List<int>> lettersVariants1, List<List<int>> lettersVariants2, int len)
        {
            var statuses = new Status[len];

            foreach (var list in lettersVariants1)
            {
                if (list.Count == 1)
                {
                    statuses[list[0]] |= Status.FirstShould;
                }
                else
                {
                    foreach (var item in list)
                    {
                        statuses[item] |= Status.FirstProbably;
                    }
                }
            }

            foreach (var list in lettersVariants2)
            {
                if (list.Count == 1)
                {
                    statuses[list[0]] |= Status.SecondShould;
                }
                else
                {
                    foreach (var item in list)
                    {
                        statuses[item] |= Status.SecondProbably;
                    }
                }
            }

            Console.WriteLine(string.Join(", ", statuses));

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
        

        private static List<List<int>> FindLettersVariants(string input, string t1)
        {
            return t1
                .Select(t => new List<int>(FindAllIndexes(input, t)))
                .ToList();
        }

        private static IEnumerable<int> FindAllIndexes(string input, char ch)
        {
            var index = 0;
            while (true)
            {
                index = input.IndexOf(ch, index);
                if (index == -1)
                {
                    yield break;
                }

                yield return index++;
            }
        }
    }
}