using GeCoTest.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace GeCoTest.Db
{
    public sealed class OrdersDbContext : DbContext
    {
        public DbSet<DbOrder> DbOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Orders");
        }
    }
}