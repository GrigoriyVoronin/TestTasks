using GeCoTest.OrdersHandlers;
using GeCoTest.Services;
using GeCoTest.Services.Converters;
using GeCoTest.Services.ErrorHandlers;
using GeCoTest.Services.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace GeCoTest.Utils
{
    public static class OrderHandlerRegistrar
    {
        public static IServiceCollection AddOrdersHandlerExecutor(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton(x =>
            {
                var ordersHandlers = new IOrderHandler[]
                {
                    new TalabatHandler(x.GetService<OrdersConverter>()),
                    new ZomatoHandler(x.GetService<OrdersConverter>()),
                    new UberHandler()
                };
                return new OrdersHandlerExecutor(ordersHandlers,
                    x.GetService<OrdersRepositoryFactory>(), x.GetService<IErrorHandler>());
            });
        }
    }
}