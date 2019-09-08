using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra collection.</summary>
    [Class("hydra", "Collection")]
    public interface ICollection : IHydraResource
    {
        /// <summary>Gets the collection's member resources.</summary>
        [Collection("hydra", "member")]
        ISet<IResource> Members { get; }

        /// <summary>Gets the statements defining the hydra:manages blocks.</summary>
        [Property("hydra", "manages")]
        ISet<IStatement> Manages { get; }
        
        /// <summary>Gets the total items in the collection.</summary>
        [Property("hydra", "totalItems")]
        int TotalItems { get; }

        /// <summary>Gets the optional partial collection view.</summary>
        [Property("hydra", "view")]
        IPartialCollectionView View { get; }
        
        /// <summary>Gets a partial collection iterator associated in case it is a partial one.</summary>
        /// <returns>Instance of the <see cref="IPartialCollectionIterator" />.</returns>
        IPartialCollectionIterator GetIterator();
    }
}
