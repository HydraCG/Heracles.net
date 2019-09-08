namespace Heracles
{
    /// <summary>Describes <see cref="PartialCollectionCrawler.From(IPartialCollectionView)" /> crawling options.</summary>
    public interface ICrawlingOptions
    {
        /// <summary>The crawling direction.</summary>
        CrawlingDirection? Direction { get; }

        /// <summary>The limit of the members to retrieve.</summary>
        int? MemberLimit { get; }
      
        /// <summary>The limit of requests to make.</summary>
        int? RequestLimit { get; }
      
        /// <summary>
        /// Value indicating whether to rewind back to the beginning (or end) of the collection in case the starting
        /// point was not the first (or last) possible view.
        /// </summary>
        bool? Rewind { get; }
    }
}