using System;
using System.Linq;

namespace ConsoleApp1
{
    public class Task1
    {
        public static void Run()
        {
            var input = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();
            var prepareWeight = input[1];
            var coinWeight = input[2];
            var coinsInPrepare = prepareWeight / coinWeight;
            var prepareElseWeight = prepareWeight % coinWeight;
            var currentWeight = input[0];
            var coinsCount = 0;

            while (currentWeight >= prepareWeight)
            {
                var preparesCount = currentWeight / prepareWeight;
                var currentCoinsCount = preparesCount * coinsInPrepare;
                coinsCount += currentCoinsCount;
                currentWeight += -preparesCount * prepareWeight + prepareElseWeight * preparesCount;
                if (currentCoinsCount == 0)
                {
                    break;
                }
            }
            Console.WriteLine(coinsCount);
        }
    }
}