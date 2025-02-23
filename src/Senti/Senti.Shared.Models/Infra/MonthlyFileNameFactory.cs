namespace Senti.Shared.Models.Infra;
public static class MonthlyFileNameFactory
{
    public static string Create(DateTime date, string name, string extension = null)
    {
        var fileName = $"{date:yyMM}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }

    public static string CreateForCurrentMonth(string name, string extension = null)
    {
        var fileName = $"{DateTime.UtcNow:yyMM}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }

    public static string CreateForLastMonth(string name, string extension = null)
    {
        var date = DateTime.UtcNow.AddMonths(-1);
        var fileName = $"{date:yyMM}-{name}";

        if (!string.IsNullOrEmpty(extension))
        {
            fileName += $".{extension}";
        }

        return fileName;
    }
}
