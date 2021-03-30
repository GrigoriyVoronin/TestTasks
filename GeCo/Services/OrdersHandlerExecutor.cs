using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeCoTest.Models.DB;
using GeCoTest.OrdersHandlers;
using GeCoTest.Repositories;
using GeCoTest.Services.ErrorHandlers;
using GeCoTest.Services.Factories;

namespace GeCoTest.Services
{
    public class OrdersHandlerExecutor
    {
        private const int Delay = 5;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IErrorHandler _errorHandler;
        private readonly OrdersRepositoryFactory _repositoryFactory;
        public readonly ImmutableDictionary<string, IOrderHandler> OrderHandlers;
        private Task _workingTask;

        public OrdersHandlerExecutor(IEnumerable<IOrderHandler> orderHandlers,
            OrdersRepositoryFactory repositoryFactory, IErrorHandler errorHandler)
        {
            _repositoryFactory = repositoryFactory;
            _errorHandler = errorHandler;
            OrderHandlers = orderHandlers
                .Where(x => x != null)
                .ToImmutableDictionary(x => x.SystemType);
        }

        public void StartWork()
        {
            if (_workingTask != null)
                throw new ApplicationException("Service already run");

            _workingTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await using var repository = _repositoryFactory.CreateRepository();
                    var orders = await GetNewOrders(repository);
                    var handledOrders = await HandleNewOrders(orders);
                    await SaveHandledOrders(handledOrders, repository);
                    await Task.Delay(TimeSpan.FromSeconds(Delay));
                }
            });
        }

        private async Task SaveHandledOrders(List<DbOrder> handledOrders, OrdersRepository repository)
        {
            await repository.SaveHandledOrders(handledOrders);
        }

        private async Task<List<DbOrder>> GetNewOrders(OrdersRepository repository)
        {
            return await repository.GetNewOrders();
        }

        private async Task<List<DbOrder>> HandleNewOrders(IEnumerable<DbOrder> orders)
        {
            var handledOrders = new List<DbOrder>();
            foreach (var dbOrder in orders)
                handledOrders.Add(await HandleOrder(dbOrder));
            return handledOrders;
        }

        private async Task<DbOrder> HandleOrder(DbOrder dbOrder)
        {
            try
            {
                await OrderHandlers[dbOrder.SystemType].HandleOrder(dbOrder);
            }
            catch (Exception exception)
            {
                _ = Task.Run(() => _errorHandler.HandleError(exception, dbOrder.Id));
            }

            //В ТЗ не указано нужно ли помечать заказ, как обработанный, при возникновении ошибки
            //решил, что да

            dbOrder.IsHandled = true;
            return dbOrder;
        }

        public void StopService()
        {
            if (_workingTask == null)
                throw new ApplicationException("Service already stopped");

            _cancellationTokenSource.Cancel();
            Task.WaitAll(_workingTask);
            _workingTask = null;
        }

        ~OrdersHandlerExecutor()
        {
            StopService();
        }
    }
}