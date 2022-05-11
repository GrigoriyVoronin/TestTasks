using System;
using System.Linq;
using System.Collections.Generic;


namespace SberFight2022
{
    class Task3
    {
        public static bool GetResult(List<int> numb)
        {
            var numbersCounter = new Dictionary<int, int>();
            foreach (var n in numb)
            {
                if (!numbersCounter.ContainsKey(n))
                {
                    numbersCounter[n] = 0;
                }

                numbersCounter[n]++;
            }

            var counterWithLower2 = 0;
            foreach (var pair in numbersCounter)
            {
                if (pair.Value % 2 != 0)
                {
                    counterWithLower2++;
                }
            }

            return numb.Count % 2 == 0
                ? counterWithLower2 == 0
                : counterWithLower2 == 1;
        }

        public static void RunCode()
        {

        }
    }
}