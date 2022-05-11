using System.Collections.Concurrent;
using System.Linq;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommandParser : IRestoreCommandParser
    {
        public (bool Ok, IDbState DbState) Parse(JObject parameters)
        {
            var dbState = new DbState();
            try
            {
                foreach (var collection in parameters.Properties())
                    dbState.Collections[collection.Name] = ParseCollection(collection);
                return (true, dbState);
            }
            catch
            {
                return default;
            }
        }

        private static DbItemsCollection ParseCollection(JProperty collection)
        {
            var parsedCollection = new DbItemsCollection(collection.Name);
            var objectsCollection = collection.Value
                .Values<JObject>()
                .ToList();

            foreach (var item in objectsCollection
                .Select(x => x.Properties().First()))
                parsedCollection.Items[item.Name] = ParseItem(item);

            return parsedCollection;
        }

        private static DbItem ParseItem(JProperty item)
        {
            var propertiesCollection = item.Value.ToObject<ConcurrentDictionary<string, string>>();
            return new DbItem(item.Name, propertiesCollection);
        }
    }
}