using System.Collections.Generic;
using System.Reflection;

namespace Push
{
    public class SystemInfo
    {
        public int ParametersCount =>
            GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
            .Length - 1;
        private Dictionary<string, object> _parameters



        public long Time { get; }
        public int Age { get; }
        public string Gender { get; }
        public int OsVersion { get; }
        public float XCoard { get; }
        public float YCoard { get; }

        public SystemInfo(long time, int age, string gender, int osVersion, float xCoard, float yCoard)
        {
            Time = time;
            Age = age;
            Gender = gender;
            OsVersion = osVersion;
            XCoard = xCoard;
            YCoard = yCoard;
        }

        public static SystemInfo Parse()
        {

        }
    }
}
