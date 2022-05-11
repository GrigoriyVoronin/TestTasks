using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IRestoreCommandParser parser;

        public RestoreCommand(IRestoreCommandParser parser)
        {
            this.parser = parser;
        }

        public string Name => "restore";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, dbState) = parser.Parse(parameters);
            if (!ok)
                return Result.Error.InvalidRequest;

            state.Collections.Clear();
            foreach (var collectionsWithNames in dbState.Collections)
                state.Collections[collectionsWithNames.Key] = collectionsWithNames.Value;
            return Result.Ok.Empty;
        }
    }
}