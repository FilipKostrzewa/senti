using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Core = Senti.News.Core.Queries;

namespace Senti.Azure.Functions.Queries
{
    public class GetArticles
    {
        private readonly ILogger<GetArticles> _logger;
        private readonly Core.GetArticles _getArticles;

        public GetArticles(ILogger<GetArticles> logger, Core.GetArticles getArticles)
        {
            _logger = logger;
            _getArticles = getArticles;
        }

        [Function(nameof(GetArticles))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var stock = req.Query["stock"];
            var articles = await _getArticles.Get(stock);

            return new OkObjectResult(articles);
        }
    }
}
