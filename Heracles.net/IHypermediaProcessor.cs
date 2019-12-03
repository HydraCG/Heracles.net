using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>
    /// Describes an abstract meta-data providing facility which translates from a raw <see cref="IResponse" />
    /// to an abstract data model.
    /// </summary>
    public interface IHypermediaProcessor
    {
        /// <summary>Gets supported media types.</summary>
        IEnumerable<string> SupportedMediaTypes { get; }
        
        /// <summary>Determines level of support of a this {@link IHypermediaProcessor} for given response.</summary>
        /// <param name="response">Response to check support for.</param>
        /// <returns>Support level of this processor.</returns>
        Level Supports(IResponse response);
        
        /// <summary>Parses a given raw response.</summary>
        /// <param name="response">Raw fetch response holding data to be parsed.</param>
        /// <param name="hydraClient">Hydra client.</param>
        /// <param name="options">Optional additional processing options.</param>
        /// <returns>Processed hypermedia with embedded response.</returns>
        Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            IHypermediaProcessingOptions options = null);

        /// <summary>Parses a given raw response.</summary>
        /// <param name="response">Raw fetch response holding data to be parsed.</param>
        /// <param name="hydraClient">Hydra client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Processed hypermedia with embedded response.</returns>
        Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            CancellationToken cancellationToken);

        /// <summary>Parses a given raw response.</summary>
        /// <param name="response">Raw fetch response holding data to be parsed.</param>
        /// <param name="hydraClient">Hydra client.</param>
        /// <param name="options">Optional additional processing options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Processed hypermedia with embedded response.</returns>
        Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            IHypermediaProcessingOptions options,
            CancellationToken cancellationToken);

        /// <summary>Serializes a given <paramref name="body" />.</summary>
        /// <param name="body">Resource to be serialized.</param>
        /// <returns>Stream with serizlied <paramref name="body" />.</returns>
        Task<Stream> Serialize(IResource body);

        /// <summary>Serializes a given <paramref name="body" />.</summary>
        /// <param name="body">Resource to be serialized.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Stream with serizlied <paramref name="body" />.</returns>
        Task<Stream> Serialize(IResource body, CancellationToken cancellationToken);
    }
}