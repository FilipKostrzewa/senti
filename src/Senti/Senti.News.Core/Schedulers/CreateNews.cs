using Senti.News.Core.Articles;
using Senti.News.Core.Rss;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using System.Xml.Linq;
namespace Senti.News.Core.Schedulers;
public class CreateNews
{
    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;

    public CreateNews(LogToStorage logToStorage, StorageAdapter storageAdapter)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
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
        foreach (var rss in rssList)
        {
        }

    }


    static IEnumerable<(string Title, string Description, string Url)> ParseRssFeed(string rssContent)
    {
        var xdoc = XDocument.Parse(rssContent);
        var items = xdoc.Descendants("item")
            .Select(item => (
                Title: item.Element("title")?.Value,
                Description: item.Element("description")?.Value,
                Url: item.Element("link")?.Value
            ));

        return items;
    }



}
