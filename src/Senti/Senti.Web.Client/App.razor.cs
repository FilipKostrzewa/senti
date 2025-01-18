using System.Reflection;

namespace Senti.Web.Client;
public partial class App
{
    private readonly Assembly _appAssembly = typeof(App).Assembly;

    private readonly Assembly[] _additionalAssemblies = new Assembly[]
    {
        typeof(Senti.Web.Views.Headlines.Headlines).Assembly,
        typeof(Senti.Web.Views.Timelines.Timeline).Assembly,
        typeof(Senti.Web.Views.Charts.Chart1).Assembly,
    };
}