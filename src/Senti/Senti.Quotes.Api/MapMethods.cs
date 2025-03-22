using Senti.Quotes.Core.Commands;
using System.Reflection;

//[assembly: AssemblyVersion("1.0.*")]
namespace Senti.Quotes.Api;

public static class MapMethods
{
    public static void MapAllMethods(this WebApplication app)
    {
        app.MapGet("/quotes/{stock}", (GetQuotes getQuotes, string stock) =>
        {
            return getQuotes.GetByStock(stock);
        });

        app.MapGet("/quotes", (HttpRequest request, GetQuotes getQuotes) =>
        {
            var stock = request.Query["stock"];
            var from = long.Parse(request.Query["from"]);
            var to = long.Parse(request.Query["to"]);

            return getQuotes.Get(stock, from, to);
        });

        app.MapGet("/count/{stock}", (string stock, GetQuotes getQuotes) => getQuotes.Count(stock));
        app.MapGet("/min-unix/{stock}", (string stock, GetQuotes getQuotes) => getQuotes.MinUnix(stock));
        app.MapGet("/max-unix/{stock}", (string stock, GetQuotes getQuotes) => getQuotes.MaxUnix(stock));
        app.MapGet("/min-date/{stock}", (string stock, GetQuotes getQuotes) => getQuotes.MinDate(stock));
        app.MapGet("/max-date/{stock}", (string stock, GetQuotes getQuotes) => getQuotes.MaxDate(stock));

    }
}
