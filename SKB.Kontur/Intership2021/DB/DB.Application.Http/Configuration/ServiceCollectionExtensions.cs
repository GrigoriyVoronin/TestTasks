using DB.Application.Http.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace DB.Application.Http.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            ILog log, ITracer tracer, Server server)
        {
            return services
                .AddSingleton(typeof(ILog), log)
                .AddSingleton(typeof(ITracer), tracer)
                .AddSingleton(typeof(Server), server);
        }
    }
}