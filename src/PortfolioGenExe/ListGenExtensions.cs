using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioGenExe.YouTube;

namespace PortfolioGenExe;

internal static class ListGenExtensions
{
    public static ServiceCollection TryAddListGenServices(this ServiceCollection services)
    {
        services.TryAddSingleton<ListItemGenFactory>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGen, ListGen>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGen, RepeatGen>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGen, YouTubeThumbnailGen>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IExam, DuplicatedValueExam>());

        return services;
    }
}