using System.Text;

namespace PortfolioGenExe;

internal class ListGen : IGen
{
    const string AcceptType = "list";
    private readonly ListItemGen _listItemGen;

    public ListGen(ListItemGen listItemGen)
    {
        _listItemGen = listItemGen ?? throw new ArgumentNullException(nameof(listItemGen));
    }


    public bool CanGen(DataMeta data)
     => data is not null
        && string.Equals(data.Type, AcceptType, StringComparison.Ordinal)
        && data.Data is not null
        && data.Data.Any();

    public string Generate(DataMeta data)
    {
        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.Append("<ul>");
        foreach (IDictionary<string, string> dataItem in data.Data)
        {
            htmlBuilder.Append(_listItemGen.Generate(data.ItemTemplate, data.Default, dataItem));
        }
        htmlBuilder.Append("</ul>");

        return htmlBuilder.ToString();
    }
}