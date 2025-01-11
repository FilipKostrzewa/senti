using Senti.Shared.Adapters.Storages;

namespace Senti.News.Core.Articles;
public static class NewsFileNameFactory
{
    public static string Create(string stock)
    {
        return DailyFileNameFactory.Create($"{stock}", "json");
    }

    public static string CreateForYesterday(string stock)
    {
        return DailyFileNameFactory.CreateForYesterday($"{stock}", "json");
    }
}
