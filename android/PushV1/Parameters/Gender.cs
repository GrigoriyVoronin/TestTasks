using System;

namespace PushV1.Parameters
{
    public class Gender : Parameter<GenderType>
    {
        private static readonly Func<string, GenderType> GenderParseFunc = x => x switch
        {
            "f" => GenderType.Female,
            "m" => GenderType.Male
        };

        private const string GenderName = "gender";

        public Gender(string value)
            : base(GenderName, GenderParseFunc, value)
        {
        }
    }
}