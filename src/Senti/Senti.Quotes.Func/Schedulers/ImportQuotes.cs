using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Core = Senti.Quotes.Core.Schedulers;

namespace Senti.Azure.Functions.Schedulers;

public class ImportQuotes
{
    private readonly ILogger _logger;
    private readonly Core.ImportQuotes _importQuotes;

    public ImportQuotes(ILoggerFactory loggerFactory, Core.ImportQuotes importQuotes)
    {
        _logger = loggerFactory.CreateLogger<ImportQuotes>();
        _importQuotes = importQuotes;
    }

    [Function(nameof(ImportQuotes))]
    public async Task Run([TimerTrigger("%Senti_Cron_ImportQuotes%")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        await _importQuotes.Run();
    }
}
