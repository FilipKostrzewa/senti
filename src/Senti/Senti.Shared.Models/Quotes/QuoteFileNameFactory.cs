using Senti.Shared.Models.Infra;

namespace Senti.Shared.Models.Quotes;

public static class QuoteFileNameFactory
{
    public static string CreateForDay(DateTime date, string stock)
    {
        return DailyFileNameFactory.Create(date, $"{stock}", "json");
    }

    public static string CreateForMonth(DateTime date, string stock)
    {
        return MonthlyFileNameFactory.Create(date, $"{stock}", "json");
    }

    public static string CreateForToday(string stock)
    {
        return DailyFileNameFactory.CreateForToday($"{stock}", "json");
    }

    public static string CreateForYesterday(string stock)
    {
        return DailyFileNameFactory.CreateForYesterday($"{stock}", "json");
    }
}
