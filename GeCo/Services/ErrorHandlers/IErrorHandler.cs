using System;
using System.Threading.Tasks;

namespace GeCoTest.Services.ErrorHandlers
{
    public interface IErrorHandler
    {
        public Task HandleError(Exception exception, Guid dbOrderId);
    }
}