using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public class FindByIdCommandExecutor : IFindCommandExecutor
    {
        public bool CanExecute(JToken parameters)
        {
            return parameters.Type == JTokenType.String;
        }

        public JObject Execute(IDbState state, string collectionName, JToken parameters)
        {
            var id = parameters.ToString();
            if (!state.Collections.TryGetValue(collectionName, out var collection))
                return Result.Error.NotFound;

            return collection.Items.ContainsKey(id)
                ? Result.Ok.WithContent(collection.Items[id].Serialize())
                : Result.Error.NotFound;
        }
    }
}