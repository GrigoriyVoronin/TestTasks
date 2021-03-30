using System;
using System.Linq;
using System.Text.Json;
using GeCoTest.Models.API;
using GeCoTest.Models.API.Requests;
using GeCoTest.Models.DB;

namespace GeCoTest.Services.Converters
{
    public sealed class OrdersConverter
    {
        private readonly ProductsConverter _productsConverter;

        public OrdersConverter(ProductsConverter productsConverter)
        {
            _productsConverter = productsConverter;
        }

        public T DeserializeJsonSource<T>(string source)
        {
            return JsonSerializer.Deserialize<T>(source, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public DbOrder CreateDbModel(string sourceOrder, string systemType)
        {
            var addOrderRequest = DeserializeJsonSource<AddOrderRequest>(sourceOrder);
            return new DbOrder
            {
                CreatedAt = DateTime.UtcNow,
                OrderNumber = addOrderRequest.OrderNumber,
                SourceOrder = sourceOrder,
                SystemType = systemType
            };
        }

        public Order ConvertFromDbModel(DbOrder dbOrder)
        {
            var orderRequest = DeserializeJsonSource<AddOrderRequest>(dbOrder.SourceOrder);
            var products = orderRequest.Products
                .Select(x => _productsConverter.ConvertFromRequest(x))
                .ToArray();
            return new Order
            {
                OrderNumber = dbOrder.OrderNumber,
                SystemType = dbOrder.SystemType,
                CreatedAt = dbOrder.CreatedAt,
                Products = products,
                Id = dbOrder.Id
            };
        }
    }
}