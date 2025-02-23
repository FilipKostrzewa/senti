using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Core = Senti.Quotes.Core.Schedulers;

namespace Senti.Azure.Functions.Schedulers
{
    public class ImportDailyQuotes
    {
        private readonly ILogger _logger;
        private readonly Core.ImportQuotes _importQuotes;

        public ImportDailyQuotes(ILoggerFactory loggerFactory, Core.ImportQuotes importQuotes)
        {
            _logger = loggerFactory.CreateLogger<ImportQuotes>();
            _importQuotes = importQuotes;
        }

        [Function(nameof(ImportDailyQuotes))]
        public async Task Run([TimerTrigger("%Cron_ImportDailyQuotes%")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            var date = DateTime.UtcNow;

            await _importQuotes.Run(date, Core.TimePeriod.Day);
        }
    }
}
