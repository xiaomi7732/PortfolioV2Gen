using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace PortfolioGenExe.YouTube;

public class YouTubeFeedReader
{
    private const string YouTubeFeedUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=UCFVGdkhRh174GKg9gVEhY6A";
    private const string AtomNS = @"{http://www.w3.org/2005/Atom}";
    public async IAsyncEnumerable<YouTubeItem> GetYouTubeItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int count = 0;
        const int maxItemCount = 5;

        using (XmlReader reader = XmlReader.Create(YouTubeFeedUrl, new XmlReaderSettings() { Async = true }))
        {
            XElement feedElement = await XElement.LoadAsync(reader, LoadOptions.None, cancellationToken);

            foreach (XElement entryElement in feedElement.Elements($"{AtomNS}entry"))
            {
                yield return entryElement.CreateYouTubeItem();
                if (++count == maxItemCount)
                {
                    yield break;
                }
            }
        }
    }
}