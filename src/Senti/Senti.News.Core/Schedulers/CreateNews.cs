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
        var stockList = JsonSerializer.Deserialize<List<string>>(stockListJson);

        foreach (var stock in stockList)
        {
            await CreateArticlesForStock(stock);
        }
    }

    private async Task CreateArticlesForStock(string stock)
    {
        var fileName = NewsFileNameFactory.Create(stock);
        var prevFileName = NewsFileNameFactory.CreateForYesterday(stock);

        if (await _storageAdapter.Exists(StorageContainers.News, fileName))
        {
            return;
        }

        var rssListJson = Environment.GetEnvironmentVariable(nameof(Envars.Rss_List));
        var rssList = JsonSerializer.Deserialize<List<string>>(rssListJson);

        var articles = new List<Article>();
        if (await _storageAdapter.Exists(StorageContainers.News, prevFileName))
        {
            var articlesJson = await _storageAdapter.Download(
                StorageContainers.News,
                NewsFileNameFactory.CreateForYesterday(stock));
            articles = JsonSerializer.Deserialize<List<Article>>(articlesJson);
        }

        foreach (var rss in rssList)
        {
            var newArticles = await CreateArticlesForRss(rss, stock);
            var newUniqueArticles = newArticles
                .Where(a => !articles.Any(a2 => 
                    a2.Title == a.Title && 
                    a2.PublishDate == a.PublishDate));
            articles.AddRange(newUniqueArticles);
        }

        await _articleFactory.CreateSentiment(articles);

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
