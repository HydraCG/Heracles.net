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

        /// <summary>Initializes a new instance of the <see cref="HydraClient" /> class.</summary>
        /// <param name="hypermediaProcessors">
        /// Hypermedia processors used for response hypermedia controls extraction.
        /// </param>
        /// <param name="iriTemplateExpansionStrategy">IRI template variable expansion strategy.</param>
        /// <param name="linksPolicy">Policy defining what is a considered a link.</param>
        /// <param name="httpCall">HTTP facility used to call remote server.</param>
        public HydraClient(
            IEnumerable<IHypermediaProcessor> hypermediaProcessors,
            IIriTemplateExpansionStrategy iriTemplateExpansionStrategy,
            LinksPolicy linksPolicy,
            HttpCallFacility httpCall)
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
        }

        internal LinksPolicy LinksPolicy { get; }

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
            var apiDocumentation = await GetApiDocumentationUrl(
                url ?? throw new ArgumentNullException(nameof(url), NoUrlProvided),
                cancellationToken);
            var options = new HypermediaProcessingOptions(apiDocumentation.Response, url);
            var resource = await GetResourceFrom(apiDocumentation.Url, options, cancellationToken);
            var result = resource.OfType(hydra.ApiDocumentation).FirstOrDefault() as IApiDocumentation;
            if (result == null)
            {
                throw new NoEntrypointDefinedException();
            }

            return result;
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

        private async Task<IHypermediaContainer> GetResourceFrom(Uri url, IHypermediaProcessingOptions options, CancellationToken cancellationToken)
        {
            var response = await MakeRequestTo(
                url ?? throw new ArgumentNullException(nameof(url), NoUrlProvided),
                null,
                cancellationToken);
            if (response.Status != 200)
            {
                throw new InvalidResponseException(response.Status);
            }

            var hypermediaProcessor = GetHypermediaProcessor(response);
            if (hypermediaProcessor == null)
            {
                throw new ResponseFormatNotSupportedException();
            }

            options = new HypermediaProcessingOptions(url, LinksPolicy).MergeWith(options);
            return await hypermediaProcessor.Process(response, this, options);
        }
        
        private async Task<ApiDocumentationDetails> GetApiDocumentationUrl(Uri url, CancellationToken cancellationToken)
        {
            var response = await MakeRequestTo(url, null, cancellationToken);
            if (response.Status != 200)
            {
                throw new InvalidResponseException(response.Status);
            }
            
            var result = (
                from header in response.Headers["Link"]
                let match = LinkHeaderPattern.Match(header)
                where match.Success
                select match).FirstOrDefault();
            if (result == null)
            {
                throw new ApiDocumentationNotProvidedException();
            }

            var uri = !Regex.IsMatch(result.Groups[1].Value, "^[a-z][a-z0-9+\\-.]*:")
                ? new Uri(url, new Uri(result.Groups[1].Value, UriKind.Relative))
                : new Uri(result.Groups[1].Value);
            return new ApiDocumentationDetails(response, uri);
        }

        private async Task<IResponse> MakeRequestTo(Uri url, IHttpOptions options, CancellationToken cancellationToken)
        {
            return await _httpCall(url, options, cancellationToken);
        }

        private class ApiDocumentationDetails
        {
            internal ApiDocumentationDetails(IResponse response, Uri url)
            {
                Response = response;
                Url = url;
            }
            
            internal IResponse Response { get; }
            
            internal Uri Url { get; }
        }
    }
}
