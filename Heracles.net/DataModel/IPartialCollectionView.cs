using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract partial collection view with links to other collection parts.</summary>
    [Class("hydra", "PartialCollectionView")]
    public interface IPartialCollectionView : IResource
    {
        /// <summary>Gets the link to the first part of the collection, if any.</summary>
        [Property("hydra", "first")]
        IResource First { get; }

        /// <summary>Gets the link to the next part of the collection, if any.</summary>
        [Property("hydra", "next")]
        IResource Next { get; }

        /// <summary>Gets the link to the previous part of the collection, if any.</summary>
        [Property("hydra", "previous")]
        IResource Previous { get; }

        /// <summary>Gets the link to the last part of the collection, if any.</summary>
        [Property("hydra", "last")]
        IResource Last { get; }
    }
}