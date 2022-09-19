using System.Text;

namespace PortfolioGenExe;

internal class ListGen : IGen
{
    const string AcceptType = "list";
    private readonly ListItemGenFactory _listItemGenFactory;

    public ListGen(ListItemGenFactory listItemGenFactory)
    {
        _listItemGenFactory = listItemGenFactory ?? throw new ArgumentNullException(nameof(listItemGenFactory));
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
            ListItemGen itemGen = _listItemGenFactory.Create(data.ItemTemplate);
            htmlBuilder.Append(itemGen.Generate(data.Default, dataItem));
        }
        htmlBuilder.Append("</ul>");

        return htmlBuilder.ToString();
    }
}