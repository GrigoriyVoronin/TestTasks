namespace PushV1
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Filters;
    using Filters.FilterTypes;
    using Pushes;
    #endregion

    public static class Program
    {
        public static void Main()
        {
            var systemData = ParseSystemData();
            var pushes = ParsePushes();
            var filters = InitFilters(systemData);
            var filteringPushes = Filter
                .FiltratePushes(pushes, filters)
                .ToList();
            PrintPushes(filteringPushes);
        }

        private static void PrintPushes(ICollection<Push> filteringPushes)
        {
            if (filteringPushes.Count == 0)
                Console.WriteLine("-1");
            else
                foreach (var push in filteringPushes)
                    Console.WriteLine(push.Text.Value);
        }

        private static List<Filter> InitFilters(SystemData systemData)
        {
            return new List<Filter>
            {
                new AgeFilter(systemData),
                new GenderFilter(systemData),
                new LocationFilter(systemData),
                new TechFilter(systemData),
                new TimeFilter(systemData)
            };
        }

        private static IEnumerable<Push> ParsePushes()
        {
            var pushCount = int.Parse(Console.ReadLine());
            for (var i = 0; i < pushCount; i++)
                yield return ParsePush();
        }

        private static Push ParsePush()
        {
            var inputLength = int.Parse(Console.ReadLine());
            var input = GetInputLines(inputLength);
            return Push.Parse<Push>(input);
        }

        private static SystemData ParseSystemData()
        {
            var input = GetInputLines(6);
            return SystemData.Parse(input);
        }

        private static IEnumerable<string> GetInputLines(int count)
        {
            for (var i = 0; i < count; i++)
                yield return Console.ReadLine();
        }
    }
}