using System;
using System.Collections.Generic;
using System.Linq;

namespace TinkoffCSharp2021
{
    internal class Program
    {
        private static void Main()
        {
            Task1();
            //Task2();
            //Task3();
            //Task4();
            //Task5();
            //Task6();
        }

        private static void Task6()
        {
            var input = ParseIntArrInput();
            var quantityOfSlices = input[0];
            var teamSize = input[1];
            var timeByPerson = ParseIntArrInput();
            var timeWhenPersonEndEat = timeByPerson.ToArray();
            var currentTime = 0;
            var slicesEaten = 0;
            while (slicesEaten < quantityOfSlices)
            {
                var currentEatEndTime = int.MaxValue;
                for (var i = 0; i < teamSize; i++)
                {
                    var currentEnd = timeWhenPersonEndEat[i];
                    if (currentTime >= currentEnd)
                    {
                        slicesEaten++;
                        timeWhenPersonEndEat[i] += 2 * timeByPerson[i];
                    }

                    if (currentEnd < currentEatEndTime)
                        currentEatEndTime = currentEnd;
                }

                currentTime = currentEatEndTime;
            }

            Console.WriteLine(currentTime);
        }

        private static void Task5()
        {
            var n = int.Parse(Console.ReadLine());
            var staff = ParseIntArrInput();
            var quantityOfPeopleWithMessage = new int[n];
            for (var i = 0; i < n; i++)
            {
                var nextMan = i;
                while (nextMan != -1)
                {
                    if (quantityOfPeopleWithMessage[nextMan] != 0)
                    {
                        quantityOfPeopleWithMessage[i] += quantityOfPeopleWithMessage[nextMan];
                        break;
                    }

                    quantityOfPeopleWithMessage[i]++;
                    nextMan = FinNextMan(staff, nextMan);
                }
            }

            Console.WriteLine(string.Join(" ", quantityOfPeopleWithMessage));
        }

        private static int FinNextMan(int[] staff, int currentMan)
        {
            var currentHeight = staff[currentMan];
            for (var i = currentMan + 1; i < staff.Length; i++)
                if (staff[i] >= currentHeight)
                    return i;

            return -1;
        }

        private static void Task4()
        {
            var input = ParseIntArrInput();
            var parcelsCount = input[0];
            var parcels = new int[parcelsCount];
            var boxSize = input[1];
            var boxCost = input[2];
            for (var i = 0; i < parcels.Length; i++)
                parcels[i] = int.Parse(Console.ReadLine());

            var boxesPrices = CalculateBoxPrices(parcels, boxCost, boxSize);
            var minPrice = FindMinWay(boxesPrices, 1, parcelsCount + 1, parcelsCount + 1);
            Console.WriteLine(minPrice);
        }

        private static long FindMinWay(Dictionary<int, Dictionary<int, long>> ways, int startPoint, int endPoint,
            int quantity)
        {
            var wayLengthToPoint = new long[quantity + 1]
                .Select(tag => long.MaxValue)
                .ToArray();
            var pointsVisited = new bool[quantity + 1];
            pointsVisited[0] = true;
            wayLengthToPoint[startPoint] = 0;
            while (!pointsVisited[endPoint])
            {
                var index = FindIndexOfPointWithMinLength(wayLengthToPoint, pointsVisited);
                if (ways.ContainsKey(index))
                    foreach (var town in ways[index])
                        wayLengthToPoint[town.Key] = Math.Min(wayLengthToPoint[town.Key],
                            town.Value + wayLengthToPoint[index]);
                pointsVisited[index] = true;
            }

            return wayLengthToPoint[endPoint];
        }

        private static int FindIndexOfPointWithMinLength(IReadOnlyList<long> tagsLength, IReadOnlyList<bool> tagsVisit)
        {
            var index = 0;
            var value = long.MaxValue;
            for (var i = 1; i < tagsVisit.Count; i++)
            {
                if (tagsVisit[i] || tagsLength[i] >= value)
                    continue;
                value = tagsLength[i];
                index = i;
            }

            return index;
        }


        private static Dictionary<int, Dictionary<int, long>> CalculateBoxPrices(int[] parcels, int boxCost,
            int boxSize)
        {
            var boxesPrices = new Dictionary<int, Dictionary<int, long>>();
            for (var i = 0; i < parcels.Length; i++)
            {
                boxesPrices[i + 1] = new Dictionary<int, long>();
                var upperBound = Math.Min(boxSize + i, parcels.Length);
                for (var j = i; j < upperBound; j++)
                    boxesPrices[i + 1][j + 2] = CalculateBoxSendPrice(boxCost, parcels, i, j);
            }

            return boxesPrices;
        }

        private static long CalculateBoxSendPrice(int boxCost, int[] parcels, int start, int end)
        {
            var currentMax = long.MinValue;
            var currentMin = long.MaxValue;
            var counter = 0L;
            for (var i = start; i <= end; i++)
            {
                var current = parcels[i];
                if (currentMax < current)
                    currentMax = current;
                if (currentMin > current)
                    currentMin = current;
                counter++;
            }

            return boxCost + counter * counter * (currentMax - currentMin);
        }

        private static void Task3()
        {
            var input = ParseIntArrInput();
            var hoursInDay = input[0];
            var minutesInHour = input[1];
            var secondsInMinute = input[2];
            var totalSeconds = input[3];
            var secondInWatch = totalSeconds % secondsInMinute;
            var totalMinutes = totalSeconds / secondsInMinute;
            var minutesInWatch = totalMinutes % minutesInHour;
            var totalHours = totalMinutes / minutesInHour;
            var hoursInWatch = totalHours % hoursInDay;
            Console.WriteLine($"{hoursInWatch} {minutesInWatch} {secondInWatch}");
        }

        private static void Task2()
        {
            var nick = Console.ReadLine();
            var vowels = new HashSet<char> {'a', 'e', 'i', 'o', 'u', 'y'};
            var isCurrentSymbolVowel = vowels.Contains(nick[0]);
            for (var i = 1; i < nick.Length; i++)
                if (isCurrentSymbolVowel & (isCurrentSymbolVowel = vowels.Contains(nick[i])))
                {
                    Console.WriteLine("NO");
                    return;
                }

            Console.WriteLine("YES");
        }

        private static void Task1()
        {
            var first = ParseIntArrInput();
            var second = ParseIntArrInput();
            for (var i = 0; i < 2; i++)
            {
                var index = Array.IndexOf(second, first[i]);
                switch (index)
                {
                    case 0:
                        Console.WriteLine(i == 0
                            ? $"{first[1]} {first[0]} {second[0]} {second[1]}"
                            : $"{first[0]} {first[1]} {second[0]} {second[1]}");
                        return;
                    case 1:
                        Console.WriteLine(i == 1
                            ? $"{first[0]} {first[1]} {second[1]} {second[0]}"
                            : $"{first[1]} {first[0]} {second[1]} {second[0]}");
                        return;
                }
            }

            Console.WriteLine(-1);
        }

        private static int[] ParseIntArrInput()
        {
            return Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();
        }
    }
}