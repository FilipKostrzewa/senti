using System.Reflection;

namespace Senti.Web.Client;
public partial class App
{
    private readonly Assembly _appAssembly = typeof(App).Assembly;

    private readonly Assembly[] _additionalAssemblies = new Assembly[]
    {
        typeof(Senti.Web.Views.Headlines.Headlines).Assembly,
        typeof(Senti.Web.Views.Timelines.Timeline).Assembly,
        typeof(Senti.Web.Views.PlusMinuses.PlusMinus).Assembly,
        typeof(Senti.Web.Views.Charts.Chart1).Assembly,
        typeof(Senti.Web.Views.PlotlyCharts.CandleSticks).Assembly,
        typeof(Senti.Web.Views.Colors.ColorButtons).Assembly,
        typeof(Senti.Web.Views.Syslogs.Syslog).Assembly,
        typeof(Senti.Web.Views.PyCandle2.Candle2).Assembly,
    };
}