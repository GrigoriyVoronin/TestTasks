using System;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public class FindByFieldCommandExecutor : IFindCommandExecutor
    {
        public bool CanExecute(JToken parameters)
        {
            return parameters is JObject {Count: 1} jObject
                   && jObject.Properties().First().Value.Type == JTokenType.String;
        }

        public JObject Execute(IDbState state, string collectionName, JToken parameters)
        {
            var property = ((JObject) parameters).Properties().First();
            var field = property.Name;
            var value = property.Value.ToString();

            if (!state.Collections.TryGetValue(collectionName, out var collection))
                return Result.Ok.WithContent(Array.Empty<object>());

            var resultContent = FindItems(field, value, collection);
            return Result.Ok.WithContent(resultContent);
        }

        private static object FindItems(string field, string value, DbItemsCollection collection)
        {
            if (!collection.Indexes.ContainsKey(field))
                return SearchInItems(collection, field, value);

            return collection.Indexes[field].ItemsIdsByValue.ContainsKey(value)
                ? SearchInIndex(collection, field, value)
                : Array.Empty<object>();
        }

        private static object SearchInIndex(DbItemsCollection collection, string field, string value)
        {
            return collection.Indexes[field].ItemsIdsByValue[value]
                .Select(id => collection.Items[id].Serialize());
        }

        private static object SearchInItems(DbItemsCollection collection, string field, string value)
        {
            return collection.Items
                .Where(document => document.Value.Properties
                                       .TryGetValue(field, out var docValue)
                                   && docValue == value)
                .Select(dbItem => dbItem.Value.Serialize());
        }
    }
}