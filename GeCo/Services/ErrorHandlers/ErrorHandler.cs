using System;
using System.Threading;
using System.Threading.Tasks;
using GeCoTest.Services.Loggers;

namespace GeCoTest.Services.ErrorHandlers
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogService _logService;

        public ErrorHandler(ILogService logService)
        {
            _logService = logService;
        }

        public async Task HandleError(Exception exception, Guid dbOrderId)
        {
            var errorText = $"{exception.Message} when Handle order with Id {dbOrderId}";
            await _logService.Log(errorText);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}