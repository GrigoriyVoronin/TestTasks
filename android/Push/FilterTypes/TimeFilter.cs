using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class TimeFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            return ((ITime)push).ExpiryDate < systemInfo.Time;
        }
    }
}
