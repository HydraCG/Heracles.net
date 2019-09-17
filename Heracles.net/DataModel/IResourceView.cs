using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstraction over a resouce that has a view.</summary>
    public interface IResourceView : IHydraResource
    {
        /// <summary>Gets the optional partial collection view.</summary>
        [Property("hydra", "view")]
        IPartialCollectionView View { get; }
    }
}
