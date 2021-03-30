using System;
using System.Threading.Tasks;
using GeCoTest.Models.DB;

namespace GeCoTest.OrdersHandlers
{
    public class UberHandler : IOrderHandler
    {
        public Task<DbOrder> HandleOrder(DbOrder order)
        {
            throw new ApplicationException("Uber exception");
        }

        public string SystemType => "uber";
    }
}