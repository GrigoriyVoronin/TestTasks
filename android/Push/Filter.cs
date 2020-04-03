using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    abstract class Filter
    {
        public static List<Push> Filtrate(Push[] pushArr, SystemInfo systemInfo)
        {
            var filteredPushs = new List<Push>(pushArr.Length);
            foreach (var push in pushArr)
            {
                if (!DefineFiltersAndFiltrate(push, systemInfo))
                    filteredPushs.Add(push);
            }
            return filteredPushs;
        }

        public abstract bool IsPushFiltered(SystemInfo systemInfo, Push push);

        private static bool DefineFiltersAndFiltrate(Push push, SystemInfo systemInfo)
        {
            switch (push.PushType)
            {
                case "LocationPush":
                    return FiltratePush(systemInfo, push, new LocationFilter(), new TimeFilter());
                case "AgeSpecificPush":
                    return FiltratePush(systemInfo, push, new AgeFilter(), new TimeFilter());
                case "TechPush":
                    return FiltratePush(systemInfo, push, new TechFilter());
                case "LocationAgePush":
                    return FiltratePush(systemInfo, push, new LocationFilter(), new AgeFilter());
                case "GenderAgePush":
                    return FiltratePush(systemInfo, push, new GenderFilter(), new AgeFilter());
                case "GenderPush":
                    return FiltratePush(systemInfo, push, new GenderFilter());
                default:
                    throw new Exception("Unknown type of push");
            }
        }

        private static bool FiltratePush(SystemInfo systemInfo, Push push, params Filter[] filters)
        {
            var isFiltered = false;
            foreach(var filter in filters)
                isFiltered |= filter.IsPushFiltered(systemInfo, push);
            return isFiltered;
        }
    }
}
