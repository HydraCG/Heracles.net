using System;
using System.Collections.Generic;
using RDeF.Entities;

namespace Heracles.Rdf.GraphTransformations
{
    /// <summary>Provides a collective wrapper over multiple <see cref="IGraphTransformer" />s.</summary>
    public class CompoundGraphTransformer : IGraphTransformer
    {
        private readonly IEnumerable<IGraphTransformer> _graphTransformers;

        /// <summary>Initializes a new instance of the <see cref="CompoundGraphTransformer" /> class.</summary>
        /// <param name="graphTransformers">Other graph transforming facilities to use.</param>
        public CompoundGraphTransformer(IEnumerable<IGraphTransformer> graphTransformers)
        {
            _graphTransformers = graphTransformers ?? throw new ArgumentNullException(nameof(graphTransformers));
        }
        
        /// <inheritdoc />
        public IEnumerable<ITypedEntity> Transform(
            IEnumerable<ITypedEntity> resources,
            IHypermediaProcessor processor,
            IHypermediaProcessingOptions options = null)
        {
            var result = resources;
            foreach (var graphTransformer in _graphTransformers)
            {
                result = graphTransformer.Transform(result, processor, options);
            }

            return result;
        }
    }
}