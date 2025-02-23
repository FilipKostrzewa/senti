using Polygon.Client;
using Polygon.Client.Requests;
using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;
using Senti.Shared.Models.Quotes;

namespace Senti.Quotes.Core.Schedulers;
public enum TimePeriod
{
    Day = 1,
    Month = 2,
}
public class ImportQuotes
{
    private const string _customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;

    public ImportQuotes(LogToStorage logToStorage, StorageAdapter storageAdapter)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
    }

    public async Task Run(DateTime date, TimePeriod period)
    {
        var stockListJson = Environment.GetEnvironmentVariable(nameof(Envars.Stock_List));
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);

        
        foreach (var stock in stockList)
        {
            await ImportForStock(date, stock, period);

            await Task.Delay(1000);
        }
    }

    private async Task ImportForStock(DateTime date, string stock, TimePeriod period)
    {
        var fileName = period switch
        {
            TimePeriod.Day => QuoteFileNameFactory.CreateForDay(date, stock),
            TimePeriod.Month => QuoteFileNameFactory.CreateForMonth(date, stock),
            _ => throw new Exception("unknown time period"),
        };

        var container = period switch
        {
            TimePeriod.Day => StorageContainers.DailyQuotes,
            TimePeriod.Month => StorageContainers.MonthlyQuotes,
            _ => throw new Exception("unknown time period"),
        };

        if (await _storageAdapter.Exists(container, fileName))
        {
            throw new Exception($"storage container does not exist: {container}");
        }

        var range = period switch
        {
            TimePeriod.Day => ($"{date:yyyy-MM-dd}", $"{date:yyyy-MM-dd}"),
            TimePeriod.Month => ($"{date:yyyy-MM}-01", $"{date.AddMonths(1):yyyy-MM}-01"),
            _ => throw new Exception("unknown time period"),
        };

        var url = Environment.GetEnvironmentVariable(Envars.QuotesApi_Endpoint);
        var key = Environment.GetEnvironmentVariable(Envars.QuotesApi_Key);

        url = url
            .Replace("{{key}}", key)
            .Replace("{{stock}}", stock)
            .Replace("{{from}}", range.Item1)
            .Replace("{{to}}", range.Item2);

        await _logToStorage.Log(nameof(ImportQuotes), $"{stock} request", url);

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", _customUserAgent);

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        await _storageAdapter.Upload(container, fileName, content);

        await _logToStorage.Log(nameof(ImportQuotes), $"{stock} {content.Length} chars");

        //var polygonClient = new PolygonClient(key);

        //var request = new PolygonAggregateRequest
        //{
        //    Ticker = stock,
        //    Timespan = "minute",
        //    From = range.Item1,
        //    To = range.Item2,
        //};

        //var response = await polygonClient.GetAggregates(request);
        //var content = response.Results.ToList();

        //var contentJson = System.Text.Json.JsonSerializer.Serialize(content);

        //await _storageAdapter.Upload(container, fileName, contentJson);

        //await _logToStorage.Log(nameof(ImportQuotes), $"{stock} {contentJson.Length} chars");
    }
}
