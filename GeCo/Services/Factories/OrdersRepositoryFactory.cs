using GeCoTest.Repositories;

namespace GeCoTest.Services.Factories
{
    public class OrdersRepositoryFactory
    {
        private readonly OrdersDbFactory _dbFactory;

        public OrdersRepositoryFactory(OrdersDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public OrdersRepository CreateRepository()
        {
            return new OrdersRepository(_dbFactory.CreateDbContext());
        }
    }
}