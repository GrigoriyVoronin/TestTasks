using System.Collections.Generic;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Update
{
    public class UpdateCommand : ICommand
    {
        private readonly IUpdateCommandParser parser;

        public UpdateCommand(IUpdateCommandParser parser)
        {
            this.parser = parser;
        }

        public string Name => "update";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, updateDefinitions) = parser.Parse(parameters);
            if (!ok)
                return Result.Error.InvalidRequest;

            var collection = state.GetOrCreateCollection(collectionName);
            if (!collection.Items.ContainsKey(id))
                return Result.Error.NotFound;

            var item = collection.Items[id];
            ExecuteOperations(item, updateDefinitions);
            collection.AddOrUpdateItem(item);
            return Result.Ok.Empty;
        }

        private static void ExecuteOperations(DbItem item, Dictionary<string, string> setUpdateDefinitions)
        {
            foreach (var updateDefinition in setUpdateDefinitions)
                if (updateDefinition.Value != null)
                    item.Properties[updateDefinition.Key] = updateDefinition.Value;
                else
                    item.Properties.TryRemove(updateDefinition.Key, out _);
        }
    }
}