using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Senti.Azure.Functions.Schedulers
{
    public class ImportNews
    {
        private readonly ILogger _logger;

        public ImportNews(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ImportNews>();
        }

        [Function("ImportNews")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
