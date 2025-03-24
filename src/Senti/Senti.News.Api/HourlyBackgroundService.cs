
using Senti.News.Core.Cache;

public class HourlyBackgroundService : BackgroundService
{
    public readonly ArticleCacheRepository _repo;
    public HourlyBackgroundService(ArticleCacheRepository repo)
    {
        _repo = repo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            await _repo.ReadData();

            await Task.Delay(1000 * 60 * 60);
        }
    }
}