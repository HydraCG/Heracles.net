using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra resource.</summary>
    [Class("hydra", "Resource")]
    public interface IHydraResource : IResource
    {
        /// <summary>Gets collections exposed by that resource.</summary>
        [Collection("hydra", "collection")]
        ISet<ICollection> Collections { get; }
        
        /// <summary>Gets operations that can be performed on that resource.</summary>
        [Collection("hydra", "operation")]
        ISet<IOperation> Operations { get; }
        
        /// <summary>Gets links related to that resource.</summary>
        ISet<IDerefencableLink> Links { get; }
    }
}
