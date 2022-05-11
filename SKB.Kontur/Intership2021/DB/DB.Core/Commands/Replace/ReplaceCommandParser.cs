using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public class ReplaceCommandParser : IReplaceCommandParser
    {
        public (bool Ok, string CollectionName, string Id, JObject Document, bool Upsert) Parse(JObject parameters)
        {
            if (parameters.Count != 2)
                return default;

            var upsertProperty = parameters.Properties().First(x => x.Name == "upsert");
            if (upsertProperty.Count != 1 || upsertProperty.Value.Type != JTokenType.String)
                return default;

            var upsertStringValue = upsertProperty.Value.ToString();
            if (!(upsertStringValue == "true" || upsertStringValue == "false"))
                return default;

            var upsert = upsertProperty.First().ToObject<bool>();
            var collectionProperty = parameters.Properties().First(x => x.Name != "upsert");
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1 || collectionProperty.First() is not JObject idAndDocument)
                return default;

            var idProperty = idAndDocument.Properties().First();
            var id = idProperty.Name;

            if (idProperty.Count != 1 || idProperty.First() is not JObject document)
                return default;

            return (true, collectionName, id, document, upsert);
        }
    }
}