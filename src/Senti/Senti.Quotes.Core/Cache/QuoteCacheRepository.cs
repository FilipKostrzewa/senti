using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.Quotes;
using System.Text.Json;

namespace Senti.Quotes.Core.Cache;

public class QuoteCacheRepository(
        QuoteCacheContext cacheContext,
        StorageAdapter storageAdapter)
{
    private readonly QuoteCacheContext _ctx = cacheContext;
    private readonly StorageAdapter _storageAdapter = storageAdapter;
    private List<string> _stockList;
    private string _container;

    public async Task<IReadOnlyList<QuoteMini>> GetByStock(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return new List<QuoteMini>();
        }

        return _ctx.Data[stock];
    }

    public async Task<QuoteMini[]> Get(string stock, long from, long to)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return [];
        }

        var data = _ctx.Data[stock]
            .Where(x => x.t >= from && x.t <= to)
            .OrderBy(x => x.t)
            .ToArray();

        return data;
    }

    public async Task<int> Count(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return 0;
        }

        return _ctx.Data[stock].Count();
    }

    public async Task<int> MinUnix(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return 0;
        }

        return _ctx.Data[stock].Min(x => x.t);
    }

    public async Task<int> MaxUnix(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return 0;
        }

        return _ctx.Data[stock].Max(x => x.t);
    }

    public async Task<string> MinDate(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return string.Empty;
        }

        var t = _ctx.Data[stock].Min(x => x.t);
        var date = DateTimeOffset.FromUnixTimeSeconds(t);

        return date.ToString("U");
    }

    public async Task<string> MaxDate(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return string.Empty;
        }

        var t = _ctx.Data[stock].Max(x => x.t);
        var date = DateTimeOffset.FromUnixTimeSeconds(t);

        return date.ToString("U");
    }


    public async Task Init()
    {
        var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
        _stockList = JsonSerializer.Deserialize<List<string>>(stockListJson);
        _container = Environment.GetEnvironmentVariable(Envars.Senti_Container_Quotes5m);

        _ctx.Data = new Dictionary<string, List<QuoteMini>>();
        foreach (var stock in _stockList)
        {
            _ctx.Data.Add(stock, new List<QuoteMini>());
        }

    }
    public async Task InitData()
    {
        foreach (var stock in _stockList)
        {
            for (int i = 0; i < 12; i++)
            {
                var utcNow = DateTime.UtcNow.AddMonths(-1 * i);

                var date = new DateTime(
                    utcNow.Year, utcNow.Month, 1, 0, 0, 0, kind: DateTimeKind.Utc);

                await ReadMonthFiles(stock, date);
            }
        }
    }

    private async Task ReadMonthFiles(string stock, DateTime date)
    {
        await ReadChunkFile(stock, date, 1);
        await ReadChunkFile(stock, date, 2);
        await ReadChunkFile(stock, date, 3);
    }

    private async Task ReadChunkFile(string stock, DateTime date, int chunk)
    {
        var fileName = $"{stock}-{date:yyMM}-{chunk}";

        if (await _storageAdapter.Exists(_container, fileName) is false)
            return;

        var quotesJson = await _storageAdapter.Download(_container, fileName);
        var quoteList = JsonSerializer.Deserialize<RawQuoteList>(quotesJson);

        await AddToDataSafe(stock, quoteList);
    }

    private async Task AddToDataSafe(string stock, RawQuoteList quoteList)
    {
        await _ctx.Sync.WaitAsync();
        _ctx.Data[stock]
            .AddRange(quoteList.results
            .Select(x => new QuoteMini(x)));
        _ctx.Sync.Release();
    }
}
