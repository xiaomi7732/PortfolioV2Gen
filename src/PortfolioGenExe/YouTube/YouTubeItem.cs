using System.Xml.Linq;

namespace PortfolioGenExe.YouTube;

public record YouTubeItem(string Title, Uri LinkToVideo, DateTime PublishUTCDateTime, Uri ThumbnailUri);

internal static class YouTubeItemExtensions
{
    public static YouTubeItem CreateYouTubeItem(this XElement entry)
    {
        const string atomNS = @"{http://www.w3.org/2005/Atom}";
        const string mediaNS = @"{http://search.yahoo.com/mrss/}";
        XElement? titleElement = entry.Element($"{atomNS}title");
        if (titleElement is null)
        {
            throw new InvalidCastException("Title element doesn't exist");
        }
        string title = titleElement.Value;

        XElement? linkElement = entry.Element($"{atomNS}link");
        if (linkElement is null)
        {
            throw new InvalidCastException("Link element doesn't exist");
        }
        XAttribute? hrefAttribute = linkElement.Attribute("href");
        if (hrefAttribute is null)
        {
            throw new InvalidCastException("Link element has no href attribute.");
        }
        Uri link = new Uri(hrefAttribute.Value);

        XElement? publishedElement = entry.Element($"{atomNS}published");
        if (publishedElement is null)
        {
            throw new InvalidCastException("Published element doesn't exist.");
        }
        DateTimeOffset publishedWithOffset = DateTimeOffset.Parse(publishedElement.Value);
        DateTime published = publishedWithOffset.UtcDateTime;

        XElement? mediaGroup = entry.Element($"{mediaNS}group");
        if (mediaGroup is null)
        {
            throw new InvalidCastException("Media:group element doesn't exist.");
        }
        XElement? thumbnailElement = mediaGroup.Element($"{mediaNS}thumbnail");
        if (thumbnailElement is null)
        {
            throw new InvalidCastException("Media group missing required element of thumbnail.");
        }
        XAttribute? thumbnailUrlAttribute = thumbnailElement.Attribute("url");
        if (thumbnailUrlAttribute is null)
        {
            throw new InvalidCastException("Thumbnail element missing required attribute url");
        }
        Uri thumbnailUri = new Uri(thumbnailUrlAttribute.Value);

        return new YouTubeItem(title, link, published, thumbnailUri);
    }
}