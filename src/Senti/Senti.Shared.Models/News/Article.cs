namespace Senti.Shared.Models.News;
public class Article
{
    public int Id { get; set; }
    public DateTime PublishDate { get; set; }
    public string PublishDateStr { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public double SentimentPositive { get; set; }
    public double SentimentNegative { get; set; }
    public double SentimentNeutral { get; set; }
    public string Version { get; set; } = "1.0";
}
