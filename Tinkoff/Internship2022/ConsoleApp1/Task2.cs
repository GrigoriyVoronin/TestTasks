using System;
using System.Linq;

namespace ConsoleApp1
{
    public class Task2
    {
        private const int GlassesCount = 10;

        public static void Run()
        {
            var glassesVolume = new decimal[GlassesCount];
            for (int i = 0; i < GlassesCount; i++)
            {
                glassesVolume[i] = decimal.Parse(Console.ReadLine());
            }

            var targetVolume = glassesVolume.Sum() / GlassesCount;
            var counter = 0;
            for (int i = 0; i < GlassesCount; i++)
            {
                if (glassesVolume[i] <= targetVolume)
                {
                    continue;
                }

                counter++;
                ProcessGlass(glassesVolume, i, targetVolume);
            }

            Console.WriteLine(counter);

        }


        private static void ProcessGlass(decimal[] glassesVolume, int index, decimal targetVolume)
        {
            var elseVolume = glassesVolume[index] - targetVolume;
            glassesVolume[index] = targetVolume;
            for (int j = 0; j < GlassesCount; j++)
            {
                if (glassesVolume[j] < targetVolume)
                {
                    var diff = Math.Min(targetVolume - glassesVolume[j], elseVolume);
                    glassesVolume[j] += diff;
                    elseVolume -= diff;
                }

                if (elseVolume == 0)
                {
                    break;
                }
            }
        }
    }
}