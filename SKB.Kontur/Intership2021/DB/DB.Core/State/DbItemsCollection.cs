using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.State
{
    public class DbItemsCollection
    {
        public DbItemsCollection(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ConcurrentDictionary<string, DbItem> Items { get; } = new();
        public ConcurrentDictionary<string, DbIndex> Indexes { get; } = new();

        public JProperty Serialize()
        {
            return new(Name,
                new JArray(Items.Select(x => x.Value.Serialize())));
        }

        public void AddOrUpdateItem(DbItem item)
        {
            Items[item.Id] = item;
            foreach (var property in item.Properties
                .Where(x => Indexes.ContainsKey(x.Key)))
                Indexes[property.Key].IndexItem(item);
        }

        public bool TryRemoveItem(string itemId, out DbItem item)
        {
            item = null;
            if (!Items.TryRemove(itemId, out item))
                return false;

            foreach (var property in item.Properties
                .Where(x => Indexes.ContainsKey(x.Key)))
                Indexes[property.Key].RemoveItem(item);
            return true;
        }
    }
}