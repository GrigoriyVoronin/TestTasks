using Artsofte.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Artsofte.Db
{
    public class StaffContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public StaffContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(_configuration.GetConnectionString("StaffDb"));
        }
    }
}