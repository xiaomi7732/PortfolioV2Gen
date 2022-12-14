using System.Text;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

public class ListItemGen
{
    private readonly ILogger _logger;
    private readonly string _itemTemplate;
    private readonly bool _skipListItemTag;

    public ListItemGen(string itemTemplate, bool skipListItemTag, ILogger<ListItemGen> logger)
    {
        if (string.IsNullOrEmpty(itemTemplate))
        {
            throw new ArgumentException($"'{nameof(itemTemplate)}' cannot be null or empty.", nameof(itemTemplate));
        }

        _itemTemplate = itemTemplate;
        _skipListItemTag = skipListItemTag;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Generate(IDictionary<string, string> defaultValues, IDictionary<string, string> itemData)
    {
        IDictionary<string, string> merged = new Dictionary<string, string>(defaultValues);
        foreach (KeyValuePair<string, string> dataItem in itemData)
        {
            merged[dataItem.Key] = dataItem.Value;
        }
        _logger.LogDebug("Merged dict: {merged}", merged);

        StringBuilder builder = new StringBuilder();

        if (!_skipListItemTag)
        {
            builder.Append("<li class='searchable-item'>");
        }

        string itemTemplate = _itemTemplate;
        foreach (KeyValuePair<string, string> dataItem in merged)
        {
            itemTemplate = itemTemplate.Replace($"${dataItem.Key}$", dataItem.Value);
        }
        builder.Append(itemTemplate);

        if (!_skipListItemTag)
        {
            builder.Append("</li>");
        }

        return builder.ToString();
    }
}