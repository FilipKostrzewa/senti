using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.Quotes;
using System.Text.Json;

namespace Senti.Quotes.Core.Cache;

public class QuoteCacheRepository(
        QuoteCacheContext cacheContext,
        StorageAdapter storageAdapter)
{
    private readonly QuoteCacheContext _cacheContext = cacheContext;
    private readonly StorageAdapter _storageAdapter = storageAdapter;

    public async Task<IReadOnlyList<RawQuote>> Get()
    {
        if (_cacheContext.Data is not null)
            return _cacheContext.Data;

        await _cacheContext.Sync.WaitAsync();
        await Refresh();
        _cacheContext.Sync.Release();

        return _cacheContext.Data;
    }

    public async Task RefreshSafe()
    {
        await _cacheContext.Sync.WaitAsync();
        await Refresh();
        _cacheContext.Sync.Release();
    }

    private async Task Refresh()
    {
        var container = Environment.GetEnvironmentVariable(Envars.Senti_Container_Quotes5m);
        var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);

        for (int i = 0; i < 12; i++)
        {
            var utcNow = DateTime.UtcNow.AddMonths(-1 * i);

            var date = new DateTime(
                utcNow.Year, utcNow.Month, 1, 0, 0, 0, kind: DateTimeKind.Utc);

            foreach (var stock in stockList)
            {
                await ReadFile(stock, date, container);
            }
        }
    }

    private async Task ReadFile(string stock, DateTime date, string container)
    {
        var fileNamePrefix = $"{stock}-{date:yyMM}";

        var fileName = $"{fileNamePrefix}-1";
        await ReadFile(fileName, container);

        fileName = $"{fileNamePrefix}-2";
        await ReadFile(fileName, container);

        fileName = $"{fileNamePrefix}-3";
        await ReadFile(fileName, container);
    }

    private async Task ReadFile(string fileName, string container)
    {
        if (await _storageAdapter.Exists(container, fileName) is false)
            return;

        var quotesJson = await _storageAdapter.Download(container, fileName);
        var quotes = JsonSerializer.Deserialize<RawQuoteList>(quotesJson);
        _cacheContext.Data.AddRange(quotes.results);
    }
}
