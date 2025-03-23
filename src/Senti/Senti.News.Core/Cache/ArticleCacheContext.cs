using Senti.Shared.Models.News;

namespace Senti.News.Core.Cache
{
    public class ArticleCacheContext
    {
        public SemaphoreSlim Sync = new(1);
        public Dictionary<string, List<ArticleMini>> Data { get; set; } = 
            new Dictionary<string, List<ArticleMini>>();
        public bool IsInitialized { get; set; } = false;
    }
}
