using System;
using System.IO;
using System.Threading.Tasks;

namespace GeCoTest.Services.Loggers
{
    public class FileLogService : ILogService
    {
        private readonly string _filePath;

        public FileLogService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task Log(string logText)
        {
            await File.AppendAllTextAsync(_filePath, $"\n{DateTime.UtcNow}\n{logText}");
        }
    }
}