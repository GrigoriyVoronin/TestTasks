using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public interface IRestoreCommandParser
    {
        (bool Ok, IDbState DbState) Parse(JObject parameters);
    }
}