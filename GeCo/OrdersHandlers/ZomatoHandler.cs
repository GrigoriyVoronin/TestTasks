using System.Text.Json;
using System.Threading.Tasks;
using GeCoTest.Models.DB;
using GeCoTest.Services.Converters;

namespace GeCoTest.OrdersHandlers
{
    public class ZomatoHandler : IOrderHandler
    {
        private readonly OrdersConverter _ordersConverter;

        public ZomatoHandler(OrdersConverter ordersConverter)
        {
            _ordersConverter = ordersConverter;
        }

        public async Task<DbOrder> HandleOrder(DbOrder dbOrder)
        {
            var order = _ordersConverter.ConvertFromDbModel(dbOrder);
            foreach (var product in order.Products)
                product.UnitPrice = product.PaidPrice / product.Quantity;
            dbOrder.ConvertedOrder = JsonSerializer.Serialize(order);
            return dbOrder;
        }

        public string SystemType => "zomato";
    }
}