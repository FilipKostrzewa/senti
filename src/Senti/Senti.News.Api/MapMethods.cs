using Senti.News.Core.Queries;

//[assembly: AssemblyVersion("1.0.*")]
namespace Senti.News.Api;

public static class MapMethods
{
    public static void MapAllMethods(this WebApplication app)
    {
        app.MapGet("/news/{stock}", (GetArticles getQuotes, string stock) =>
        {
            return getQuotes.GetByStock(stock);
        });

        app.MapGet("/news", (HttpRequest request, GetArticles getQuotes) =>
        {
            var stock = request.Query["stock"];
            var from = long.Parse(request.Query["from"]);
            var to = long.Parse(request.Query["to"]);

            return getQuotes.Get(stock, from, to);
        });

        app.MapGet("/count/{stock}", (string stock, GetArticles getQuotes) => getQuotes.Count(stock));
        app.MapGet("/min-unix/{stock}", (string stock, GetArticles getQuotes) => getQuotes.MinUnix(stock));
        app.MapGet("/max-unix/{stock}", (string stock, GetArticles getQuotes) => getQuotes.MaxUnix(stock));
        app.MapGet("/min-date/{stock}", (string stock, GetArticles getQuotes) => getQuotes.MinDate(stock));
        app.MapGet("/max-date/{stock}", (string stock, GetArticles getQuotes) => getQuotes.MaxDate(stock));

    }
}
