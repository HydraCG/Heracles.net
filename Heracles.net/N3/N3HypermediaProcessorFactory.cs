using Heracles.Rdf.GraphTransformations;

namespace Heracles.N3
{
    /// <summary>Provides a factory method to be registered with Hydra client factory.</summary>
    public class N3HypermediaProcessorFactory
    {
        /// <summary>Provides instances of the <see cref="N3HypermediaProcessor" />s.</summary>
        /// <param name="context">Resolution context.</param>
        /// <returns>Instance of the <see cref="N3HypermediaProcessor" /> class.</returns>
        public static IHypermediaProcessor Instance(IHydraClientFactory context)
        {
            var graphTransformer = new CompoundGraphTransformer(new[] { new EntryPointCorrectingGraphTransformer() });
            return new N3HypermediaProcessor(context.OntologyProvider, context.CurrentHttpCall, graphTransformer);
        }
    }
}
