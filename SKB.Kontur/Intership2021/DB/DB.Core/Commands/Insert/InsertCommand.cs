using System.Collections.Concurrent;
using DB.Core.Helpers;
using DB.Core.State;
using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public class InsertCommand : ICommand
    {
        private readonly IInsertCommandParser parser;
        private readonly IDocumentValidator validator;

        public InsertCommand(IInsertCommandParser parser, IDocumentValidator validator)
        {
            this.parser = parser;
            this.validator = validator;
        }

        public string Name => "insert";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, document) = parser.Parse(parameters);

            if (!ok)
                return Result.Error.InvalidRequest;

            var collection = state.GetOrCreateCollection(collectionName);
            if (collection.Items.ContainsKey(id))
                return Result.Error.AlreadyExists;

            if (!validator.IsValid(document))
                return Result.Error.InvalidRequest;

            var item = new DbItem(id, document.ToObject<ConcurrentDictionary<string, string>>());
            collection.AddOrUpdateItem(item);
            return Result.Ok.Empty;
        }
    }
}