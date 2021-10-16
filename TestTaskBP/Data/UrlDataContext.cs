using Microsoft.EntityFrameworkCore;
using VoroninTestTask.Models;

namespace VoroninTestTask.Data
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