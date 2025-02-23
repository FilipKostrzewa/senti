namespace Senti.Shared.Models.Infra;
public static class DailyFileNameFactory
{
    public static string Create(DateTime date, string name, string extension = null)
    {
        var fileName = $"{date:yyMMdd}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }

    public static string CreateForToday(string name, string extension = null)
    {
        var fileName = $"{DateTime.UtcNow:yyMMdd}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }

    public static string CreateForYesterday(string name, string extension = null)
    {
        var date = DateTime.UtcNow.AddDays(-1);
        var fileName = $"{date:yyMMdd}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }
}
