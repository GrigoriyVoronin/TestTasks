using DbModels;
using Microsoft.EntityFrameworkCore;

namespace DbRepositories
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}