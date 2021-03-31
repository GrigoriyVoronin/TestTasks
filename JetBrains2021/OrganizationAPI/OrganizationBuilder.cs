using OrganizationApi.Models;

namespace OrganizationApi
{
    public class OrganizationBuilder
    {
        public OrganizationBuilder()
        {
            Organization = new Organization();
        }

        internal Organization Organization { get; }
        internal Path Path { get; private set; }

        public OrganizationBuilder SetupStampsCount(int stampsCount)
        {
            Organization.StampsCount = stampsCount;
            return this;
        }

        public OrganizationBuilder SetupPath(Path path)
        {
            Path = path;
            return this;
        }

        public OrganizationBuilder AddDepartment(Department department)
        {
            Organization.Departments.Add(department);
            return this;
        }
    }
}