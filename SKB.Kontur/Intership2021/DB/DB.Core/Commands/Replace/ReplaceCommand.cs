using System.Collections.Concurrent;
using DB.Core.Helpers;
using DB.Core.State;
using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public class ReplaceCommand : ICommand
    {
        private readonly IReplaceCommandParser parser;
        private readonly IDocumentValidator validator;

        public ReplaceCommand(IReplaceCommandParser parser, IDocumentValidator validator)
        {
            this.parser = parser;
            this.validator = validator;
        }

        public string Name => "replace";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, document, upsert) = parser.Parse(parameters);

            if (!ok)
                return Result.Error.InvalidRequest;

            var collection = state.GetOrCreateCollection(collectionName);

            if (!collection.Items.ContainsKey(id) && !upsert)
                return Result.Error.NotFound;

            if (!validator.IsValid(document))
                return Result.Error.InvalidRequest;

            var item = new DbItem(id, document.ToObject<ConcurrentDictionary<string, string>>());
            collection.AddOrUpdateItem(item);
            return Result.Ok.Empty;
        }
    }
}