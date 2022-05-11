using System.Collections.Concurrent;

namespace DB.Core.State
{
    public interface IDbState
    {
        public ConcurrentDictionary<string, DbItemsCollection> Collections { get; }
        DbItemsCollection GetOrCreateCollection(string collectionName);
    }
}