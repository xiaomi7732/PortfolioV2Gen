namespace PortfolioGenExe.YouTube;

internal class YouTubeThumbnailGen : IGen
{
    private readonly ListItemGenFactory _itemGenFactory;

    public YouTubeThumbnailGen(ListItemGenFactory itemGenFactory)
    {
        _itemGenFactory = itemGenFactory ?? throw new ArgumentNullException(nameof(itemGenFactory));
    }

    // Always can gen
    public bool CanGen(DataMeta data)
    => data is not null
        && string.Equals(data.Type, AcceptType, StringComparison.Ordinal)
        && data.Data is not null
        && data.Data.Any();

    public string AcceptType
        => "YTThumbnail";

    public string Generate(DataMeta data)
    {
        string template = data.GetTemplate();
        ListItemGen itemGen = _itemGenFactory.Create(template, skipListItemTag: true);
        return itemGen.Generate(data.Default, data.Data.First());
    }
}