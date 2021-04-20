using System.Collections.Generic;
using RDeF.Entities;

namespace Heracles.Rdf.GraphTransformations
{
    /// <summary>Describes an abstract resource transforming facility.</summary>
    public interface IGraphTransformer
    {
        /// <summary>Tranforms given resources.</summary>
        /// <param name="resources">Resources to be transformed.</param>
        /// <param name="processor">Hypermedia processor requesting a resource transformation.</param>
        /// <param name="options">Additional processing options.</param>
        /// <returns>Transformed resources.</returns>
        IEnumerable<ITypedEntity> Transform(
            IEnumerable<ITypedEntity> resources,
            IHypermediaProcessor processor,
            IHypermediaProcessingOptions options = null);
    }
}