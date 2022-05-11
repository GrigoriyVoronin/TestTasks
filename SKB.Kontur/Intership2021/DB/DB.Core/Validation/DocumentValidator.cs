using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Validation
{
    public class DocumentValidator : IDocumentValidator
    {
        public bool IsValid(JObject document)
        {
            return document.Properties().All(x => x.Value.Type == JTokenType.String);
        }
    }
}