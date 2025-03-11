using Senti.Shared.Models.Quotes;

namespace Senti.Quotes.Core.Cache
{
    public class QuoteCacheContext
    {
        public SemaphoreSlim Sync = new(1);
        public List<RawQuote> Data { get; set; }


    }
}
