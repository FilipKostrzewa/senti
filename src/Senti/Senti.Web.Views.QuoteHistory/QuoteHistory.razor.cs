using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.CandlestickLib;
using Senti.Shared.Models.Quotes;
using Senti.Web.Shared;
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
    string stock = "AAPL";
    DateTime pickedDate = new DateTime(2025, 02, 05, 0, 0, 0, kind: DateTimeKind.Utc);

    private PlotlyChart _chart;
    private Config _config;
    private Layout _layout;

    private List<Quote> _quotes = new List<Quote>();
    private IList<ITrace> _chartData = new List<ITrace>{ new Candlestick() };

    List<string> stocks = new List<string> { "AAPL", "MSFT", "GOOGL", "NVDA", "AMZN", "TSLA", "MCD" };

    public async Task OnStockChange(object args)
    {
        stock = args?.ToString() ?? stocks[0];

        SetChartData();
        await _chart.NewPlot();

        StateHasChanged();
    }

    public async Task OnDateChange(object args)
    {
        pickedDate = (args as DateTime?) ?? 
            new DateTime(2025, 02, 05, 0, 0, 0, kind: DateTimeKind.Utc);

        SetChartData();
        await _chart.NewPlot();

        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        InitChart();
        await InitQuotes();
        SetChartData();

        await base.OnInitializedAsync();
    }

    private void SetChartData()
    {
        var quotes = _quotes
            .Where(x => x.Time > pickedDate &&  x.Time < pickedDate.AddDays(2))
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

    private async Task InitQuotes()
    {
        var raw = await GetQuotes();

        _quotes = raw.Select(x => new Quote
        {
            Time = DateTimeOffset.FromUnixTimeMilliseconds(x.t).UtcDateTime,

            Open = x.o,
            Close = x.c,

            High = x.h,
            Low = x.l,

        }).ToList();
    }

    private async Task<List<RawQuote>> GetQuotes()
    {
        var quoteFileNames = GetQuoteFileNames();
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

    private List<string> GetQuoteFileNames()
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
        _config = new Config
        {
            // Responsive = true,
            // AutoSizable = true,
            // ScrollZoom = Plotly.Blazor.ConfigLib.ScrollZoomFlag.,

        };

        _layout = new Layout
        {
            // Title = new Plotly.Blazor.LayoutLib.Title
            // {
            //     Text = GetType().Name
            // },

            //PaperBgColor = Theme.PaletteDark.Surface.ToString(),
            //PlotBgColor = Theme.PaletteDark.Surface.ToString(),
            // Font = new Font
            // {
            //     //Color = Theme.PaletteDark.TextPrimary.ToString()
            // },

            // DragMode = DragModeEnum.Pan,
            // Margin = new Margin
            // {
            //     R = 10,
            //     T = 10,
            //     B = 10,
            //     L = 10
            // },
            // ShowLegend = true,
            XAxis = new List<XAxis>
        {
            new()
            {
                //AutoRange = AutoRangeEnum.True,
                //Domain = new List<object> { 0, 1 },
                //Range = new List<object> { "2025-01-01 09:00", "2025-01-05 23:00"},
                //FixedRange = new List<object> { "2025-01-01 09:00", "2025-01-05 23:00"},
                // RangeSlider = new RangeSlider
                // {
                //     AutoRange = true,
                //     //Range = new object[] { "2025-01-01 01:00", "2025-02-01 01:00" }
                // },
                // Title = new Plotly.Blazor.LayoutLib.XAxisLib.Title
                // {
                //     Text = "Date"
                // },

                RangeSlider = new RangeSlider
                {
                    Visible = false,
                },


                Type = TypeEnum.Category,


            }
        },
            YAxis = new List<YAxis>
            {
                // new()
                // {
                //     AutoRange = Plotly.Blazor.LayoutLib.YAxisLib.AutoRangeEnum.True,
                //     Domain = new List<object> { 0, 1 },
                //     //Range = new List<object> { 200, 500 },

                //     //Type = Plotly.Blazor.LayoutLib.YAxisLib.TypeEnum.Linear,
                //     Type = Plotly.Blazor.LayoutLib.YAxisLib.TypeEnum.Linear,

                //     ShowGrid = true,


                // }
            }
        };
    }
}