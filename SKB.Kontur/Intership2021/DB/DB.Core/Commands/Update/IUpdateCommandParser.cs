using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Update
{
    public interface IUpdateCommandParser
    {
        (bool Ok, string CollectionName, string Id, Dictionary<string, string> UpdateDefinitions) Parse(
            JObject parameters);
    }
}