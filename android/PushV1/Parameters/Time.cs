using System;

namespace PushV1.Parameters
{
    public class Time : Parameter<long>
    {
        private const string TimeName = "time";
        private static readonly Func<string, long> TimeParseFunc = long.Parse;

        public Time(string value)
            : base(TimeName, TimeParseFunc, value)
        {
        }
    }
}