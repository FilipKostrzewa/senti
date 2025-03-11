using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models;

namespace Senti.Quotes.Core.Schedulers;
public class ImportQuotes
{
    private const string _customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
    private const int _httpGetCountMax = 5;
    private int _httpGetCount = 0;

    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;
    private string _container = Environment.GetEnvironmentVariable(Envars.Senti_Container_Quotes5m);

    public ImportQuotes(LogToStorage logToStorage, StorageAdapter storageAdapter)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
    }

    public async Task Run()
    {
        var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
        var stockList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(stockListJson);

        for (int i = 0; i < 12; i++)
        {
            var utcNow = DateTime.UtcNow.AddMonths(-1 * i);

            var date = new DateTime(
                utcNow.Year, utcNow.Month, 1, 0, 0, 0, kind: DateTimeKind.Utc);

            foreach (var stock in stockList)
            {
                await ImportStock(stock, date);

                if (isHttpGetCountExceeded())
                    break;
            }

            if (isHttpGetCountExceeded())
                break;
        }
    }

    private async Task ImportStock(string stock, DateTime date)
    {
        var fileNamePrefix = $"{stock}-{date:yyMM}";

        var fileName1 = $"{fileNamePrefix}-1";
        await ImportChunk(date, date.AddDays(10), stock, fileName1);

        if (DateTime.UtcNow.Year == date.Year &&
            DateTime.UtcNow.Month == date.Month &&
            DateTime.UtcNow.Day < 11)
            return;

        var fileName2 = $"{fileNamePrefix}-2";
        await ImportChunk(date.AddDays(10), date.AddDays(20), stock, fileName2);

        if (DateTime.UtcNow.Year == date.Year &&
            DateTime.UtcNow.Month == date.Month &&
            DateTime.UtcNow.Day < 21)
            return;

        var fileName3 = $"{fileNamePrefix}-3";
        await ImportChunk(date.AddDays(20), date.AddMonths(1), stock, fileName3);
    }

    private async Task ImportChunk(DateTime from, DateTime to, string stock, string fileName)
    {
        if (isHttpGetCountExceeded()) 
            return;

        if (await _storageAdapter.Exists(container, fileName)) 
            return;

        var url = Environment.GetEnvironmentVariable(Envars.Senti_QuotesApi_Endpoint);
        var key = Environment.GetEnvironmentVariable(Envars.Senti_QuotesApi_Key);
        url = url
            .Replace("{{key}}", key)
            .Replace("{{stock}}", stock)
            .Replace("{{from}}", $"{from:yyyy-MM-dd}")
            .Replace("{{to}}", $"{to:yyyy-MM-dd}");

        _httpGetCount++;
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", _customUserAgent);
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        await _storageAdapter.Upload(container, fileName, content);
        await _logToStorage.Log(nameof(ImportQuotes), $"{fileName} {content.Length} chars");
    }

    private bool isHttpGetCountExceeded()
    {
        if (_httpGetCount >= _httpGetCountMax)
            return true;

        return false;
    }
}
