using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    class GenderFilter : Filter
    {
        public override bool IsPushFiltered(SystemInfo systemInfo, Push push)
        {
            return ((IGender)push).Gender != systemInfo.Gender;
        }
    }
}
