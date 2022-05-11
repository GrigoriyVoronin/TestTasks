using System;
using System.Collections.Generic;
using System.Linq;

namespace SberFight2022
{
    class Task2
    {
        public static bool GetResult(List<string> time)
        {
            SortedDictionary<int, int> timeBounds = new SortedDictionary<int, int>();
            foreach (var timeItem in time)
            {
                var periodBounds = timeItem.Split('-')
                    .Select(int.Parse)
                    .ToArray();
                if (timeBounds.ContainsKey(periodBounds[0]))
                {
                    return false;
                }
                timeBounds[periodBounds[0]] = periodBounds[1];
            }

            int previousEnd = -1;
            foreach (var pair in timeBounds)
            {
                if (previousEnd > pair.Key)
                {
                    return false;
                }

                previousEnd = pair.Value;
            }

            return true;
        }

        public static void RunCode()
        {
            Console.WriteLine(GetResult(new List<string>() { "09-13", "12-14" }));
        }
    }
}