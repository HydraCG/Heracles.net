using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes a deferencable link to another resource.</summary>
    public interface IDerefencableLink : IPointingResource
    {
        /// <summary>Gets a relation of the link.</summary>
        string Relation { get; }

        /// <summary>Gets a link's supported operations.</summary>
        [Collection("hydra", "supportedOperation")]
        ISet<IOperation> SupportedOperations { get; }
    }
}