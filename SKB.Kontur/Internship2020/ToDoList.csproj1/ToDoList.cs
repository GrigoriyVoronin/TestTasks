namespace ToDoList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ToDoList : IToDoList
    {
        private readonly Dictionary<int, SortedSet<ItemWithName>> entries = new Dictionary<int, SortedSet<ItemWithName>>();

        private readonly Dictionary<int, SortedSet<ItemWithState>> entriesStatus = new Dictionary<int, SortedSet<ItemWithState>>();

        private readonly HashSet<int> dismissUsers = new HashSet<int>();

        public int Count => TakeLastEntries().Count();

        public void AddEntry(int entryId, int userId, string name, long timestamp)
        {
            AddEntryItem(entryId, userId, name, timestamp, false);
        }

        public void RemoveEntry(int entryId, int userId, long timestamp)
        {
            AddEntryItem(entryId, userId, null, timestamp, true);
        }

        public void MarkDone(int entryId, int userId, long timestamp)
        {
            AddStateInfo(entryId, userId, timestamp, EntryState.Done);
        }

        public void MarkUndone(int entryId, int userId, long timestamp)
        {
            AddStateInfo(entryId, userId, timestamp, EntryState.Undone);
        }

        public void DismissUser(int userId)
        {
            dismissUsers.Add(userId);
        }

        public void AllowUser(int userId)
        {
            dismissUsers.Remove(userId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            foreach (var entry in TakeLastEntries())
                yield return entry;
        }

        private void AddEntryItem(int entryId, int userId, string name, long timestamp, bool hide)
        {
            if (!entries.ContainsKey(entryId))
                entries[entryId] = new SortedSet<ItemWithName>();
            entries[entryId].Add(new ItemWithName(userId, name, timestamp, hide));
        }

        private void AddStateInfo(int entryId, int userId, long timestamp, EntryState state)
        {
            if (!entriesStatus.ContainsKey(entryId))
                entriesStatus[entryId] = new SortedSet<ItemWithState>();
            entriesStatus[entryId].Add(new ItemWithState(userId, timestamp, state));
        }

        private IEnumerable<Entry> TakeLastEntries()
        {
            foreach (var pair in entries)
            {
                var item = TakeAvailableItem(pair.Value);
                if (!item?.Hide ?? false)
                    yield return new Entry(pair.Key, item.Name, TakeItemState(pair.Key));
            }
        }

        private EntryState TakeItemState(int entryId)
        {
            if (entriesStatus.ContainsKey(entryId))
                return TakeAvailableItem(entriesStatus[entryId])?.State ?? EntryState.Undone;
            return EntryState.Undone;
        }

        private T TakeAvailableItem<T>(SortedSet<T> entries)
           where T : EntryItem
        {
            return entries.LastOrDefault(x => !dismissUsers.Contains(x.UserId));
        }

        private class ItemWithName : EntryItem, IComparable<ItemWithName>
        {
            public ItemWithName(int userId, string name, long timestamp, bool hide)
                : base(userId, timestamp)
            {
                Name = name;
                Hide = hide;
            }

            public string Name { get; }

            public bool Hide { get; }

            public int CompareTo(ItemWithName other)
            {
                return (Timestamp, Hide, -UserId).CompareTo((other.Timestamp, other.Hide, -other.UserId));
            }
        }

        private class ItemWithState : EntryItem, IComparable<ItemWithState>
        {
            public ItemWithState(int userId, long timestamp, EntryState state)
                : base(userId, timestamp)
            {
                State = state;
            }

            public EntryState State { get; }

            public int CompareTo(ItemWithState other)
            {
                return (Timestamp, -(int)State).CompareTo((other.Timestamp, -(int)other.State));
            }
        }

        private abstract class EntryItem
        {
            public EntryItem(int userId, long timestamp)
            {
                UserId = userId;
                Timestamp = timestamp;
            }

            public int UserId { get; }

            public long Timestamp { get; }
        }
    }
}