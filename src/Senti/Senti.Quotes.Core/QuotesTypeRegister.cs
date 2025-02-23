using Microsoft.Extensions.DependencyInjection;
using Senti.Quotes.Core.Schedulers;

namespace Senti.News.Core;
public static class QuotesTypeRegister
{
    public static void Register(this IServiceCollection services)
    {
        services.AddTransient<ImportQuotes>();
    }
}
