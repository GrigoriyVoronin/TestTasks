using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public class InsertCommandParser : IInsertCommandParser
    {
        public (bool Ok, string CollectionName, string Id, JObject Document) Parse(JObject parameters)
        {
            if (parameters.Count != 1)
                return default;

            var collectionProperty = parameters.Properties().First();
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1 || collectionProperty.First() is not JObject idAndDocument)
                return default;

            var idProperty = idAndDocument.Properties().First();
            var id = idProperty.Name;

            return idProperty.Count != 1 || idProperty.First() is not JObject document
                ? default
                : (true, collectionName, id, document);
        }
    }
}