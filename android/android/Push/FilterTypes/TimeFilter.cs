using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class TimeFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            return ((ITime)push).ExpiryDate < systemInfo.Time;
        }
    }
}
