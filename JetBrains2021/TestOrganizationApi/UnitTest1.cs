using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OrganizationApi;
using OrganizationApi.Models;
using OrganizationApi.Models.Response;

namespace TestOrganizationApi
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var orgClient = new OrganizationApiClient();
            const int stampsCount = 2_000_000_000;
            const int departmentsCount = 2_000_000_000;
            orgClient.SetupOrganization(builder =>
            {
                builder
                    .SetupStampsCount(10)
                    .SetupPath((1, 2));
                var sRandom = new Random();
                var dRandom = new Random();
                for (int i = 1; i <= departmentsCount; i++)
                {
                    builder.AddDepartment(Department.WithSimpleRule(i,
                        (sRandom.Next(1, stampsCount), sRandom.Next(1, stampsCount),
                            dRandom.Next(1, departmentsCount))));
                }
            });
            var tasks = new List<Task>();
            for (int t = 0; t < departmentsCount; t++)
            {
                ThreadPool.QueueUserWorkItem(number =>
                {
                    var response = orgClient.GetSheetsInDepartment((int) number);
                }, t, false);
            }
        }

        private bool AreEquals(IEnumerable<int> first, IEnumerable<int> second)
        {
            return !first
                .Except(second)
                .Any();
        }
    }
}