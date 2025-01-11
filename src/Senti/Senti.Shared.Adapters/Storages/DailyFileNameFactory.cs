namespace Senti.Shared.Adapters.Storages;
public static class DailyFileNameFactory
{
    public static string Create(string name, string extension = null)
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
