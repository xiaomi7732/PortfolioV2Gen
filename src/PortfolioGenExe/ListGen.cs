using System.Text;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal class ListGen : ListGenBase
{
    private readonly ListItemGenFactory _listItemGenFactory;

    public ListGen(
        ListItemGenFactory listItemGenFactory,
        IEnumerable<IExam> exams,
        ILogger<ListGen> logger
        ) : base(exams, logger)
    {
        _listItemGenFactory = listItemGenFactory ?? throw new ArgumentNullException(nameof(listItemGenFactory));
    }

    protected override string AcceptType => "list";

    protected override ListItemGen CreateListItemGen(string templateContent)
    {
        return _listItemGenFactory.Create(templateContent, skipListItemTag: false);
    }

    protected override void OnGeneratedItems(StringBuilder builder)
    {
        builder.Append("</ul>");
    }

    protected override void OnGeneratingItems(StringBuilder builder)
    {
        builder.Append("<ul>");
    }
}