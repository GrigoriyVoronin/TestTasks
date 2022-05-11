using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace _2GisTestTask
{
    [TestFixture]
    public class TableTest
    {
        [Test]
        public void ShouldAddElement()
        {
            var table = new Table<UserType, int, int>();
            var key = new CompositeKey<UserType, int>(new UserType(0), 0);
            table.Add(key, 0);
            Assert.IsTrue(table.ContainsKey(key));
            Assert.AreEqual(1, table.Count);
        }

        [Test]
        public void ShouldRemoveElementByKey()
        {
            var table = new Table<UserType, int, int>();
            var key = new CompositeKey<UserType, int>(new UserType(0), 0);
            table.Add(key, 0);
            table.Remove(key);
            Assert.AreEqual(0, table.Count);
            Assert.IsFalse(table.ContainsKey(key));
        }

        [Test]
        public void ShouldGetAllValuesById()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(0), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table[new UserType(0)].Count());
        }

        [Test]
        public void ShouldGetAllValuesByName()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(1), 0);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table[0].Count());
        }

        [Test]
        public void ShouldClearTable()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(0), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            table.Clear();
            Assert.AreEqual(0, table.Count);
        }

        [Test]
        public void ShouldGetValueByKey()
        {
            var table = new Table<UserType, int, int>();
            var key = new CompositeKey<UserType, int>(new UserType(0), 0);
            table.Add(key, 0);
            Assert.AreEqual(0, table[key]);
        }

        [Test]
        public void ShouldGetAllIds()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(1), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table.Ids.Count);
        }

        [Test]
        public void ShouldGetAllNames()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(1), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table.Names.Count);
        }

        [Test]
        public void ShouldGetAllKeys()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(1), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table.Keys.Count);
        }

        [Test]
        public void ShouldGetAllValues()
        {
            var table = new Table<UserType, int, int>();
            var key1 = new CompositeKey<UserType, int>(new UserType(0), 0);
            var key2 = new CompositeKey<UserType, int>(new UserType(1), 1);
            table.Add(key1, 0);
            table.Add(key2, 1);
            Assert.AreEqual(2, table.Values.Count);
        }

        [Test]
        public void ConcurrentTableShouldWorkInManyTreads()
        {
            var concurrentTable = new ConcurrentTable<UserType, int, int>();
            var rnd = new Random();
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() =>
            {
                for (var i = 0; i < 1_000_000; i++)
                    concurrentTable.Add(new CompositeKey<UserType, int>(new UserType(i), i + 1), rnd.Next());
            }));
            for (var i = 0; i < 10; i++)
                tasks.Add(Task.Run(() =>
                {
                    for (var j = 0; j < 1_000_000; j++)
                    {
                        concurrentTable[rnd.Next()].ToArray();
                        concurrentTable[new UserType(rnd.Next())].ToArray();
                    }
                }));

            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(100);
                concurrentTable.Clear();
            }));

            Task.WaitAll(tasks.ToArray());
        }
    }
}