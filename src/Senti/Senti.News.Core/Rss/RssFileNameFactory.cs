using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models.Infra;

namespace Senti.News.Core.Rss;
public static class RssFileNameFactory
{
    public static string Create(string rss, string stock)
    {
        return DailyFileNameFactory.CreateForToday($"{rss}-{stock}", "xml");
    }
}
