using System;

namespace PushV1.Parameters
{
    public class Age : Parameter<int>
    {
        private const string AgeName = "age";
        private static readonly Func<string, int> AgeParseFunc = int.Parse;

        public Age(string value)
            : base(AgeName, AgeParseFunc, value)
        {
        }
    }
}