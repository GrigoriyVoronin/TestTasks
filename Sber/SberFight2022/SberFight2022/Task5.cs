using System;
using System.Collections.Generic;

namespace SberFight2022
{
    public class Task5
    {
        public static int GetResult(List<int> treasures)
        {
            return (int) Math.Ceiling(Sum(treasures, 0d, false, 0));
        }

        private static double Sum(List<int> treasures, double currentSum, bool isPrevTake, int curentIndex)
        {
            if (curentIndex == treasures.Count)
            {
                return currentSum;
            }

            if (isPrevTake)
            {
                return Math.Max(
                    Sum(treasures, currentSum + treasures[curentIndex] / 2d, true, curentIndex + 1),
                    Sum(treasures, currentSum, false, curentIndex + 1));
            }
            else
            {
                return Math.Max(
                    Sum(treasures, currentSum + treasures[curentIndex], true, curentIndex + 1),
                    Sum(treasures, currentSum, false, curentIndex + 1));
            }
        }
    }
}