using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _2GisTestTask
{
    public class ConcurrentTable<TId, TName, TValue> : IEnumerable<TValue>
    {
        private readonly Dictionary<TId, HashSet<TName>> _idsNamesTable = new();
        private readonly ReaderWriterLockSlim _locker = new();
        private readonly Dictionary<TName, HashSet<TId>> _namesIdsTable = new();
        private readonly Dictionary<CompositeKey<TId, TName>, TValue> _table = new();

        public IEnumerable<TValue> this[TId id] =>
            ReadAction(() =>
            {
                if (_idsNamesTable.ContainsKey(id))
                    return _idsNamesTable[id]
                        .Select(name => _table[(id, name)]);
                return Array.Empty<TValue>();
            });

        public IEnumerable<TValue> this[TName name] =>
            ReadAction(() =>
            {
                if (_namesIdsTable.ContainsKey(name))
                    return _namesIdsTable[name]
                        .Select(id => _table[(id, name)]);
                return Array.Empty<TValue>();
            });

        public TValue this[CompositeKey<TId, TName> key] => ReadAction(() => _table[key]);

        public ICollection<TId> Ids => ReadAction(() => _idsNamesTable.Keys);
        public ICollection<TName> Names => ReadAction(() => _namesIdsTable.Keys);
        public ICollection<CompositeKey<TId, TName>> Keys => ReadAction(() => _table.Keys);
        public ICollection<TValue> Values => ReadAction(() => _table.Values);

        public int Count => ReadAction(() => _table.Count);

        public IEnumerator<TValue> GetEnumerator()
        {
            return ReadAction(() =>
                _idsNamesTable
                    .SelectMany(idNamesPair => idNamesPair.Value
                        .Select(name => _table[(idNamesPair.Key, name)]))
                    .GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(CompositeKey<TId, TName> key)
        {
            return ReadAction(() => _table.ContainsKey(key));
        }

        public void Add(CompositeKey<TId, TName> key, TValue value)
        {
            WriteAction(() =>
            {
                _table.Add(key, value);
                AddItemToCollection(key.Id, key.Name, _idsNamesTable);
                AddItemToCollection(key.Name, key.Id, _namesIdsTable);
            });
        }

        private static void AddItemToCollection<TKey, TItem>(TKey key, TItem item,
            IDictionary<TKey, HashSet<TItem>> collection)
        {
            if (!collection.ContainsKey(key))
                collection[key] = new HashSet<TItem>();
            collection[key].Add(item);
        }

        public void Remove(CompositeKey<TId, TName> key)
        {
            WriteAction(() =>
            {
                RemoveItemFromCollection(key.Id, key.Name, _idsNamesTable);
                RemoveItemFromCollection(key.Name, key.Id, _namesIdsTable);
                _table.Remove(key);
            });
        }

        private static void RemoveItemFromCollection<TKey, TItem>(TKey key, TItem item,
            IDictionary<TKey, HashSet<TItem>> collection)
        {
            if (!collection.ContainsKey(key))
                return;

            collection[key].Remove(item);
            if (!collection[key].Any())
                collection.Remove(key);
        }

        public void Clear()
        {
            WriteAction(() =>
            {
                _idsNamesTable.Clear();
                _namesIdsTable.Clear();
                _table.Clear();
            });
        }

        private T ReadAction<T>(Func<T> func)
        {
            _locker.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        private void WriteAction(Action action)
        {
            _locker.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }
    }
}