using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.DropIndex
{
    public class DropIndexCommand : ICommand
    {
        public string Name => "dropIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1 || parameters.First is not JProperty property ||
                property.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            if (!state.Collections.ContainsKey(property.Name))
                return Result.Error.NotFound;

            var collection = state.GetOrCreateCollection(property.Name);
            var fieldName = property.Value.ToString();
            if (!collection.Indexes.ContainsKey(fieldName))
                return Result.Error.NotFound;

            collection.Indexes.TryRemove(fieldName, out _);
            return Result.Ok.Empty;
        }
    }
}