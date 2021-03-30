using System.IO;
using System.Threading.Tasks;
using GeCoTest.Repositories;
using GeCoTest.Services;
using GeCoTest.Services.Converters;
using Microsoft.AspNetCore.Mvc;

namespace GeCoTest.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrdersConverter _ordersConverter;
        private readonly OrdersHandlerExecutor _ordersHandlerExecutor;
        private readonly OrdersRepository _ordersRepository;

        public OrderController(OrdersRepository ordersRepository, OrdersConverter ordersConverter,
            OrdersHandlerExecutor ordersHandlerExecutor)
        {
            _ordersRepository = ordersRepository;
            _ordersConverter = ordersConverter;
            _ordersHandlerExecutor = ordersHandlerExecutor;
        }

        [HttpPost("{systemType}")]
        public async Task<ActionResult> Post(string systemType)
        {
            try
            {
                if (!_ordersHandlerExecutor.OrderHandlers.ContainsKey(systemType))
                    return NotFound(systemType);

                using var reader = new StreamReader(Request.Body);
                var sourceOrder = await reader.ReadToEndAsync();
                var dbOrder = _ordersConverter.CreateDbModel(sourceOrder, systemType);
                await _ordersRepository.AddNewOrder(dbOrder);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}