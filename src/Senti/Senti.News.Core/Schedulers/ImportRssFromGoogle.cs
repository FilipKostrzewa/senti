using Senti.Shared.Adapters.Storages;

namespace Senti.News.Core.Schedulers;
public class ImportRssFromGoogle
{
    private readonly LogToStorage _logToStorage;
    private readonly StorageAdapter _storageAdapter;

    public ImportRssFromGoogle(LogToStorage logToStorage, StorageAdapter storageAdapter)
    {
        _logToStorage = logToStorage;
        _storageAdapter = storageAdapter;
    }

    public async Task Run()
    {
        _storageAdapter.Download

        await _logToStorage.Log(nameof(ImportRss), "No activity");
    }
}
