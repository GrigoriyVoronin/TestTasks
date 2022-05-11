using System;
using System.Collections.Generic;
using System.Text;

namespace Push
{
    interface ILocation
    {
        float XCoard { get; set; }

        float YCoard { get; set; }

        int Radius { get; set; }
    }
}
