using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class LocationFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            var locationPush = (ILocation)push;
            return IsInRadius(systemInfo.XCoard, systemInfo.YCoard, locationPush.XCoard, locationPush.YCoard, locationPush.Radius);
        }

        private static bool IsInRadius (float xSystem, float ySystem, float xPush, float yPush, int radius)
        {
            return Math.Sqrt(Math.Pow(xSystem-xPush,2)+Math.Pow(ySystem-yPush,2)) > radius;
        }

    }
}
