using System.Xml.Linq;

namespace Senti.News.Core.Articles;
public class ArticleFactory
{
    private readonly TextSentimentFactory _textSentimentFactory;

    public ArticleFactory(TextSentimentFactory textSentimentFactory)
    {
        _textSentimentFactory = textSentimentFactory;
    }

    public async Task<List<Article>> Create(string rssContent)
    {
        var xdoc = XDocument.Parse(rssContent);
        var articles = xdoc.Descendants("item")
            .Select(x => new Article
            {
                Title = x.Element("title")?.Value,
                Description = x.Element("description")?.Value,
                Url = x.Element("link")?.Value,
                PublishDate = x.Element("pubDate")?.Value
            })
            .ToList();

        foreach (var article in articles)
        {
            var sentiment = await _textSentimentFactory.Create(article.Title + ". " + article.Description);

            article.SentimentPositive = sentiment.Positive;
            article.SentimentNegative = sentiment.Negative;
            article.SentimentNeutral = sentiment.Neutral;
        }

        return articles;


    }
}
