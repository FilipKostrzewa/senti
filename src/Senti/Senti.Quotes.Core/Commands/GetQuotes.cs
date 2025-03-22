using Senti.Quotes.Core.Cache;
using Senti.Shared.Models.Quotes;

namespace Senti.Quotes.Core.Commands
{
    public class GetQuotes
    {
        private readonly QuoteCacheRepository _repo;

        public GetQuotes(QuoteCacheRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<QuoteMini>> GetByStock(string stock)
        {
            return await _repo.GetByStock(stock);
        }

        public async Task<QuoteMini[]> Get(string stock, long from, long to)
        {
            return await _repo.Get(stock, from, to);
        }

        public async Task<int> Count(string stock) => await _repo.Count(stock);
        public async Task<int> MinUnix(string stock) => await _repo.MinUnix(stock);
        public async Task<int> MaxUnix(string stock) => await _repo.MaxUnix(stock);
        public async Task<string> MinDate(string stock) => await _repo.MinDate(stock);
        public async Task<string> MaxDate(string stock) => await _repo.MaxDate(stock);
    }
}
