using Senti.Shared.Adapters.Storages;

namespace Senti.News.Core.Schedulers;
public class ImportRss
{
    private readonly LogToStorage _logToStorage;

    public ImportRss(LogToStorage logToStorage)
    {
        _logToStorage = logToStorage;
    }

    public async Task Run()
    {
        await _logToStorage.Log(nameof(ImportRss), "No activity");
    }
}
