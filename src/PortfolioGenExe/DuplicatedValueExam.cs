namespace PortfolioGenExe;

internal class DuplicatedValueExam : IExam
{
    /// <summary>
    /// Checks if there's duplicated values in the list.
    /// </summary>
    /// <param name="Pass"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public (bool Pass, string? Reason) Execute(DataMeta data)
    {
        string[] values = data.Data.SelectMany(d => d.Values).ToArray();
        string[] distinctValues = values.Distinct().ToArray();
        if (values.Length == distinctValues.Length)
        {
            return (true, null);
        }
        IEnumerable<string> duplicatedItems = values.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key + "::" + g.Count().ToString());
        return (false, string.Join(',', duplicatedItems));
    }
}