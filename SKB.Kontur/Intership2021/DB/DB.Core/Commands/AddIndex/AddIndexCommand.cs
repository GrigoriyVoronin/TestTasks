using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.AddIndex
{
    public class AddIndexCommand : ICommand
    {
        public string Name => "addIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1 || parameters.First is not JProperty property ||
                property.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            var collection = state.GetOrCreateCollection(property.Name);
            var fieldName = property.Value.ToString();
            if (collection.Indexes.ContainsKey(fieldName))
                return Result.Error.AlreadyExists;

            var index = collection.Indexes[fieldName] = new DbIndex(fieldName);
            IndexExistingItems(index, collection);
            return Result.Ok.Empty;
        }

        private static void IndexExistingItems(DbIndex index, DbItemsCollection collection)
        {
            foreach (var dbItemWithName in collection.Items)
                index.IndexItem(dbItemWithName.Value);
        }
    }
}