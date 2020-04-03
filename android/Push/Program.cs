using System;
using System.Collections.Generic;

namespace PushTask
{
    class Program
    {
        static void Main()
        {
            var input = new InputIntorpretator(SystemInfo.ParametrsCount);
            var systemInfo = new SystemInfo(input.Time, input.Age, input.Gender, input.OsVersion,input.XCoard,input.YCoard);
            var pushsCount = int.Parse(Console.ReadLine());
            var pushArr = CreatePushs(pushsCount);
            var filteredPushs = Filter.Filtrate(pushArr, systemInfo);
            PrintPushs(filteredPushs);
        }

        private static void PrintPushs(List<Push> filteredPushs)
        {
            if (filteredPushs.Count == 0)
                Console.WriteLine("-1");
            foreach (var push in filteredPushs)
                Console.WriteLine(push.Text);
        }

        private static Push[] CreatePushs(int pushsCount)
        {
            var pushsArr = new Push[pushsCount];
            for (int i=0; i<pushsCount;i++)
            {
                var inputSize = int.Parse(Console.ReadLine());
                var input = new InputIntorpretator(inputSize);
                pushsArr[i] = Push.CreateNewPush(input.Type, input);
            }
            return pushsArr;
        }
    }
}
