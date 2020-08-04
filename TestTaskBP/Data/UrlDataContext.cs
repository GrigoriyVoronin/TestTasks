using Microsoft.EntityFrameworkCore;
using TestTaskBP.Models;

namespace TestTaskBP.Data
{
    public sealed class UrlDataContext : DbContext
    {
        public UrlDataContext(DbContextOptions<UrlDataContext> options) :
            base(options)
        {
        }

        public DbSet<URL> Urls { get; set; }
    }
}