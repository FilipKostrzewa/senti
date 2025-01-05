using Senti.Shared.Models;

namespace Senti.Shared.Adapters.Storages;
public class LogToStorage
{
    private readonly StorageAdapter _storageAdapter;

    public LogToStorage(StorageAdapter storageAdapter)
    {
        _storageAdapter = storageAdapter;
    }

    public async Task Log(string source, string message, string innerMessage = "")
    {
        await _storageAdapter.Upload(
            StorageContainers.Logs, 
            $"{DateTime.UtcNow:yyMMdd HH:mm} {source} {message}", 
            innerMessage);
    }
}
