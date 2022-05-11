using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB.Client.Core;
using DB.Client.Core.Helpers;
using NUnit.Framework;

namespace DB.Tests.Unit
{
    [TestFixture]
    public class DbTests
    {
        [SetUp]
        public void SetUp()
        {
            var server = new TestServer();
            var application = new Application.Core.Application(server);
            application.Init();

            sender = new TestDbRequestSender(server);
            dbClient = new DbClient(sender);
        }

        private IDbRequestSender sender;
        private IDbClient dbClient;

        [TestCase("{}")]
        [TestCase("[]")]
        [TestCase("{}}")]
        [TestCase("\"do cool\"")]
        [TestCase("{\"insert\"}")]
        public async Task AnyRequest_ReturnsError_OnInvalidRequest(string command)
        {
            var result = await sender.SendAsync(command).ConfigureAwait(false);

            Assert.That(result, Does.Contain("error"));
            Assert.That(result, Does.Contain(Errors.InvalidRequest));
        }

        [TestCase(@"{""doCool"": {}}")]
        public async Task UnknownCommand_ReturnsError(string command)
        {
            var result = await sender.SendAsync(command).ConfigureAwait(false);

            Assert.That(result, Does.Contain("\"error\""));
            Assert.That(result, Does.Contain(Errors.CommandNotFound));
        }

        [TestCase("{\"insert\":{}}")]
        [TestCase("{\"insert\":{\"cars\":\"tesla\"}}")]
        [TestCase("{\"insert\":{\"cars\":{\"1\":\"tesla\"}}}")]
        [TestCase("{\"replace\":{}}")]
        [TestCase("{\"replace\":{\"cars\":\"tesla\"}}")]
        [TestCase("{\"replace\":{\"cars\":{\"1\":\"tesla\"}}}")]
        [TestCase("{\"find\":{}}")]
        [TestCase("{\"delete\":{}}")]
        public async Task AnyCommand_ReturnsError_WithWrongParameters(string command)
        {
            var result = await sender.SendAsync(command).ConfigureAwait(false);

            Assert.That(result, Does.Contain("\"error\""));
            Assert.That(result, Does.Contain(Errors.InvalidRequest));
        }

        [Test]
        public void Insert_NonExistingId_Ok()
        {
            Task Act()
            {
                return dbClient.GetCollection("cars")
                    .InsertAsync("1", new Dictionary<string, string> {["brand"] = "Tesla"});
            }

            Assert.That(Act, Throws.Nothing);
        }

        [Test]
        public async Task Insert_ExistingId_ReturnsError()
        {
            Task Act()
            {
                return dbClient.GetCollection("cars")
                    .InsertAsync("1", new Dictionary<string, string> {["brand"] = "Tesla"});
            }

            await Act().ConfigureAwait(false);
            Assert.That(Act,
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.AlreadyExists));
        }

        [Test]
        public void Find_NonExistingId_ReturnsError()
        {
            Task Act()
            {
                return dbClient.GetCollection("cars").FindAsync("1");
            }

            Assert.That(Act,
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.NotFound));
        }

        [Test]
        public async Task Find_ExistingId_Ok()
        {
            var collection = dbClient.GetCollection("cars");
            var expected = new Dictionary<string, string> {["brand"] = "Tesla"};
            await collection.InsertAsync("1", expected).ConfigureAwait(false);

            var actual = await collection.FindAsync("1").ConfigureAwait(false);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task FindMany_NonExistingDocuments_Ok()
        {
            var collection = dbClient.GetCollection("cars");

            var actual = await collection.FindAsync("brand", "Tesla").ConfigureAwait(false);

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public async Task FindMany_ExistingDocuments_Ok()
        {
            var collection = dbClient.GetCollection("cars");
            var modelS = new Dictionary<string, string> {["brand"] = "Tesla", ["model"] = "ModelS"};
            var roadster = new Dictionary<string, string> {["brand"] = "Tesla", ["model"] = "Roadster"};
            await collection.InsertAsync("1", modelS).ConfigureAwait(false);
            await collection.InsertAsync("2", roadster).ConfigureAwait(false);

            var actual = await collection.FindAsync("brand", "Tesla").ConfigureAwait(false);

            Assert.That(actual, Is.EquivalentTo(new[] {("1", modelS), ("2", roadster)}));
        }

        [Test]
        public void Replace_NotUpsertNonExistingId_ReturnsError()
        {
            Task Act()
            {
                return dbClient.GetCollection("cars")
                    .ReplaceAsync("1", new Dictionary<string, string> {["brand"] = "Tesla"});
            }

            Assert.That(Act,
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.NotFound));
        }

        [Test]
        public async Task Replace_UpsertNonExistingId_Ok()
        {
            var expected = new Dictionary<string, string> {["brand"] = "Tesla"};
            var collection = dbClient.GetCollection("cars");

            Task Act()
            {
                return collection.ReplaceAsync("1", expected, true);
            }

            Assert.That(Act, Throws.Nothing);
            Assert.That(await collection.FindAsync("1").ConfigureAwait(false), Is.EqualTo(expected));
        }

        [Test]
        public async Task Replace_ExistingId_Ok()
        {
            var collection = dbClient.GetCollection("cars");
            await collection.InsertAsync("1", new Dictionary<string, string> {["brand"] = "Tesla"})
                .ConfigureAwait(false);
            var expected = new Dictionary<string, string> {["brand"] = "Tesla", ["model"] = "ModelS"};

            await collection.ReplaceAsync("1", expected).ConfigureAwait(false);

            Assert.That(await collection.FindAsync("1").ConfigureAwait(false), Is.EqualTo(expected));
        }

        [Test]
        public void Delete_NonExistingId_ReturnsError()
        {
            Task Act()
            {
                return dbClient.GetCollection("cars").DeleteAsync("1");
            }

            Assert.That(Act,
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.NotFound));
        }

        [Test]
        public async Task Delete_ExistingId_Ok()
        {
            var collection = dbClient.GetCollection("cars");
            var expected = new Dictionary<string, string> {["brand"] = "Tesla"};
            await collection.InsertAsync("1", expected).ConfigureAwait(false);

            await collection.DeleteAsync("1").ConfigureAwait(false);

            Assert.That(() => collection.FindAsync("1"),
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.NotFound));
        }

        [Test]
        public async Task BackupDb()
        {
            var collection = dbClient.GetCollection("cars");
            var expected = new Dictionary<string, string>
            {
                ["brand"] = "Tesla"
            };
            await collection.InsertAsync("1", expected).ConfigureAwait(false);
            var backup = await dbClient.BackupAsync();
            Assert.AreEqual("{\r\n" +
                            "  \"cars\": [\r\n" +
                            "    {\r\n" +
                            "      \"1\": {\r\n" +
                            "        \"brand\": \"Tesla\"\r\n" +
                            "      }\r\n" +
                            "    }\r\n" +
                            "  ]\r\n" +
                            "}",
                backup);
        }


        [Test]
        public async Task BackupEmptyDb()
        {
            var backup = await dbClient.BackupAsync();
            Assert.AreEqual("{}", backup);
        }

        [Test]
        public async Task RestoreDb()
        {
            var collection = dbClient.GetCollection("cars");
            var property = new Dictionary<string, string> {["brand"] = "Tesla"};
            await collection.InsertAsync("1", property).ConfigureAwait(false);
            var backup = await dbClient.BackupAsync();
            await collection.DeleteAsync("1");
            await collection.InsertAsync("2", property).ConfigureAwait(false);
            await collection.InsertAsync("3", property).ConfigureAwait(false);
            await dbClient.RestoreAsync(backup);
            backup = await dbClient.BackupAsync();
            Assert.AreEqual("{\r\n" +
                            "  \"cars\": [\r\n" +
                            "    {\r\n" +
                            "      \"1\": {\r\n" +
                            "        \"brand\": \"Tesla\"\r\n" +
                            "      }\r\n" +
                            "    }\r\n" +
                            "  ]\r\n" +
                            "}",
                backup);
        }

        [Test]
        public async Task RestoreEmptyDb()
        {
            var collection = dbClient.GetCollection("cars");
            var property = new Dictionary<string, string> {["brand"] = "Tesla"};
            var backup = await dbClient.BackupAsync();
            await collection.InsertAsync("2", property).ConfigureAwait(false);
            await collection.InsertAsync("3", property).ConfigureAwait(false);
            await dbClient.RestoreAsync(backup);
            backup = await dbClient.BackupAsync();
            Assert.AreEqual("{}", backup);
        }

        [Test]
        public async Task UpdateExistingEntity()
        {
            var collection = dbClient.GetCollection("cars");
            var property = new Dictionary<string, string> {["brand"] = "Tesla"};
            await collection.InsertAsync("1", property).ConfigureAwait(false);
            await collection.UpdateAsync("1", Update.Set("Price", "100").Unset("brand"));
            var item = await collection.FindAsync("Price", "100");
            Assert.AreEqual("1", item.First().Id);
            var items = await collection.FindAsync("brand", "Tesla");
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void UpdateNotExistingEntity()
        {
            var collection = dbClient.GetCollection("cars");
            Assert.That(() => collection.UpdateAsync("1", Update.Set("Price", "100")),
                Throws.TypeOf<DbCommandException>().And.Property(nameof(DbCommandException.DbError))
                    .EqualTo(Errors.NotFound));
        }

        [Test]
        public async Task AddIndex()
        {
            var collection = dbClient.GetCollection("cars");
            await collection.InsertAsync("1", new Dictionary<string, string> {["model"] = "2"});
            var x1 = await collection.FindAsync("model", "2");
            Assert.AreEqual(x1.Single().Id, "1");
            await collection.Indexes.AddAsync("model");
            var x2 = await collection.FindAsync("model", "2");
            Assert.AreEqual(x2.Single().Id, "1");
        }
    }
}