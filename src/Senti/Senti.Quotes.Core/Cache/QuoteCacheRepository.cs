using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.Quotes;
using System.ComponentModel;
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

    //private async Task<IReadOnlyList<RawQuote>> ()
    //{
    //    if (_cacheContext.Data is not null)
    //        return _cacheContext.Data;

    //    await _cacheContext.Sync.WaitAsync();
    //    await Refresh();
    //    _cacheContext.Sync.Release();

    //    return _cacheContext.Data;
    //}

    public async Task<IReadOnlyList<RawQuote>> GetByStock(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return new List<RawQuote>();
        }

        return _ctx.Data[stock];
    }

    public async Task Init()
    {
        var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
        _stockList = JsonSerializer.Deserialize<List<string>>(stockListJson);
        _container = Environment.GetEnvironmentVariable(Envars.Senti_Container_Quotes5m);

        _ctx.Data = new Dictionary<string, List<RawQuote>>();
        foreach (var stock in _stockList)
        {
            _ctx.Data.Add(stock, new List<RawQuote>());
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
        _ctx.Data[stock].AddRange(quoteList.results);
        _ctx.Sync.Release();
    }
}
