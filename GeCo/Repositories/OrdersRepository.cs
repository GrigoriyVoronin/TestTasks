using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeCoTest.Db;
using GeCoTest.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace GeCoTest.Repositories
{
    public class OrdersRepository : IDisposable, IAsyncDisposable
    {
        private readonly OrdersDbContext _dbContext;

        public OrdersRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask DisposeAsync()
        {
            return _dbContext?.DisposeAsync() ?? new ValueTask();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<DbOrder> AddNewOrder(DbOrder dbOrder)
        {
            await _dbContext.AddAsync(dbOrder);
            await _dbContext.SaveChangesAsync();
            return dbOrder;
        }

        public async Task<List<DbOrder>> GetNewOrders()
        {
            return await _dbContext.DbOrders
                .Where(x => x.IsHandled == false)
                .ToListAsync();
        }

        public async Task SaveHandledOrders(List<DbOrder> handledOrders)
        {
            _dbContext.UpdateRange(handledOrders);
            await _dbContext.SaveChangesAsync();
        }
    }
}