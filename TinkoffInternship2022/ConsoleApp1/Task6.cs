using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Task6
    {
        public static void Run()
        {
            var eventsCount = int.Parse(Console.ReadLine());
            var columnsCounters = new SortedDictionary<int, int>();
            var median = 1_000_000_000.5;
            var leftValue = 0;
            var rightValue = 0;
            for (int i = 0; i < eventsCount; i++)
            {
                var input = Console.ReadLine();
                var type = input[0];
                int[] coordinates;
                int xCoordinate;
                switch (type)
                {
                    case 'A':
                        coordinates = GetCoordinates(input);
                        xCoordinate = coordinates[0];
                        columnsCounters[xCoordinate] = columnsCounters.GetValueOrDefault(xCoordinate, 0) + 1;
                        if (xCoordinate < median)
                        {
                            leftValue++;
                        }
                        else
                        {
                            rightValue++;
                        }
                        break;
                    case 'D':
                        coordinates = GetCoordinates(input);
                        xCoordinate = coordinates[0];
                        columnsCounters[xCoordinate] = columnsCounters.GetValueOrDefault(xCoordinate, 0) - 1;
                        if (xCoordinate < median)
                        {
                            leftValue--;
                        }
                        else
                        {
                            rightValue--;
                        }
                        break;
                    case 'Q':
                        if (rightValue != leftValue)
                        {
                            var newMedian = TryFindMedian(columnsCounters, median, leftValue, rightValue, out int newValue);
                            if (newMedian != -1)
                            {
                                median = newMedian;
                                leftValue = newValue;
                                rightValue = newValue;
                            }
                            Console.WriteLine((int) newMedian);
                        }
                        else
                        {
                            Console.WriteLine((int) median);
                        }
                        break;
                }
            }
        }

        private static double TryFindMedian(SortedDictionary<int, int> columnsCounters, double median, int leftValue, int rightValue, out int newValue)
        {
            var currentMedian = median;
            if (leftValue < rightValue)
            {
                foreach (var column in columnsCounters.Keys.SkipWhile(col => col < median))
                {
                    var columnValue = columnsCounters[column];
                    if (leftValue + columnValue <= rightValue - columnValue)
                    {
                        currentMedian = column + 0.5;
                        leftValue+= columnValue;
                        rightValue-= columnValue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var column in columnsCounters.Keys.Reverse().SkipWhile(col => col > median))
                {
                    var columnValue = columnsCounters[column];
                    if (rightValue + columnValue <= leftValue - columnValue)
                    {
                        currentMedian = column - 0.5;
                        leftValue -= columnValue;
                        rightValue += columnValue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            newValue = leftValue;
            return leftValue == rightValue ? currentMedian : -1;
        }

        private static int[] GetCoordinates(string input)
        {
            return input[2..].Split().Select(int.Parse).ToArray();
        }
    }
}