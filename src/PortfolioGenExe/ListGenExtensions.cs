using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PortfolioGenExe;

internal static class ListGenExtensions
{
    public static ServiceCollection TryAddListGenServices(this ServiceCollection services)
    {
        services.TryAddSingleton<ListItemGenFactory>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGen, ListGen>());

        return services;
    }
}