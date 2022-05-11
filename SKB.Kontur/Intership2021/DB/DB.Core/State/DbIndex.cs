using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DB.Core.State
{
    public class DbIndex
    {
        public DbIndex(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
        public ConcurrentDictionary<string, HashSet<string>> ItemsIdsByValue { get; } = new();

        public void IndexItem(DbItem item)
        {
            if (!item.Properties.ContainsKey(FieldName))
                return;

            var itemsIds = ItemsIdsByValue
                .GetOrAdd(item.Properties[FieldName], _ => new HashSet<string>());
            itemsIds.Add(item.Id);
        }

        public void RemoveItem(DbItem item)
        {
            if (!item.Properties.ContainsKey(FieldName))
                return;

            var propertyValue = item.Properties[FieldName];
            if (!ItemsIdsByValue.ContainsKey(propertyValue))
                return;

            ItemsIdsByValue[propertyValue].Remove(item.Id);
        }
    }
}