using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;

namespace Heracles
{
    /// <summary>HydraClient, also known as Heracles.ts, is a generic client for Hydra-powered Web APIs.</summary>
    /// <remarks>
    /// To learn more about Hydra please refer to
    /// <a href="https://www.hydra-cg.com/spec/latest/core/">Hydra Core Vocabulary.</a>
    /// </remarks>
    public class HydraClient : IHydraClient
    {
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
            return GetApiDocumentation(resource?.Iri);
        }

        /// <inheritdoc />
        public async Task<IApiDocumentation> GetApiDocumentation(Uri url)
        {
            var apiDocumentation = await GetApiDocumentationUrl(
                url ?? throw new ArgumentNullException(nameof(url), NoUrlProvided));
            var options = new HypermediaProcessingOptions(apiDocumentation.Response, url);
            var resource = await GetResourceFrom(apiDocumentation.Url, options);
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
            return GetResource(resource?.Iri);
        }
        
        public async Task<IResponse> Invoke(
            IOperation operation, 
            IWebResource body = null, 
            IResource parameters = null)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation), NoOperationProvided);
            }

            var targetOperation = _iriTemplateExpansionStrategy.CreateRequest(operation, body, parameters);
            // TODO: move Content-Type header to some specialized component.
            // TODO: move body serialization to some specialized component.
            return await MakeRequestTo(
                targetOperation.Target.Iri,
                new HttpOptions(
                    targetOperation.Method,
                    body,
                    new Dictionary<string, string>() { { "Content-Type", "application/ld+json" } }));
        }

        /// <inheritdoc />
        public async Task<IHypermediaContainer> GetResource(Uri url)
        {
            return await GetResourceFrom(url);
        }

        private async Task<IHypermediaContainer> GetResourceFrom(Uri url, IHypermediaProcessingOptions options = null)
        {
            var response = await MakeRequestTo(url ?? throw new ArgumentNullException(nameof(url), NoUrlProvided));
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
        
        private async Task<ApiDocumentationDetails> GetApiDocumentationUrl(Uri url)
        {
            var response = await MakeRequestTo(url);
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

            return new ApiDocumentationDetails(
                response,
                !Regex.IsMatch(result.Groups[1].Value, "^[a-z][a-z0-9+\\-.]*:")
                    ? new Uri(url, new Uri(result.Groups[1].Value, UriKind.Relative))
                    : new Uri(result.Groups[1].Value));
        }

        private async Task<IResponse> MakeRequestTo(Uri url, IHttpOptions options = null)
        {
            return await _httpCall(url, options);
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
