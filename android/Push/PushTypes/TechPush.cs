using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class TechPush : Push, ITech
    {
        public int OsVersion { get; set; }

        public TechPush(string text, string type,int osVersion) : base(text, type)
        {
            OsVersion = osVersion;
        }
    }
}
