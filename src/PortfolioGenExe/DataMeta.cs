namespace PortfolioGenExe;

public class DataMeta
{
    public string Target { get; set; } = default!;

    public string Type { get; set; } = default!;

    public string ItemTemplate { get; set; } = default!;

    public IDictionary<string, string> Default { get; set; } = default!;

    public IEnumerable<IDictionary<string, string>> Data { get; set; } = default!;
}