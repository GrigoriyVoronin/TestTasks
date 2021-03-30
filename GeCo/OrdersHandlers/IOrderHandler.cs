using System.Threading.Tasks;
using GeCoTest.Models.DB;

namespace GeCoTest.OrdersHandlers
{
    public interface IOrderHandler
    {
        public string SystemType { get; }
        public Task<DbOrder> HandleOrder(DbOrder order);
    }
}