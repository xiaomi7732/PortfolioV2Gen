using System.Text;

namespace PortfolioGenExe;

public class ListItemGen
{
    public string Generate(string itemTemplate, IDictionary<string, string> defaultValues, IDictionary<string, string> itemData)
    {
        IDictionary<string, string> merged = new Dictionary<string, string>(defaultValues);
        foreach (KeyValuePair<string, string> dataItem in itemData)
        {
            merged[dataItem.Key] = dataItem.Value;
        }

        StringBuilder builder = new StringBuilder();
        builder.Append("<li>");
        foreach (KeyValuePair<string, string> dataItem in merged)
        {
            itemTemplate = itemTemplate.Replace($"${dataItem.Key}$", dataItem.Value);
        }
        builder.Append(itemTemplate);
        builder.Append("</li>");

        return builder.ToString();
    }
}