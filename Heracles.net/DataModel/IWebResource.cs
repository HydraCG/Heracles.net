namespace Heracles.DataModel
{
    /// <summary>Describes an abstract web resource.</summary>
    public interface IWebResource : IResource
    {
        /// <summary>Gets a collection of hypermedia controls.</summary>
        IHypermediaContainer Hypermedia { get; }
    }
}