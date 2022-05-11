using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Delete
{
    public class DeleteCommand : ICommand
    {
        public string Name => "delete";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
                return Result.Error.InvalidRequest;

            var property = parameters.Properties().First();
            var collectionName = property.Name;

            if (property.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            var id = property.Value.ToString();
            if (!state.Collections.TryGetValue(collectionName, out var collection)
                || !collection.Items.ContainsKey(id))
                return Result.Error.NotFound;

            collection.TryRemoveItem(id, out _);
            return Result.Ok.Empty;
        }
    }
}