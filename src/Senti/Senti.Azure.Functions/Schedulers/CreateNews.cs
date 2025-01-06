using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Core = Senti.News.Core.Schedulers;

namespace Senti.Azure.Functions.Schedulers
{
    public class CreateNews
    {
        private readonly ILogger _logger;
        private readonly Core.CreateNews _createNews;

        public CreateNews(ILoggerFactory loggerFactory, Core.CreateNews createNews)
        {
            _logger = loggerFactory.CreateLogger<ImportRss>();
            _createNews = createNews;
        }

        [Function(nameof(CreateNews))]
        public async Task Run([TimerTrigger("%Cron_CreateNews%")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            await _createNews.Run();
        }
    }
}
