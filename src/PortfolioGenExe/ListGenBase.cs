using System.Text;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe;

internal abstract class ListGenBase : IGen
{
    protected abstract string AcceptType { get; }
    protected ILogger _logger { get; }
    private readonly IEnumerable<IExam> _exams;

    public ListGenBase(
        IEnumerable<IExam> exams,
        ILogger<ListGenBase> logger
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        string templateContent = data.GetTemplate();
        ListItemGen itemGen = CreateListItemGen(templateContent);

        StringBuilder htmlBuilder = new StringBuilder();
        OnGeneratingItems(htmlBuilder);
        foreach (IDictionary<string, string> dataItem in data.Data)
        {
            htmlBuilder.Append(itemGen.Generate(data.Default, dataItem));
        }
        OnGeneratedItems(htmlBuilder);

        return htmlBuilder.ToString();
    }

    protected abstract ListItemGen CreateListItemGen(string templateContent);

    protected abstract void OnGeneratingItems(StringBuilder builder);

    protected abstract void OnGeneratedItems(StringBuilder builder);
}