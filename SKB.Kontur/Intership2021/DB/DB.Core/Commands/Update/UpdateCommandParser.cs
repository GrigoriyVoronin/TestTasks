using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Update
{
    public class UpdateCommandParser : IUpdateCommandParser
    {
        public (bool Ok, string CollectionName, string Id, Dictionary<string, string> UpdateDefinitions) Parse(
            JObject parameters)
        {
            if (parameters.Count != 1 || parameters.First is not JProperty collectionProperty)
                return default;

            var collectionName = collectionProperty.Name;
            if (collectionProperty.First is not JObject entity || entity.Count != 1)
                return default;

            var entityProperty = entity.Properties().First();
            var id = entityProperty.Name;
            if (entityProperty.Value is not JArray updateDefinitions)
                return default;

            try
            {
                var parsedUpdateDefinitions = ParseUpdateDefinitions(updateDefinitions);
                return (true, collectionName, id, parsedUpdateDefinitions);
            }
            catch
            {
                return default;
            }
        }

        private static Dictionary<string, string> ParseUpdateDefinitions(JArray updateDefinitions)
        {
            var parsedUpdateDefinitions = new Dictionary<string, string>();
            foreach (var updateOperation in updateDefinitions
                .Values<JObject>()
                .Select(updateDefinition => updateDefinition.Properties().Single()))
                switch (updateOperation.Name)
                {
                    case "set":
                        var fieldValue = updateOperation.Value.Values<JProperty>().Single();
                        parsedUpdateDefinitions[fieldValue.Name] = fieldValue.Value.ToObject<string>();
                        break;
                    case "unset":
                        var field = updateOperation.Value.ToObject<string>();
                        parsedUpdateDefinitions[field] = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown operation name: {updateOperation.Name}");
                }

            return parsedUpdateDefinitions;
        }
    }
}