using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Describes <see cref="PartialCollectionCrawler.From(ICollection)" /> crawling options.</summary>
    public interface ICrawlingOptions
    {
        /// <summary>Gets a crawling direction.</summary>
        CrawlingDirection? Direction { get; }

        /// <summary>Gets a limit of the members to retrieve.</summary>
        int? MemberLimit { get; }
      
        /// <summary>Gets a limit of requests to make.</summary>
        int? RequestLimit { get; }
      
        /// <summary>
        /// Gets a value indicating whether to rewind back to the beginning (or end) of the collection in case the starting
        /// point was not the first (or last) possible view.
        /// </summary>
        bool? Rewind { get; }
    }
}