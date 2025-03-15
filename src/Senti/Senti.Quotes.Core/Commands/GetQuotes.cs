using Senti.Quotes.Core.Cache;
using Senti.Shared.Models.Quotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senti.Quotes.Core.Commands
{
    public class GetQuotes
    {
        private readonly QuoteCacheRepository _repo;

        public GetQuotes(QuoteCacheRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<RawQuote>> GetByStock(string stock)
        {
            return await _repo.GetByStock(stock);
        }
    }
}
