using ABTestReal.Services;
using DbRepositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ABTestReal
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<UsersContext>(x =>
                x.UseNpgsql(_configuration.GetConnectionString("Db")));
            services.AddScoped<UsersRepository>();
            services.AddScoped<UsersService>();
            services.AddScoped<RetentionService>();
            services.AddOpenApiDocument(c =>
            {
                c.Title = "API";
                c.DocumentName = "v1";
                c.Version = "v1";
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
