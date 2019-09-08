using System;
using System.Collections.Generic;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstraction layer over hypermedia container.</summary>
    public interface IHypermediaContainer : IEnumerable<IResource>, IHydraResource, IResponse
    {
        /// <summary>Gets a collection members.</summary>
        /// <remarks>
        /// This may be <i>null</i> if the resource owning this container is not a <i>hydra:Collection</i>.
        /// </remarks>
        ISet<IResource> Members { get; }
        
        /// <summary>Gets a partial collection view.</summary>
        /// <remarks>
        /// This may be <i>null</i> if the resource owning this container is not
        /// a <i>hydra:Collection</i> with <i>hydra:view</i>.
        /// </remarks>
        IPartialCollectionView View { get; }

        /// <summary>Gets a part iterator associated with the collection.</summary>
        /// <remarks>
        /// This may throw <see cref="NotImplementedException" /> if the resource owning this container is not
        /// a <i>hydra:Collection</i> with <i>hydra:view</i>.
        /// </remarks>
        /// <returns>Instance of the <see cref="IPartialCollectionIterator" />.</returns>
        IPartialCollectionIterator GetIterator();
    }
}
