using System.Text;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal class RepeatGen : ListGenBase
{
    private readonly ListItemGenFactory _listItemGenFactory;

    public RepeatGen(
        ListItemGenFactory listItemGenFactory,
        IEnumerable<IExam> exams,
        ILogger<RepeatGen> logger) : base(exams, logger)
    {
        _listItemGenFactory = listItemGenFactory ?? throw new ArgumentNullException(nameof(listItemGenFactory));
    }

    protected override string AcceptType => "repeat";

    protected override ListItemGen CreateListItemGen(string templateContent)
        => _listItemGenFactory.Create(templateContent, skipListItemTag: true);

    protected override void OnGeneratedItems(StringBuilder builder)
    {
        // Do nothing
    }

    protected override void OnGeneratingItems(StringBuilder builder)
    {
        // Do nothing
    }
}