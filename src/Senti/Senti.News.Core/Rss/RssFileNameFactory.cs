using Senti.Shared.Adapters.Storages;
using Senti.Shared.Models.News;

namespace Senti.News.Core.Rss;
public static class RssFileNameFactory
{
    public static string Create(string rss, string stock)
    {
        return DailyFileNameFactory.Create($"{rss}-{stock}", "xml");
    }
}
