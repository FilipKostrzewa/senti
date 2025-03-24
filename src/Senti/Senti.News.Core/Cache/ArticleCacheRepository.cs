using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.News;
using System.Text.Json;

namespace Senti.News.Core.Cache;

public class ArticleCacheRepository
{
    private readonly ArticleCacheContext _ctx;
    private readonly StorageAdapter _storageAdapter;
    private List<string> _stockList;
    private string _container;

    public ArticleCacheRepository(ArticleCacheContext ctx, StorageAdapter storageAdapter)
    {
        _ctx = ctx;
        _storageAdapter = storageAdapter;
        var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
        _stockList = JsonSerializer.Deserialize<List<string>>(stockListJson);
        _container = Environment.GetEnvironmentVariable(Envars.Senti_Container_Articles);
    }

    public async Task<IReadOnlyList<ArticleMini>> GetByStock(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return new List<ArticleMini>();
        }

        return _ctx.Data[stock];
    }

    public async Task<ArticleMini[]> Get(string stock, long from, long to)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return [];
        }

        var data = _ctx.Data[stock]
            .Where(x => x.Published >= from && x.Published <= to)
            .OrderBy(x => x.Published)
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

        return _ctx.Data[stock].Min(x => x.Published);
    }

    public async Task<int> MaxUnix(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return 0;
        }

        return _ctx.Data[stock].Max(x => x.Published);
    }

    public async Task<string> MinDate(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return string.Empty;
        }

        var t = _ctx.Data[stock].Min(x => x.Published);
        var date = DateTimeOffset.FromUnixTimeSeconds(t);

        return date.ToString("U");
    }

    public async Task<string> MaxDate(string stock)
    {
        if (_ctx.Data.ContainsKey(stock) is not true)
        {
            return string.Empty;
        }

        var t = _ctx.Data[stock].Max(x => x.Published);
        var date = DateTimeOffset.FromUnixTimeSeconds(t);

        return date.ToString("U");
    }


    private async Task Init()
    {

    }
    public async Task ReadData()
    {
        foreach (var stock in _stockList)
        {
            var utcNow = DateTime.UtcNow;

            int counter = 0;
            for (int i = 0; i < 100; i++)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                counter += await ReadDalyFile(stock, date);

                if (counter > 5)
                    return;
            }
        }
    }

    private async Task<int> ReadDalyFile(string stock, DateTime date)
    {
        var fileName = $"{date:yyMMdd}-{stock}.json";
        if (await _storageAdapter.Exists(_container, fileName) is false)
            return 0;

        var articlesJson = await _storageAdapter.Download(_container, fileName);
        var articleList = JsonSerializer.Deserialize<List<Article>>(articlesJson);
        if (articleList.Count == 0)
            return 0;

        var allUrls = _ctx.Data[stock].Select(x => x.Url).ToList();
        var newArticles = articleList
            .Where(x => !allUrls.Contains(x.Url))
            .ToList();
        if (newArticles.Count == 0)
            return 1;

        await AddToDataSafe(stock, articleList);

        return 0;
    }

    private async Task AddToDataSafe(string stock, List<Article> articleList)
    {
        var urls = articleList
            .Select(x => x.Url)
            .Distinct();

        var newMinis = new List<ArticleMini>();
        foreach (var url in urls)
        {
            var article = articleList.First(x => x.Url == url);
            newMinis.Add(new ArticleMini(article));
        }

        await _ctx.Sync.WaitAsync();
        _ctx.Data[stock].AddRange(newMinis);
        _ctx.Sync.Release();
    }
}
