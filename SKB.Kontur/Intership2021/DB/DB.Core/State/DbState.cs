using System.Collections.Concurrent;

namespace DB.Core.State
{
    public class DbState : IDbState
    {
        public ConcurrentDictionary<string, DbItemsCollection> Collections { get; } = new();

        public DbItemsCollection GetOrCreateCollection(string collectionName)
        {
            return Collections.GetOrAdd(collectionName, _ => new DbItemsCollection(collectionName));
        }
    }
}