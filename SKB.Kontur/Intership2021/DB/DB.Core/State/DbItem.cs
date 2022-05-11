using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.State
{
    public class DbItem
    {
        public DbItem(string id, ConcurrentDictionary<string, string> properties)
        {
            Id = id;
            Properties = properties;
        }

        public string Id { get; }
        public ConcurrentDictionary<string, string> Properties { get; }

        public JObject Serialize()
        {
            return new(new JProperty(Id,
                new JObject(Properties.Select(kvp =>
                    new JProperty(kvp.Key, kvp.Value)))));
        }
    }
}