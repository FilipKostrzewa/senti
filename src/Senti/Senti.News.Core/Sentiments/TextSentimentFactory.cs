using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Senti.Shared.Models;

public class TextSentimentFactory
{
    private readonly TextAnalyticsClient _client;

    public TextSentimentFactory()
    {
        var endpoint = Environment.GetEnvironmentVariable(Envars.AzureLanguageAI_Endpoint);
        var key = Environment.GetEnvironmentVariable(Envars.AzureLanguageAI_Key);

        Uri uri = new(endpoint);
        AzureKeyCredential credential = new(key);
        _client = new(uri, credential);
    }

    public async Task<SentimentConfidenceScores> Create(string text)
    {
        var result = await _client.AnalyzeSentimentAsync(text);

        return result?.Value?.ConfidenceScores;
    }
}