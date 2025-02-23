using Senti.Shared.Models.Infra;

namespace Senti.Shared.Models.News;

public static class NewsFileNameFactory
{
    public static string Create(string stock)
    {
        return DailyFileNameFactory.CreateForToday($"{stock}", "json");
    }

    public static string CreateForYesterday(string stock)
    {
        return DailyFileNameFactory.CreateForYesterday($"{stock}", "json");
    }
}
