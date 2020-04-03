using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    interface ITime
    {
        long ExpiryDate { get; set; }
    }
}
