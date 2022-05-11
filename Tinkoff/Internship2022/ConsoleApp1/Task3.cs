using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace ConsoleApp1
{
    public class Task3
    {
        private static int counter;

        private static Dictionary<int, HashSet<Point>> Ways;

        private static Dictionary<Point, int> WaysPair;

        public static void Run()
        {
            counter = 0;
            var target = int.Parse(Console.ReadLine());
            FindVariants(target);
            Process0_0(new int[3, 3]);
            Console.WriteLine(counter);
            Console.WriteLine(Ways.Sum(x => x.Value.Count) + WaysPair.Count);
        }

        private static void Process0_0(int[,] matrix)
        {
            foreach (var (value, points) in Ways)
            {
                matrix[0, 0] = value;
                foreach (var point in points)
                {
                    matrix[1, 0] = point.X;
                    matrix[2, 0] = point.Y;
                    Process0_1(matrix, points);
                }
            }
        }

        private static void Process0_1(int[,] matrix, IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                matrix[0, 1] = point.X;
                matrix[0, 2] = point.Y;
                Process1_1(matrix, Ways[matrix[1, 0]], point.X, point.Y);
            }
        }

        private static void Process1_1(int[,] matrix, IEnumerable<Point> points, int first, int second)
        {
            foreach (var point in points)
            {
                var firstPoint = new Point(first, point.X);
                var secondPoint = new Point(second, point.Y);
                if (WaysPair.ContainsKey(firstPoint) && WaysPair.ContainsKey(secondPoint))
                {
                    matrix[1, 1] = point.X;
                    matrix[1, 2] = point.Y;
                    Process2_1(matrix, Ways[matrix[2, 0]], firstPoint, secondPoint);
                }
            }
        }

        private static void Process2_1(int[,] matrix, IEnumerable<Point> points, Point firstPoint, Point secondPoint)
        {
            foreach (var point in points)
            {
                if (WaysPair.ContainsKey(firstPoint)
                    && WaysPair.ContainsKey(secondPoint)
                    && WaysPair[firstPoint] == point.X
                    && WaysPair[secondPoint] == point.Y)
                {
                    matrix[2, 1] = point.X;
                    matrix[2, 2] = point.Y;
                    counter++;
                }
            }
        }

        private static void FindVariants(int target)
        {
            Ways = new Dictionary<int, HashSet<Point>>();
            WaysPair = new Dictionary<Point, int>();
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                    for (var k = 0; k < 10; k++)
                    {
                        if (i + j + k == target)
                        {
                            if (!Ways.ContainsKey(i))
                            {
                                Ways[i] = new HashSet<Point>();
                            }
                            Ways[i].Add(new Point(j, k));
                            WaysPair[new Point(i, j)] = k;
                        }
                    }
        }
    }
}