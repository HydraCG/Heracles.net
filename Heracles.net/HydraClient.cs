using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;

namespace Heracles
{
    /// <summary>HydraClient, also known as Heracles.ts, is a generic client for Hydra-powered Web APIs.</summary>
    /// <remarks>
    /// To learn more about Hydra please refer to
    /// <a href="https://www.hydra-cg.com/spec/latest/core/">Hydra Core Vocabulary</a>.
    /// </remarks>
    public class HydraClient : IHydraClient
    {
        internal const string ContentType = "Content-Type";
        internal const string NoUrlProvided = "There was no Url provided.";
        internal const string NoHypermediaProcessors = "No valid hypermedia processor instances were provided.";
        private const string NoOperationProvided = "There was no operation provided.";
        private const string NoIriTemplateExpansionStrategy = "No IRI template expansion strategy was provided.";
        private const string NoHttpFacility = "No HTTP facility provided.";

        private static readonly Regex LinkHeaderPattern = new Regex($"<([^>]+)>; rel=\"{hydra.apiDocumentation}\"");

        private readonly IEnumerable<IHypermediaProcessor> _hypermediaProcessors;
        private readonly IIriTemplateExpansionStrategy _iriTemplateExpansionStrategy;
        private readonly HttpCallFacility _httpCall;
        private readonly IResourceCache _resourceCache;

        /// <summary>Initializes a new instance of the <see cref="HydraClient" /> class.</summary>
        /// <param name="hypermediaProcessors">
        /// Hypermedia processors used for response hypermedia controls extraction.
        /// </param>
        /// <param name="iriTemplateExpansionStrategy">IRI template variable expansion strategy.</param>
        /// <param name="linksPolicy">Policy defining what is a considered a link.</param>
        /// <param name="apiDocumentationPolicy">Policy defining on how to treat API documentation.</param>
        /// <param name="httpCall">HTTP facility used to call remote server.</param>
        /// <param name="resourceCache">Cache used for storing API documentation fetched.</param>
        public HydraClient(
            IEnumerable<IHypermediaProcessor> hypermediaProcessors,
            IIriTemplateExpansionStrategy iriTemplateExpansionStrategy,
            LinksPolicy linksPolicy,
            ApiDocumentationPolicy apiDocumentationPolicy,
            HttpCallFacility httpCall,
            IResourceCache resourceCache)
        {
            _hypermediaProcessors = hypermediaProcessors ??
                throw new ArgumentNullException(nameof(hypermediaProcessors));
            if (!_hypermediaProcessors.Any())
            {
                throw new ArgumentOutOfRangeException(NoHypermediaProcessors);
            }
            
            _iriTemplateExpansionStrategy = iriTemplateExpansionStrategy ??
                throw new ArgumentNullException(nameof(iriTemplateExpansionStrategy), NoIriTemplateExpansionStrategy);
            _httpCall = httpCall ??
                throw new ArgumentNullException(nameof(httpCall), NoHttpFacility);
            LinksPolicy = linksPolicy;
            ApiDocumentationPolicy = apiDocumentationPolicy;
            _resourceCache = resourceCache ?? throw new ArgumentNullException(nameof(resourceCache));
        }

        internal LinksPolicy LinksPolicy { get; }
        
        internal ApiDocumentationPolicy ApiDocumentationPolicy { get; }

        /// <inheritdoc />
        public IHypermediaProcessor GetHypermediaProcessor(IResponse response)
        {
            return (
                from hypermediaProcessor in _hypermediaProcessors
                let supportLevel = hypermediaProcessor.Supports(response)
                where supportLevel > Level.None
                orderby supportLevel descending
                select hypermediaProcessor).FirstOrDefault();
        }

        /// <inheritdoc />
        public Task<IApiDocumentation> GetApiDocumentation(IResource resource)
        {
            return GetApiDocumentation(resource, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<IApiDocumentation> GetApiDocumentation(IResource resource, CancellationToken cancellationToken)
        {
            return GetApiDocumentation(resource?.Iri, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IApiDocumentation> GetApiDocumentation(Uri url)
        {
            return GetApiDocumentation(url, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<IApiDocumentation> GetApiDocumentation(Uri url, CancellationToken cancellationToken)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url), NoUrlProvided);
            }

            var response = await MakeRequestTo(url, null, cancellationToken);
            if (response.Status != 200)
            {
                throw new InvalidResponseException(response.Status);
            }

            return await GetApiDocumentation(response, url, true, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IHypermediaContainer> GetResource(IResource resource)
        {
            return GetResource(resource, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<IHypermediaContainer> GetResource(IResource resource, CancellationToken cancellationToken)
        {
            return GetResource((resource as IPointingResource)?.Target?.Iri ?? resource?.Iri, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IResponse> Invoke(IOperation operation, IResource body = null, IResource parameters = null)
        {
            return Invoke(operation, body, parameters, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<IResponse> Invoke(IOperation operation, CancellationToken cancellationToken)
        {
            return Invoke(operation, null, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IResponse> Invoke(IOperation operation, IResource body, CancellationToken cancellationToken)
        {
            return Invoke(operation, body, null, cancellationToken);
        }
        
        /// <inheritdoc />
        public async Task<IResponse> Invoke(IOperation operation, IResource body, IResource parameters, CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation), NoOperationProvided);
            }

            var targetOperation = _iriTemplateExpansionStrategy.CreateRequest(operation, body, parameters);
            Stream serializedBody = null;
            var headers = new Dictionary<string, string>() { { ContentType, _hypermediaProcessors.First().SupportedMediaTypes.First() } };
            if (body != null)
            {
                var mediaTypeCandidates = new Dictionary<string, int>() { { headers[ContentType], 0 } };
                if (!String.IsNullOrEmpty(operation.OriginatingMediaType))
                {
                    mediaTypeCandidates[operation.OriginatingMediaType] = Int32.MaxValue;
                }

                var hypermediaProcessor = (
                    from mediaType in mediaTypeCandidates
                    from processor in _hypermediaProcessors
                    from supportedMediaType in processor.SupportedMediaTypes
                    where StringComparer.InvariantCultureIgnoreCase.Equals(supportedMediaType, mediaType.Key)
                    orderby mediaType.Value descending 
                    select processor).First();
                serializedBody = await hypermediaProcessor.Serialize(body, cancellationToken);
            }

            return await MakeRequestTo(
                targetOperation.Target.Iri,
                new HttpOptions(targetOperation.Method, serializedBody, headers),
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<IHypermediaContainer> GetResource(Uri url)
        {
            return GetResource(url, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<IHypermediaContainer> GetResource(Uri url, CancellationToken cancellationToken)
        {
            return GetResourceFrom(url, null, cancellationToken);
        }

        private async Task<IApiDocumentation> GetApiDocumentation(
            IResponse response,
            Uri baseUrl,
            bool throwIfUnavailable,
            CancellationToken cancellationToken)
        {
            var apiDocumentationUrl = GetApiDocumentationUrl(response, baseUrl);
            if (apiDocumentationUrl == null)
            {
                return Assert<IApiDocumentation, ApiDocumentationNotProvidedException>(null, throwIfUnavailable);
            }

            IApiDocumentation result = _resourceCache[apiDocumentationUrl] as IApiDocumentation;
            if (result == null)
            {
                var options = new HypermediaProcessingOptions(response, baseUrl);
                var resource = await GetResourceFrom(apiDocumentationUrl, options, cancellationToken);
                result = resource.OfType(hydra.ApiDocumentation).FirstOrDefault() as IApiDocumentation;
                if (result == null)
                {
                    return Assert<IApiDocumentation, NoEntrypointDefinedException>(null, throwIfUnavailable);
                }

                _resourceCache[apiDocumentationUrl] = result;
            }

            return result;
        }

        private async Task<IHypermediaContainer> GetResourceFrom(
            Uri url,
            IHypermediaProcessingOptions options,
            CancellationToken cancellationToken)
        {
            var response = await MakeRequestTo(
                url ?? throw new ArgumentNullException(nameof(url), NoUrlProvided),
                null,
                cancellationToken);
            if (response.Status != 200)
            {
                throw new InvalidResponseException(response.Status);
            }

            if (ApiDocumentationPolicy != ApiDocumentationPolicy.None)
            {
                await GetApiDocumentation(response, url, false, cancellationToken);
            }
            
            var hypermediaProcessor = GetHypermediaProcessor(response);
            if (hypermediaProcessor == null)
            {
                throw new ResponseFormatNotSupportedException();
            }

            options = new HypermediaProcessingOptions(
                    url,
                    LinksPolicy,
                    ApiDocumentationPolicy,
                    _resourceCache.All<IApiDocumentation>())
                .MergeWith(options);
            return await hypermediaProcessor.Process(response, this, options, cancellationToken);
        }
        
        private Uri GetApiDocumentationUrl(IResponse response, Uri baseUrl)
        {
            var apiDocumentationLink = (
                from header in response.Headers["Link"]
                let match = LinkHeaderPattern.Match(header)
                where match.Success
                select match).FirstOrDefault();
            Uri result = null;
            if (apiDocumentationLink != null)
            {
                result = !Regex.IsMatch(apiDocumentationLink.Groups[1].Value, "^[a-z][a-z0-9+\\-.]*:")
                    ? new Uri(baseUrl, new Uri(apiDocumentationLink.Groups[1].Value, UriKind.Relative))
                    : new Uri(apiDocumentationLink.Groups[1].Value);
            }

            return result;
        }

        private async Task<IResponse> MakeRequestTo(Uri url, IHttpOptions options, CancellationToken cancellationToken)
        {
            return await _httpCall(url, options, cancellationToken);
        }

        private TResource Assert<TResource, TException>(
            TResource resource,
            bool throwIfUnabailable)
            where TResource : class
            where TException : Exception, new()
        {
            if (resource == null && throwIfUnabailable)
            {
                throw new TException();
            }

            return resource;
        }
    }
}
