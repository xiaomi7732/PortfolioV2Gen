using System.Text;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal class ListGen : IGen
{
    const string AcceptType = "list";
    private readonly ListItemGenFactory _listItemGenFactory;
    private readonly IEnumerable<IExam> _exams;
    private readonly ILogger<ListGen> _logger;

    public ListGen(
        ListItemGenFactory listItemGenFactory,
        IEnumerable<IExam> exams,
        ILogger<ListGen> logger
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listItemGenFactory = listItemGenFactory ?? throw new ArgumentNullException(nameof(listItemGenFactory));
        _exams = exams ?? throw new ArgumentNullException(nameof(exams));
    }


    public bool CanGen(DataMeta data)
     => data is not null
        && string.Equals(data.Type, AcceptType, StringComparison.Ordinal)
        && data.Data is not null
        && data.Data.Any();

    public string Generate(DataMeta data)
    {
        foreach (IExam exam in _exams)
        {
            (bool pass, string? details) = exam.Execute(data);
            if (!pass)
            {
                _logger.LogWarning("Failed examination: {examType}. Details: {failedDetails}", exam.GetType().Name, details);
            }
        }

        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<ul>");
        foreach (IDictionary<string, string> dataItem in data.Data)
        {
            ListItemGen itemGen = _listItemGenFactory.Create(data.ItemTemplate);
            htmlBuilder.Append(itemGen.Generate(data.Default, dataItem));
        }
        htmlBuilder.Append("</ul>");

        return htmlBuilder.ToString();
    }
}