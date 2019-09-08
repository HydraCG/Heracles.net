namespace Heracles
{
    /// <summary>Provides an abstraction over facility used for creating Hydra clients.</summary>
    public interface IHydraClientFactory
    {
        /// <summary>Gets a currently configured facility for HTTP communication.</summary>
        HttpCallFacility CurrentHttpCall { get; }

        /// <summary>Gets a currently configured <see cref="LinksPolicy" />.</summary>
        LinksPolicy CurrentLinksPolicy { get; }

        /// <summary>
        /// Creates an instance of the <see cref="IHypermediaProcessor" />
        /// that will be capable for working with given media type.
        /// </summary>
        /// <remarks>In case no processors are registered, an exception will be thrown.</remarks>
        /// <param name="mediaType">Media type of the RDF serialization to handle by the processor.</param>
        /// <returns>
        /// Instance of the <see cref="IHypermediaProcessor" />
        /// capable of handling a given <paramref name="mediaType" />.
        /// </returns>
        IHypermediaProcessor CreateProcessorToHandle(string mediaType);
    }
}
