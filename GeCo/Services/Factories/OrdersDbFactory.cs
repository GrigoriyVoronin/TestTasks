using GeCoTest.Db;

namespace GeCoTest.Services.Factories
{
    public class OrdersDbFactory
    {
        public OrdersDbContext CreateDbContext()
        {
            return new OrdersDbContext();
        }
    }
}