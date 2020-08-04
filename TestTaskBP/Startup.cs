using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTaskBP.Data;

namespace TestTaskBP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<UrlDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("UrlDataContext")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRewriter();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=URLs}/{action=Index}"));
        }
    }
}