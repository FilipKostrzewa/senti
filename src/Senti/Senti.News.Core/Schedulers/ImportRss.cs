using Senti.News.Core.Rss;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using System.Net.Http;
namespace Senti.News.Core.Schedulers;
public class ImportRss
{
    private const string _customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
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
                await ImportRssForStock(rss, stock);

                await Task.Delay(1000);
            }
        }
    }

    private async Task ImportRssForStock(string rss, string stock)
    {
        var fileName = RssFileNameFactory.Create(rss, stock);
        if (await _storageAdapter.Exists(StorageContainers.Rss, fileName))
        {
            return;
        }

        var url = Environment.GetEnvironmentVariable($"Rss_{rss}");
        url = url.Replace("{{stock}}", stock);

        await _logToStorage.Log(nameof(ImportRss), $"{rss} {stock} request", url);

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", _customUserAgent);

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string rssContent = await response.Content.ReadAsStringAsync();

        await _storageAdapter.Upload(StorageContainers.Rss, fileName, rssContent);

        await _logToStorage.Log(nameof(ImportRss), $"{rss} {stock} {rssContent.Length} chars");
    }
}
