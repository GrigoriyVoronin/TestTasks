using System;

namespace PushV1.Parameters
{
    public class OsVersion : Parameter<int>
    {
        private const string OsVersionName = "os_version";
        private static readonly Func<string, int> OsVersionParseFunc = int.Parse;
        public OsVersion(string value)
            : base(OsVersionName, OsVersionParseFunc, value)
        {
        }
    }
}