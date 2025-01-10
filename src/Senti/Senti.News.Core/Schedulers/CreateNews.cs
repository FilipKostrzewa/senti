using Senti.News.Core.Articles;
using Senti.News.Core.Rss;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.News;
using System.Text.Json;
using System.Xml.Linq;
namespace Senti.News.Core.Schedulers;
public class CreateNews
{
    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;
    private readonly ArticleFactory _articleFactory;

    public CreateNews(LogToStorage logToStorage, StorageAdapter storageAdapter, ArticleFactory articleFactory)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
        _articleFactory = articleFactory;
    }

    public async Task Run()
    {
        var stockListJson = Environment.GetEnvironmentVariable(nameof(Envars.Stock_List));
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);

        foreach (var stock in stockList)
        {
            await CreateArticlesForStock(stock);
        }
    }

    private async Task CreateArticlesForStock(string stock)
    {
        var fileName = NewsFileNameFactory.Create(stock);

        if (await _storageAdapter.Exists(StorageContainers.News, fileName))
        {
            return;
        }

        var rssListJson = Environment.GetEnvironmentVariable(nameof(Envars.Rss_List));
        var rssList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rssListJson);

        var articles = new List<Article>();

        foreach (var rss in rssList)
        {
            articles.AddRange(await CreateArticlesForRss(rss, stock));
        }

        await _storageAdapter.Upload(
            StorageContainers.News, 
            fileName, 
            JsonSerializer.Serialize(articles));

    }

    private async Task<IEnumerable<Article>> CreateArticlesForRss(string rss, string stock)
    {
        var rssStream = await _storageAdapter.Download(
            StorageContainers.Rss, 
            RssFileNameFactory.Create(rss, stock));

        var articles = await _articleFactory.Create(rssStream);

        return articles;
    }

}
