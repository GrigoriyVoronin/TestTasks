using System;
using System.Collections.Generic;

namespace SberFight2022
{
    public class Task4
    {
        public static bool GetResult(int calendar, int dateOfBirth, string name, List<string> phrases)
        {
            if (int.TryParse(phrases[0], out var res))
            {
                if (calendar - dateOfBirth == res || calendar - dateOfBirth - 1 == res)
                {
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            var nameV = phrases[1].ToLower();
            name = name.ToLower();

            if (Math.Abs(nameV.Length - name.Length) > 1)
            {
                return false;
            }

            var diffCount = LevenshteinDistance(name, nameV);
            return diffCount <= 1;

        }
        
        private static int LevenshteinDistance(string firstWord, string secondWord)
        {
            var n = firstWord.Length + 1;
            var m = secondWord.Length + 1;
            var matrixD = new int[n, m];

            const int deletionCost = 1;
            const int insertionCost = 1;

            for (var i = 0; i < n; i++)
            {
                matrixD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                matrixD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                    matrixD[i, j] = Math.Min(Math.Min(
                        matrixD[i - 1, j] + deletionCost,
                        matrixD[i, j - 1] + insertionCost),
                        matrixD[i - 1, j - 1] + substitutionCost);
                }
            }

            return matrixD[n - 1, m - 1];
        }

        public static void RunCode()
        {
            // Entrypoint to debug your function
        }
    }
}