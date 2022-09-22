using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal class TemplateUpdateService
{
    private readonly ILogger<TemplateUpdateService> _logger;

    public TemplateUpdateService(ILogger<TemplateUpdateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool TryUpdate(string template, string target, string content, out string result)
    {
        string startMark = $"<!-- start:{target} -->";
        string endMark = $"<!-- end:{target} -->";

        string pattern = $"({startMark})(.*?)({endMark})";
        Regex matcher = new Regex(pattern, RegexOptions.Singleline | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        _logger.LogInformation($"Looking for regex matching. Pattern: {pattern}");
        if (matcher.IsMatch(template))
        {
            _logger.LogTrace("Match succeeded.");
            result = matcher.Replace(template, m => $"{m.Groups[1].Value}{content}{m.Groups[3].Value}");
            return true;
        }
        result = template;
        return false;
    }
}