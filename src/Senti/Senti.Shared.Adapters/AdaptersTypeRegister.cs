using Microsoft.Extensions.DependencyInjection;
using Senti.Shared.Adapters.Storages;

namespace Senti.Shared.Adapters;
public static class AdaptersTypeRegister
{
    public static void Register(this IServiceCollection services)
    {
        services.AddTransient<StorageAdapter>();
        services.AddTransient<LogToStorage>();
    }
}
