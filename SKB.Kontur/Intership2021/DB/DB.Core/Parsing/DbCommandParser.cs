using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Parsing
{
    public class DbCommandParser : IDbCommandParser
    {
        public (bool Ok, string CommandName, JObject Parameters) Parse(string input)
        {
            if (!TryParse(input, out var jObject) || jObject.Count != 1)
                return default;

            var commandProperty = jObject.Properties().First();
            return commandProperty.First is not JObject parameters
                ? default
                : (true, commandProperty.Name, parameters);
        }

        private static bool TryParse(string json, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(json);
                return true;
            }
            catch
            {
                jObject = null;
                return false;
            }
        }
    }
}