using System.Collections.Generic;

namespace OrganizationApi.Models
{
    public class Organization
    {
        public Organization(int stampsCount, List<Department> departments)
        {
            StampsCount = stampsCount;
            Departments = departments;
            Departments
                .Sort((a, b) => a.Number
                    .CompareTo(b.Number));
        }

        internal Organization()
        {
            Departments = new List<Department>();
        }

        internal List<Department> Departments { get; }
        internal int StampsCount { get; set; }

        internal Department GetDepartmentByNumber(int number)
        {
            return Departments[number - 1];
        }
    }
}