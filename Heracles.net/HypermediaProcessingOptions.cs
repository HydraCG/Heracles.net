using System;
using System.Collections.Generic;
using System.Linq;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Provides a default implementation of the <see cref="IHypermediaProcessingOptions" />.</summary>
    public class HypermediaProcessingOptions : IHypermediaProcessingOptions
    {
        private readonly LinksPolicy? _linksPolicy;
        private readonly ApiDocumentationPolicy? _apiDocumentationPolicy;

        /// <summary>Initializes a new instance of the <see cref="HypermediaProcessingOptions" /> class.</summary>
        /// <param name="originalUrl">Original Url.</param>
        /// <param name="linksPolicy">Links policy.</param>
        /// <param name="apiDocumentationPolicy">API documentation policy.</param>
        /// <param name="apiDocumentations">API documentations.</param>
        public HypermediaProcessingOptions(
            Uri originalUrl = null,
            LinksPolicy linksPolicy = LinksPolicy.Strict,
            ApiDocumentationPolicy apiDocumentationPolicy = ApiDocumentationPolicy.None,
            IEnumerable<IApiDocumentation> apiDocumentations = null)
            : this(null, null, originalUrl, linksPolicy, apiDocumentationPolicy, apiDocumentations)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HypermediaProcessingOptions" /> class.</summary>
        /// <param name="auxiliaryResponse">Auxiliar response.</param>
        /// <param name="auxiliaryOriginalUrl">Original url of the <paramref name="auxiliaryResponse"/>.</param>
        /// <param name="originalUrl">Original Url.</param>
        /// <param name="linksPolicy">Links policy.</param>
        /// <param name="apiDocumentationPolicy">API documentation policy.</param>
        /// <param name="apiDocumentations">API documentations.</param>
        internal HypermediaProcessingOptions(
            IResponse auxiliaryResponse = null,
            Uri auxiliaryOriginalUrl = null,
            Uri originalUrl = null,
            LinksPolicy? linksPolicy = null,
            ApiDocumentationPolicy? apiDocumentationPolicy = null,
            IEnumerable<IApiDocumentation> apiDocumentations = null)
        {
            OriginalUrl = originalUrl;
            _linksPolicy = linksPolicy;
            _apiDocumentationPolicy = apiDocumentationPolicy;
            ApiDocumentations = apiDocumentations ?? Array.Empty<IApiDocumentation>();
            AuxiliaryResponse = auxiliaryResponse;
            AuxiliaryOriginalUrl = auxiliaryOriginalUrl;
        }

        /// <inheritdoc />
        public LinksPolicy LinksPolicy
        {
            get { return _linksPolicy ?? Heracles.LinksPolicy.Strict; }
        }

        /// <inheritdoc />
        public ApiDocumentationPolicy ApiDocumentationPolicy
        {
            get { return _apiDocumentationPolicy ?? ApiDocumentationPolicy.None; }
        }
        
        /// <inheritdoc />
        public IEnumerable<IApiDocumentation> ApiDocumentations { get; }
        
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
            var apiDocumentations = new List<IApiDocumentation>();
            foreach (var apiDocumentation in ApiDocumentations)
            {
                apiDocumentations.Add(apiDocumentation);
            }

            if (options != null)
            {
                foreach (var apiDocumentation in options.ApiDocumentations)
                {
                    if (apiDocumentations.All(_ => _.Iri != apiDocumentation.Iri))
                    {
                        apiDocumentations.Add(apiDocumentation);
                    }
                }
            }

            return new HypermediaProcessingOptions(
                AuxiliaryResponse ?? options?.AuxiliaryResponse,
                AuxiliaryOriginalUrl ?? options?.AuxiliaryOriginalUrl,
                OriginalUrl ?? options?.OriginalUrl,
                _linksPolicy ?? options?.LinksPolicy,
                _apiDocumentationPolicy ?? options?.ApiDocumentationPolicy,
                apiDocumentations);
        }
    }
}