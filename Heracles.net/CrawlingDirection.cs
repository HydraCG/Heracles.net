namespace Heracles
{
    /// <summary>Defines possible partial collection view crawling directions.</summary>
    public enum CrawlingDirection
    {
        /// <summary>Defines a forward direction that consumes hydra:next link.</summary>
        Forward,
    
        /// <summary>Defines a forward direction that consumes hydra:previous link.</summary>
        Backward
    }
}