using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OrganizationApi;
using OrganizationApi.Models;
using OrganizationApi.Models.Response;
using OrganizationApi.Models.Rules;

namespace TestOrganizationApi
{
    public class Tests
    {
        [Test]
        public async Task ShouldGoToEndWithSimpleRule()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 2))
                    .AddDepartment(Department.WithSimpleRule(1, (1, 2, 2)))
                    .AddDepartment(Department.WithSimpleRule(2, (2, 3, -1))));
            var response = await organizationClient.GetSheetsInDepartment(2);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Attended, new HashSet<int> {1, 2}));
        }

        [Test]
        public async Task ShouldGoToEndWithConditionRule()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 2))
                    .AddDepartment(Department.WithConditionRule(1, new ConditionRule(1, (1, 2, 2), (2, 5, 2))))
                    .AddDepartment(Department.WithConditionRule(2, new ConditionRule(2, (3, 2, -1), (3, 5, 5)))));
            var response = await organizationClient.GetSheetsInDepartment(2);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Attended, new HashSet<int> {3}));
        }

        [Test]
        public async Task ShouldGoToEndWithDifferentRules()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 2))
                    .AddDepartment(Department.WithConditionRule(1, new ConditionRule(1, (1, 2, 2), (2, 5, 2))))
                    .AddDepartment(Department.WithSimpleRule(2, (3, 2, -1))));
            var response = await organizationClient.GetSheetsInDepartment(2);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Attended, new HashSet<int> {3}));
        }

        [Test]
        public async Task ShouldStoppedWhenEndlessCycleExist()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 3))
                    .AddDepartment(Department.WithSimpleRule(1, (1, 2, 2)))
                    .AddDepartment(Department.WithSimpleRule(2, (2, 1, 1)))
                    .AddDepartment(Department.WithSimpleRule(3, (1, 2, -1))));
            var response = await organizationClient.GetSheetsInDepartment(2);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.EndlessCycle, new HashSet<int> {2}));
            response = await organizationClient.GetSheetsInDepartment(1);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.EndlessCycle, new HashSet<int> {1}));
            response = await organizationClient.GetSheetsInDepartment(3);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Unvisited));
        }

        [Test]
        public async Task ShouldReturnSameResultsForSameRequests()
        {
            var organizationClient = new OrganizationClient();
            await RandomOrgSetup(50, 100, organizationClient);
            var first = await organizationClient.GetSheetsInDepartment(51);
            var second = await organizationClient.GetSheetsInDepartment(51);
            Assert.IsTrue(IsExpectedResponse(first, second.RouteStatus,
                second.UniqueBypassSheets
                    .Select(x => x.Stamps)
                    .ToArray()));
        }

        [Test]
        public async Task ShouldReturnManySheetsForManyTimesAttendedDepartment()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 3))
                    .AddDepartment(Department.WithConditionRule(1, new ConditionRule(2, (1, 5, 3), (1, 5, 2))))
                    .AddDepartment(Department.WithSimpleRule(2, (2, 3, 1)))
                    .AddDepartment(Department.WithSimpleRule(3, (3, 4, -1))));
            var response = await organizationClient.GetSheetsInDepartment(1);
            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Attended, new HashSet<int> {1},
                new HashSet<int> {1, 2}));
        }

        [Test]
        public async Task ShouldWorkWithRandomNumbersAndParallelRequests()
        {
            var organizationClient = new OrganizationClient();
            const int stampsCount = 20_000;
            const int departmentsCount = 20_000;
            await RandomOrgSetup(stampsCount, departmentsCount, organizationClient);
            var tasks = new Task[departmentsCount];
            for (var t = 0; t < departmentsCount; t++)
            {
                var number = t;
                tasks[number] = Task.Run(() => { _ = organizationClient.GetSheetsInDepartment(number); });
            }

            Task.WaitAll(tasks);
        }

        [Test]
        public async Task ShouldReturnRightAnswersWithConcurrentResponses()
        {
            var organizationClient = new OrganizationClient();
            await organizationClient.SetupOrganization(builder =>
                builder
                    .SetupStampsCount(5)
                    .SetupPath((1, 3))
                    .AddDepartment(Department.WithSimpleRule(1, (1, 2, 2)))
                    .AddDepartment(Department.WithSimpleRule(2, (2, 1, 1)))
                    .AddDepartment(Department.WithSimpleRule(3, (1, 2, -1))));
            const int requestCount = 20_000;
            var tasks = new Task[requestCount];
            for (var i = 0; i < requestCount; i += 1)
            {
                var number = i % 3 + 1;
                tasks[i] = Task.Run(async () =>
                {
                    var response = await organizationClient.GetSheetsInDepartment(number);
                    switch (number)
                    {
                        case 1:
                            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.EndlessCycle, new HashSet<int> {1}));
                            break;
                        case 2:
                            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.EndlessCycle, new HashSet<int> {2}));
                            break;
                        case 3:
                            Assert.IsTrue(IsExpectedResponse(response, RouteStatus.Unvisited));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(number.ToString());
                    }
                });
            }

            Task.WaitAll(tasks);
        }

        [Test]
        public void ShouldWorkWithParallelRequestsWhileInitializing()
        {
            var organizationClient = new OrganizationClient();
            const int stampsCount = 20_000;
            const int departmentsCount = 20_000;

            var tasks = new List<Task>
            {
                Task.Run(() => RandomOrgSetup(stampsCount, departmentsCount, organizationClient))
            };

            for (var t = 0; t < departmentsCount; t++)
            {
                var number = t;
                tasks.Add(Task.Run(() => { _ = organizationClient.GetSheetsInDepartment(number); }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task RandomOrgSetup(int stampsCount, int departmentsCount,
            OrganizationClient organizationClient)
        {
            await organizationClient.SetupOrganization(builder =>
            {
                builder
                    .SetupStampsCount(stampsCount)
                    .SetupPath((1, 2));
                var sRandom = new Random();
                var dRandom = new Random();
                for (var i = 1; i <= departmentsCount; i++)
                    builder.AddDepartment(Department.WithSimpleRule(i,
                        (sRandom.Next(1, stampsCount), sRandom.Next(1, stampsCount),
                            dRandom.Next(1, departmentsCount))));
            });
        }

        private static bool IsExpectedResponse(BypassSheetInfoResponse response, RouteStatus expectedStatus,
            params HashSet<int>[] expectedSheets)
        {
            return response.RouteStatus == expectedStatus
                   && response.UniqueBypassSheets.Count == expectedSheets.Length
                   && !response.UniqueBypassSheets
                       .Where((t, i) => !AreEqualsSets(t.Stamps, expectedSheets[i]))
                       .Any();
        }

        private static bool AreEqualsSets(IEnumerable<int> first, IEnumerable<int> second)
        {
            return !first
                .Except(second)
                .Any();
        }
    }
}