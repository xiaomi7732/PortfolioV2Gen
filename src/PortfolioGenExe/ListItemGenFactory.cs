using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal class ListItemGenFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public ListItemGenFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public ListItemGen Create(string itemTemplate, bool skipListItemTag = false)
    {
        return new ListItemGen(itemTemplate, skipListItemTag, _loggerFactory.CreateLogger<ListItemGen>());
    }
}