using Senti.Shared.Models.News;
using System.Globalization;
using System.Xml.Linq;

namespace Senti.News.Core.Articles;
public class ArticleFactory
{
    private readonly TextSentimentFactory _textSentimentFactory;

    public ArticleFactory(TextSentimentFactory textSentimentFactory)
    {
        _textSentimentFactory = textSentimentFactory;
    }

    public async Task<List<Article>> Create(Stream rssStream)
    {
        var xdoc = XDocument.Load(rssStream);
        var articles = xdoc.Descendants("item")
            .Select(x => new Article
            {
                Title = x.Element("title")?.Value,
                //Description = x.Element("description")?.Value,
                Url = x.Element("link")?.Value,
                PublishDateStr = x.Element("pubDate")?.Value
            })
            .ToList();

        string parseFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";

        foreach (var article in articles)
        {
            var sentiment = await _textSentimentFactory.Create(article.Title);

            article.SentimentPositive = sentiment.Positive;
            article.SentimentNegative = sentiment.Negative;
            article.SentimentNeutral = sentiment.Neutral;

            article.PublishDate = GetDate(article.PublishDateStr);
        }

        return articles;
    }

    public DateTime GetDate(string dateStr)
    {
        DateTime date;

        List<string> formats = [
            "ddd, dd MMM yyyy HH:mm:ss zzz",
            "ddd, dd MMM yyyy HH:mm:ss 'GMT'"
            ];

        foreach (var format in formats)
        {
            var dateParsed = DateTime.TryParseExact(
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

        return DateTime.Now;
    }
}
