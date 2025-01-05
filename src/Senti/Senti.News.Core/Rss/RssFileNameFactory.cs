using Senti.Shared.Adapters.Storages;

namespace Senti.News.Core.Rss;
public static class RssFileNameFactory
{
    public static string Create(string rss, string stock)
    {
        return DailyFileNameFactory.Create($"{rss}-{stock}", "xml");
    }
}
