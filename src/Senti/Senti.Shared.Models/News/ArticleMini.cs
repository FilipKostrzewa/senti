using System.Globalization;

namespace Senti.Shared.Models.News;
public class ArticleMini
{
    public int Published { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public double SentimentPositive { get; set; }
    public double SentimentNegative { get; set; }
    public double SentimentNeutral { get; set; }

    public ArticleMini() { }

    public ArticleMini(Article article) 
    {
        Title = article.Title;
        Url = article.Url;
        Source = article.Source;
        SentimentPositive = article.SentimentPositive;
        SentimentNegative = article.SentimentNegative;
        SentimentNeutral = article.SentimentNeutral;

        Published = GetUnixSeconds(article.PublishDateStr);
    }

    private int GetUnixSeconds(string dateStr)
    {
        var date = GetDate(dateStr);
        var unixSec = date.ToUnixTimeSeconds();

        return (int)unixSec;
    }

    public DateTimeOffset GetDate(string dateStr)
    {
        DateTimeOffset date;

        List<string> formats = [
            "ddd, dd MMM yyyy HH:mm:ss zzz",
            "ddd, dd MMM yyyy HH:mm:ss 'GMT'"
            ];

        foreach (var format in formats)
        {
            var dateParsed = DateTimeOffset.TryParseExact(
                dateStr, 
                format, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.AssumeUniversal, 
                out date);

            if (dateParsed)
            {
                return date;
            }
        }

        throw new ArgumentException($"Published: {dateStr}");
    }
}
