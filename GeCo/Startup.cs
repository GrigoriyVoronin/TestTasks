using GeCoTest.Db;
using GeCoTest.Repositories;
using GeCoTest.Services;
using GeCoTest.Services.Converters;
using GeCoTest.Services.ErrorHandlers;
using GeCoTest.Services.Factories;
using GeCoTest.Services.Loggers;
using GeCoTest.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GeCoTest
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
            services.AddControllers();
            services.AddDbContext<OrdersDbContext>();
            services.AddScoped<OrdersRepository>();
            services.AddSingleton<OrdersDbFactory>();
            services.AddSingleton<OrdersRepositoryFactory>();
            services.AddSingleton<ILogService>(x => new FileLogService("log.txt"));
            services.AddSingleton<IErrorHandler>(x => new ErrorHandler(x.GetService<ILogService>()));
            services.AddSingleton<ProductsConverter>();
            services.AddSingleton<OrdersConverter>();
            services.AddOrdersHandlerExecutor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrdersHandlerExecutor executor)
        {
            executor.StartWork();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}