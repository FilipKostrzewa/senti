using Senti.News.Core.Articles;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.News;
using System.Text.Json;

namespace Senti.News.Core.Queries;
public class GetArticles
{
    private readonly StorageAdapter _storageAdapter;

    public GetArticles(StorageAdapter storageAdapter)
    {
        _storageAdapter = storageAdapter;
    }

    public async Task<List<Article>> Get(string stock)
    {
        var stockListJson = Environment.GetEnvironmentVariable(nameof(Envars.Stock_List));
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);
        if (!stockList.Contains(stock))
        {
            throw new ArgumentException("Stock not found");
        }
        var fileName = NewsFileNameFactory.Create(stock);
        if (!await _storageAdapter.Exists(StorageContainers.News, fileName))
        {
            throw new ArgumentException("News not found");
        }
        var newsJson = await _storageAdapter.Download(StorageContainers.News, fileName);
        var news = JsonSerializer.Deserialize<List<Article>>(newsJson);

        return news;
    }
}

