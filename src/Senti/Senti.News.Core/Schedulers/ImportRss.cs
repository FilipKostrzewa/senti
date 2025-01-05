using Senti.News.Core.Rss;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
namespace Senti.News.Core.Schedulers;
public class ImportRss
{
    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;

    public ImportRss(LogToStorage logToStorage, StorageAdapter storageAdapter)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
    }

    public async Task Run()
    {
        var rssListJson = Environment.GetEnvironmentVariable(nameof(Envars.Rss_List));
        var stockListJson = Environment.GetEnvironmentVariable(nameof(Envars.Stock_List));

        var rssList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rssListJson);
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);

        var noActivity = true;

        foreach (var rss in rssList)
        {
            foreach (var stock in stockList)
            {
                await ImportRssProvider(rss, stock);
            }
        }

        //await _logToStorage.Log(nameof(ImportRss), "No activity");
    }

    private async Task ImportRssProvider(string rss, string stock)
    {
        var fileName = RssFileNameFactory.Create(rss, stock);
        if (await _storageAdapter.Exists(StorageContainers.Rss, fileName))
        {
            return;
        }

        var url = Environment.GetEnvironmentVariable($"Rss_{rss}");
        url = url.Replace("{{stock}}", stock);

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string rssContent = await response.Content.ReadAsStringAsync();

        await _storageAdapter.Upload(StorageContainers.Rss, fileName, rssContent);

        await _logToStorage.Log(nameof(ImportRss), $"{rssContent.Length} chars");
    }
}
