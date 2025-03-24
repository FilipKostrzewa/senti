using Senti.Shared.Models;
using Senti.Shared.Models.News;
using System.Text.Json;

namespace Senti.News.Core.Cache
{
    public class ArticleCacheContext
    {
        public SemaphoreSlim Sync = new(1);
        public Dictionary<string, List<ArticleMini>> Data { get; set; } = 
            new Dictionary<string, List<ArticleMini>>();
        public bool IsInitialized { get; set; } = false;

        public ArticleCacheContext()
        {
            var stockListJson = Environment.GetEnvironmentVariable(Envars.Senti_Stocks);
            var stockList = JsonSerializer.Deserialize<List<string>>(stockListJson);

            Data = new Dictionary<string, List<ArticleMini>>();
            foreach (var stock in stockList)
            {
                Data.Add(stock, new List<ArticleMini>());
            }
        }
    }
}
