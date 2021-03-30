using System.Threading.Tasks;

namespace GeCoTest.Services.Loggers
{
    public interface ILogService
    {
        public Task Log(string logText);
    }
}