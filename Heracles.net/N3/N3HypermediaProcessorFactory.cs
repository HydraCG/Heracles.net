namespace Heracles.N3
{
    /// <summary>Provides a factory method to be registered with Hydra client factory.</summary>
    public class N3HypermediaProcessorFactory
    {
        /// <summary>Provides instances of the <see cref="N3HypermediaProcessor" />s.</summary>
        public static IHypermediaProcessor Instance(IHydraClientFactory context)
        {
            return new N3HypermediaProcessor(context.OntologyProvider, context.CurrentHttpCall);
        }
    }
}
