using Senti.Shared.Models.Quotes;

namespace Senti.Quotes.Core.Cache
{
    public class QuoteCacheContext
    {
        public SemaphoreSlim Sync = new(1);
        public Dictionary<string, List<RawQuote>> Data { get; set; } = 
            new Dictionary<string, List<RawQuote>>();
        public bool IsInitialized { get; set; } = false;
    }
}
