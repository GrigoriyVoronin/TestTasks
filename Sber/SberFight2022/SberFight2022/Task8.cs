using System.Collections.Generic;
using System.Linq;

namespace SberFight2022
{
    public class Task8
    {
        public static int GetPipePrice(int left, int top, int right, int bot)
        {
            if (left == 0 && right == 1 && top == 0 && bot == 1)
            {
                return 17;
            }
            if (left == 1 && right == 1 && top == 0 && bot == 1)
            {
                return 32;
            }
            if (left == 1 && right == 0 && top == 0 && bot == 1)
            {
                return 10;
            }
            if (left == 0 && right == 1 && top == 1 && bot == 1)
            {
                return 40;
            }
            if (left == 1 && right == 1 && top == 1 && bot == 1)
            {
                return 63;
            }
            if (left == 1 && right == 0 && top == 1 && bot == 1)
            {
                return 31;
            }
            if (left == 0 && right == 1 && top == 1 && bot == 0)
            {
                return 15;
            }
            if (left == 1 && right == 1 && top == 1 && bot == 0)
            {
                return 29;
            }
            if (left == 1 && right == 0 && top == 1 && bot == 0)
            {
                return 13;
            }
            if (left == 1 && right == 1 && top == 0 && bot == 0)
            {
                return 21;
            }
            if (left == 0 && right == 0 && top == 1 && bot == 1)
            {
                return 20;
            }

            return 0;
        }

        public static int GetResult(List<string> scheme)
        {
            if (scheme.Count == 0)
            {
                return 0;
            }

            var lenJ = scheme.First().Split('-').Length;
            var lenI = scheme.Count;
            var matrix = new int [lenI, lenJ];
            for (int i = 0; i < lenI; i++)
            {
                var lineArr = scheme[i].Split('-');
                for (int j = 0; j < lineArr.Length; j++)
                {
                    matrix[i, j] = int.Parse(lineArr[j]);
                }
            }

            var sum = 0;
            for (int i = 0; i < lenI; i++)
            {
                for (int j = 0; j < lenJ; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        continue;
                    }
                    var left = j == 0 ? 1 : matrix[i, j - 1];
                    var right = j == lenJ - 1 ? 1 : matrix[i, j + 1];
                    var bot = i == lenI - 1 ? 1 : matrix[i + 1, j];
                    var top = i == 0 ? 1 : matrix[i - 1, j];
                    var p = GetPipePrice(left, top, right, bot);
                    sum += p;
                }
            }

            return sum;
        }
        /*
            "0-0-1-0-0-0-0-0",
            "0-0-1-0-1-1-1-0",
            "0-0-1-1-1-0-1-0",
            "0-0-0-0-0-0-1-0",
            "0-0-0-0-0-0-1-0",
            "0-0-0-0-1-1-1-0",
            "0-0-0-0-1-0-0-0"
         */
    }
}