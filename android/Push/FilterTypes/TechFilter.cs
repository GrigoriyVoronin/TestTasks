using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class TechFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            return systemInfo.OsVersion > ((ITech)push).OsVersion;
        }
    }
}
