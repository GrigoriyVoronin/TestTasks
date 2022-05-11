using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Task4
    {
        public static void Run()
        {
            var input = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();
            var partsCount = input[0];
            var partsLengths = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();
            var accumulatorLength = input[2];
            var maxUnionSize = input[1];

            var unions = new List<(int, int)>();
            var economy = 0;
            var position = 0;
            while (position < partsCount)
            {
                var currentUnionSize = 0;
                var currentLen = 0;
                var currentPosition = position;
                while (currentUnionSize < maxUnionSize
                       && currentPosition < partsCount
                       && currentLen + partsLengths[currentPosition] <= accumulatorLength)
                {
                    currentLen += partsLengths[currentPosition];
                    currentUnionSize++;
                    currentPosition++;
                }

                if (currentUnionSize > 1)
                {
                    economy += currentUnionSize - 1;
                    unions.Add((position, currentUnionSize));
                    position += currentUnionSize;
                }
                else
                {
                    position += 1;
                }
            }
            Console.WriteLine(economy);
            Console.WriteLine(unions.Count);
            foreach (var (positionIndex, length) in unions)
            {
                Console.WriteLine($"{positionIndex + 1} {length}");
            }
        }
    }
}