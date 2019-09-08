using System;

namespace Heracles
{
    /// <summary>Provides a default implementation of the <see cref="IHypermediaProcessingOptions" />.</summary>
    public class HypermediaProcessingOptions : IHypermediaProcessingOptions
    {
        private readonly LinksPolicy? _linksPolicy;
        
        /// <summary>Initializes a new instance of the <see cref="HypermediaProcessingOptions" /> class.</summary>
        /// <param name="originalUrl">Original Url.</param>
        /// <param name="linksPolicy">Links policy.</param>
        public HypermediaProcessingOptions(
            Uri originalUrl = null,
            LinksPolicy linksPolicy = LinksPolicy.Strict) : this(null, null, originalUrl, linksPolicy)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HypermediaProcessingOptions" /> class.</summary>
        /// <param name="auxiliaryResponse">Auxiliar response.</param>
        /// <param name="auxiliaryOriginalUrl">Original url of the <paramref name="auxiliaryResponse"/>.</param>
        /// <param name="originalUrl">Original Url.</param>
        /// <param name="linksPolicy">Links policy.</param>
        public HypermediaProcessingOptions(
            IResponse auxiliaryResponse = null,
            Uri auxiliaryOriginalUrl = null,
            Uri originalUrl = null,
            LinksPolicy? linksPolicy = null)
        {
            OriginalUrl = originalUrl;
            _linksPolicy = linksPolicy;
            AuxiliaryResponse = auxiliaryResponse;
            AuxiliaryOriginalUrl = auxiliaryOriginalUrl;
        }

        /// <inheritdoc />
        public LinksPolicy LinksPolicy
        {
            get { return _linksPolicy ?? LinksPolicy.Strict; }
        }
        
        /// <inheritdoc />
        public Uri OriginalUrl { get; }
        
        /// <inheritdoc />
        public IResponse AuxiliaryResponse { get; }
        
        /// <inheritdoc />
        public Uri AuxiliaryOriginalUrl { get; }

        /// <summary>Merges options.</summary>
        /// <param name="options">Options to merge with.</param>
        /// <returns>Merged options where values are taken first from this instance.</returns>
        public HypermediaProcessingOptions MergeWith(IHypermediaProcessingOptions options)
        {
            return new HypermediaProcessingOptions(
                AuxiliaryResponse ?? options?.AuxiliaryResponse,
                AuxiliaryOriginalUrl ?? options?.AuxiliaryOriginalUrl,
                OriginalUrl ?? options?.OriginalUrl,
                _linksPolicy ?? options?.LinksPolicy);
        }
    }
}