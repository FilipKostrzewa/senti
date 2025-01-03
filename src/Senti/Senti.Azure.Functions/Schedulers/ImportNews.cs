using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Core = Senti.News.Core.Schedulers;

namespace Senti.Azure.Functions.Schedulers
{
    public class ImportNews
    {
        private readonly ILogger _logger;
        private readonly Core.ImportNews _importNews;

        public ImportNews(ILoggerFactory loggerFactory, Core.ImportNews importNews)
        {
            _logger = loggerFactory.CreateLogger<ImportNews>();
            _importNews = importNews;
        }

        [Function(nameof(ImportNews))]
        public async Task Run([TimerTrigger("%Cron_ImportNews%")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            await _importNews.Run();
        }
    }
}
