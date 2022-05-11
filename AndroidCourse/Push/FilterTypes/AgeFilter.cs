using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class AgeFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            return ((IAge)push).Age > systemInfo.Age;
        }
    }
}
