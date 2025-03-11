using Microsoft.Extensions.DependencyInjection;
using Senti.Quotes.Core.Schedulers;

namespace Senti.Quotes.Core;
public static class QuotesTypeRegister
{
    public static void Register(this IServiceCollection services)
    {
        services.AddTransient<ImportQuotes>();
    }
}
