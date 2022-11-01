namespace PortfolioGenExe;

internal static class DataMetaExtensions
{
    public static string GetTemplate(this DataMeta data)
    {
        if (!string.IsNullOrEmpty(data.ItemTemplate))
        {
            return data.ItemTemplate;
        }

        string templateFilePath = string.Empty;
        if (!string.IsNullOrEmpty(data.ItemTemplatePath))
        {
            templateFilePath = Path.Combine("Data", data.ItemTemplatePath);
        }
        else
        {
            throw new InvalidOperationException($"At least one of the properties has to have a value: {nameof(data.ItemTemplate)}, {nameof(data.ItemTemplatePath)}");
        }

        if (!File.Exists(templateFilePath))
        {
            throw new FileNotFoundException($"File not found. Relative Path: {templateFilePath}. Full Path: {Path.GetFullPath(templateFilePath)}");
        }

        return File.ReadAllText(templateFilePath);
    }
}