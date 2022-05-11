using System.Collections.Generic;

namespace SberFight2022
{
    public class Task6
    {
        public static int GetResult(List<int> weight)
        {
            weight.Sort();
            var freeP = new SortedSet<int>();
            for (int i = 0; i < weight.Count; i++)
            {
                freeP.Add(i);
            }

            var carCount = 0;
            while (freeP.Count > 0)
            {
                CollectCar(weight, freeP);
                carCount++;
            }

            return carCount;
        }

        private static void CollectCar(List<int> weights, SortedSet<int> freeP)
        {
            var max = freeP.Max;
            freeP.Remove(max);
            int wSum = 0;
            var indexes = new List<int>();
            foreach (var index in freeP)
            {
                if (indexes.Count < 3)
                {
                    if (wSum + weights[index] <= 210)
                    {
                        wSum += weights[index];
                        indexes.Add(index);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (wSum - weights[indexes[0]] + weights[index] <= 210)
                    {
                        indexes.RemoveAt(0);
                        indexes.Add(index);
                        wSum = wSum - weights[indexes[0]] + weights[index];
                    }
                    else
                    {
                        break;
                    }
                }
            }
            foreach (var index in indexes)
            {
                freeP.Remove(index);
            }
        }
    }
}