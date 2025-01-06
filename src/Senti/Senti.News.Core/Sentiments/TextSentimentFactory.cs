using Azure.AI.TextAnalytics;
using Azure.Identity;
using Senti.Shared.Models;

public class TextSentimentFactory
{
    private readonly TextAnalyticsClient _client;

    public TextSentimentFactory()
    {
        var credential = new DefaultAzureCredential();
        var endpoint = Environment.GetEnvironmentVariable(Envars.AzureLanguageAI_Endpoint);

        _client = new TextAnalyticsClient(new Uri(endpoint), credential);
    }

    public async Task<SentimentConfidenceScores> Create(string text)
    {
        var result = await _client.AnalyzeSentimentAsync(text);

        return result?.Value?.ConfidenceScores;
    }
}