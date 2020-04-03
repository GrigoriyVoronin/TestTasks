using System;
using System.Collections.Generic;
using System.Text;

namespace PushTask
{
    class SystemInfo
    {
        public const int ParametrsCount = 6;

        public long Time { get; private set; }

        public int Age { get; private set; }

        public string Gender { get; private set; }

        public int OsVersion { get; private set; }

        public float XCoard { get; private set; }

        public float YCoard { get; private set; }

        public SystemInfo(long time,int age, string gender,int osVersion,float xCoard, float yCoard)
        {
            Time = time;
            Age = age;
            Gender = gender;
            OsVersion = osVersion;
            XCoard = xCoard;
            YCoard = yCoard;
        }
    }
}
