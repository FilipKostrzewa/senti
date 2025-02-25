using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.CandlestickLib;
using Senti.Shared.Models.News;
using Senti.Shared.Models.Quotes;
using Senti.Web.Shared;
using System.Globalization;
using System.Text.Json;
using Line = Plotly.Blazor.Traces.CandlestickLib.DecreasingLib.Line;

namespace Senti.Web.Views.QuoteHistory;

public class Quote
{
    public DateTime Time { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
}

public partial class QuoteHistory
{
    private bool _reloaded = false;
    private string _stock = "AAPL";
    private DateTime _pickedDate = new DateTime(2025, 02, 05, 0, 0, 0, kind: DateTimeKind.Utc);

    private PlotlyChart _chart;
    private Config _config;
    private Layout _layout;

    private Dictionary<string, List<Quote>> _quotes = new Dictionary<string, List<Quote>>();
    private IList<ITrace> _chartData = new List<ITrace>{ new Candlestick() };
    private IList<Article> _newsData = new List<Article>();

    private List<string> _stocks = new List<string> { "AAPL", "MSFT", "GOOGL", "NVDA", "AMZN", "TSLA", "MCD" };

    public async Task OnStockChange(object args)
    {
        _stock = args?.ToString() ?? _stocks[0];

        SetChartData();
        SetNewsData();
        await Task.Delay(500);
        await _chart.NewPlot();
    }

    public async Task OnDateChange(object args)
    {
        _pickedDate = (args as DateTime?) ?? 
            new DateTime(2025, 02, 05, 0, 0, 0, kind: DateTimeKind.Utc);

        SetChartData();
        SetNewsData();
        await Task.Delay(500);
        await _chart.NewPlot();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetAllQuotes();
            await SetArticlesForAllStocks();
            SetNewsData();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        InitChart();
        await SetQuotes(_stocks[0]);
        SetChartData();

        await base.OnInitializedAsync();
    }

    private void SetChartData()
    {
        if (!_quotes.ContainsKey(_stock) || 
            _quotes[_stock] == null || 
            _quotes[_stock].Count() == 0)
            return;

        var quotes = _quotes[_stock]
            .Where(x => x.Time > _pickedDate.AddDays(-7) &&  x.Time < _pickedDate.AddDays(1))
            .ToList();

        var candlestick = new Candlestick
        {
            X = quotes.Select(x => (object)x.Time).ToList(),

            Open = quotes.Select(x => (object)x.Open).ToList(),
            Close = quotes.Select(x => (object)x.Close).ToList(),

            High = quotes.Select(x => (object)x.High).ToList(),
            Low = quotes.Select(x => (object)x.Low).ToList(),

            Increasing = new Increasing { Line = new Plotly.Blazor.Traces.CandlestickLib.IncreasingLib.Line { Color = "#00cc00" } },
            Decreasing = new Decreasing { Line = new Line { Color = "#ff0000" } },

            XAxis = "date",
            YAxis = "price"
        };

        _chartData = new List<ITrace> { candlestick };
    }

    private async Task SetAllQuotes()
    {
        foreach (var stock in _stocks)
        {
            await SetQuotes(stock);
        }
    }

    private async Task SetQuotes(string stock)
    {
        if (_quotes.ContainsKey(stock) && 
            _quotes[stock] != null && 
            _quotes[stock].Count() > 0)
            return;

        var raw = await GetQuotes(stock);

        _quotes[stock] = raw.Select(x => new Quote
        {
            Time = DateTimeOffset.FromUnixTimeMilliseconds(x.t).UtcDateTime,

            Open = x.o,
            Close = x.c,

            High = x.h,
            Low = x.l,

        }).ToList();
    }

    private async Task<List<RawQuote>> GetQuotes(string stock)
    {
        var quoteFileNames = GetQuoteFileNames(stock);
        List<Task<RawQuoteList>> quoteTasks = new();

        quoteFileNames.ForEach(x =>
        {
            var task = GetQuoteFile(x);
            quoteTasks.Add(task);
        });

        await Task.WhenAll(quoteTasks);

        List<RawQuote> rawQuotes = new();
        quoteTasks.ForEach(async x => rawQuotes.AddRange( x.Result.results));

        return rawQuotes;
    }

    private async Task<RawQuoteList> GetQuoteFile(string fileName)
    {
        var storageUrl = Configuration[WebSettings.StorageUrl];
        var fileUrl = $"{storageUrl}/monthly-quotes/{fileName}";
        var response = await new HttpClient().GetStreamAsync(fileUrl);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<RawQuoteList>(response, options);
        return result;
    }

    private List<string> GetQuoteFileNames(string stock)
    {
        var today = DateTime.Today.Date;
        List<string> fileNames = new();
        List<DateTime> dates = [today, today.AddMonths(-1), today.AddMonths(-2)];

        dates.ForEach(x =>
        {
            var fileName = QuoteFileNameFactory.CreateForMonth(x, stock);
            fileNames.Add(fileName);
        });

        return fileNames;
    }

    private void InitChart()
    {
        _config = new Config { };
        _layout = new Layout
        {
            XAxis = new List<XAxis>
            {
                new()
                {
                    RangeSlider = new RangeSlider { Visible = false, },
                    Type = TypeEnum.Category,
                }
            },
            YAxis = new List<YAxis> { }
        };
    }

    private Dictionary<string, List<Article>> _articles = new Dictionary<string, List<Article>>();

    public async Task SetArticlesForAllStocks()
    {
        foreach (var stock in _stocks)
        {
            await SetArticles(stock);
        }
    }
    
    public async Task SetArticles(string stock)
    {
        var fileName = NewsFileNameFactory.Create(stock);
        var storageUrl = Configuration[WebSettings.StorageUrl];
        var fileUrl = $"{storageUrl}/news/{fileName}";
        var response = await new HttpClient().GetStreamAsync(fileUrl);

        var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        var result = JsonSerializer.Deserialize<List<Article>>(response, options);
        _articles[stock] = result.OrderByDescending(x => x.PublishDate).ToList();
    }

    public void SetNewsData()
    {
        if (!_articles.ContainsKey(_stock) ||
            _articles[_stock].Count == 0)
            return;

        _newsData = _articles[_stock]
            .Where(x =>
                x.PublishDate > _pickedDate.AddDays(-7) &&
                x.PublishDate < _pickedDate.AddDays(1))
            .ToList();

        Random rand = new Random();
        foreach(var item in _newsData)
        {
            item.Diff = item.SentimentPositive - item.SentimentNegative + (0.000001 * rand.NextDouble());
        }

        StateHasChanged();
    }

    string GetHeader(object obj)
    {
        //return ((Article)article).Title;
        double diff = (double)obj;

        var text = _newsData.FirstOrDefault(x =>  x.Diff == diff)?.Title ?? "";
        return text;
    }
}