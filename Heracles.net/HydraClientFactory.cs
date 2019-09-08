using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Heracles.JsonLd;
using Heracles.Net.Http;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RDeF.Entities;

namespace Heracles
{
    public delegate IHypermediaProcessor HypermediaProcessorFactory(IHydraClientFactory context);

    /// <summary>Provides a factory of the <see cref="IHydraClient" />s.</summary>
    public sealed class HydraClientFactory : IHydraClientFactory
    {
        private static readonly string HydraVocabularyResourceName =
            typeof(HydraClientFactory).Assembly.GetManifestResourceNames().First(_ => _.Contains("hydra.json"));

        private static readonly JToken HydraVocabulary = (JToken)JsonSerializer.Create().Deserialize(
            new JsonTextReader(
                new StreamReader(
                    typeof(HydraClientFactory).Assembly.GetManifestResourceStream(HydraVocabularyResourceName))));

        private static readonly IEnumerable<Statement> Hydra =
            new JsonLdApi(new JsonLdApi().Expand(new Context(), HydraVocabulary), new JsonLdOptions(String.Empty)).ToRDF().AsStatements();

        private readonly IDictionary<HypermediaProcessorFactory, IHypermediaProcessor> _hypermediaProcessorInstances;
        private readonly ICollection<HypermediaProcessorFactory> _hypermediaProcessorFactories;
        private IIriTemplateExpansionStrategy _iriTemplateExpansionStrategy;
        private HttpCallFacility _httpCall;
        private LinksPolicy _linksPolicy;

        private HydraClientFactory(
            ICollection<HypermediaProcessorFactory> hypermediaProcessorFactories = null,
            IIriTemplateExpansionStrategy iriTemplateExpansionStrategy = null,
            LinksPolicy? linksPolicy = null,
            HttpCallFacility httpCall = null)
        {
            _hypermediaProcessorInstances = new Dictionary<HypermediaProcessorFactory, IHypermediaProcessor>();
            _hypermediaProcessorFactories = hypermediaProcessorFactories ?? new List<HypermediaProcessorFactory>();
            _iriTemplateExpansionStrategy = iriTemplateExpansionStrategy;
            _linksPolicy = linksPolicy ?? LinksPolicy.Strict;
            _httpCall = httpCall;
        }

        /// <inheritdoc />
        HttpCallFacility IHydraClientFactory.CurrentHttpCall
        {
            get { return _httpCall; }
        }

        /// <inheritdoc />
        LinksPolicy IHydraClientFactory.CurrentLinksPolicy
        {
            get { return _linksPolicy; }
        }

        /// <summary>Starts the factory configuration.</summary>
        /// <returns>Instance of the <see cref="HydraClientFactory" />.</returns>
        public static HydraClientFactory Configure()
        {
            return new HydraClientFactory();
        }
        
        private static IHypermediaProcessor CreateJsonLdHypermediaProcessor(HttpCallFacility httpCall)
        {
            return new JsonLdHypermediaProcessor(new StaticOntologyProvider(Hydra), httpCall);
        }

        /// <inheritdoc />
        public IHypermediaProcessor CreateProcessorToHandle(string mediaType)
        {
            if (!String.IsNullOrEmpty(mediaType))
            {
                foreach (var hypermediaProcessor in ResolveProcessors())
                {
                    foreach (var supportedMediaType in hypermediaProcessor.SupportedMediaTypes)
                    {
                        if (supportedMediaType == mediaType)
                        {
                            return hypermediaProcessor;
                        }
                    }
                }
            }

            throw new ArgumentNullException(HydraClient.NoHypermediaProcessors);
        }

        /// <summary>
        /// Configures a future <see cref="IHydraClient" /> with <see cref="JsonLdHypermediaProcessor" />,
        /// <see cref="BodyResourceBoundIriTemplateExpansionStrategy" /> and <i><see cref="HttpClient"/></i> components.
        /// </summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithDefaults()
        {
            return With(new BodyResourceBoundIriTemplateExpansionStrategy())
                .WithStrictLinks()
                .With(new HttpRequestHttpCallFacility().Call)
                .WithJsonLd();
        }

        /// <summary>Configures a factory to create a client with explicitly defined links.</summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithStrictLinks()
        {
            _linksPolicy = LinksPolicy.Strict;
            return this;
        }

        /// <summary>Configures a factory to create a client with links of resources from the same host and port.</summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithSameRootLinks()
        {
            _linksPolicy = LinksPolicy.SameRoot;
            return this;
        }

        /// <summary>Configures a factory to create a client with all resources from HTTP/HTTPS considered links.</summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithAllHttpLinks()
        {
            _linksPolicy = LinksPolicy.AllHttp;
            return this;
        }
   
        /// <summary>Configures a factory to create a client with all resources considered links.</summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithAllLinks()
        {
            _linksPolicy = LinksPolicy.All;
            return this;
        }

        /// <summary>Configures a factory with JSON-LD hypermedia processor.</summary>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithJsonLd()
        {
            WithFactory(context => CreateJsonLdHypermediaProcessor(context.CurrentHttpCall));
            return this;
        }

        /// <summary>Adds an another <see cref="IHypermediaProcessor" /> component via it's factory method.</summary>
        /// <param name="hypermediaProcessorFactory">
        /// Hypermedia processor factory to be passed to future <see cref="HydraClient" /> instances.
        /// </param>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory WithFactory(HypermediaProcessorFactory hypermediaProcessorFactory)
        {
            _hypermediaProcessorFactories.Add(hypermediaProcessorFactory);
            return this;
        }

        /// <summary>Adds an another <see cref="IHypermediaProcessor" /> component.</summary>
        /// <param name="hypermediaProcessor">
        /// Hypermedia processor to be passed to future <see cref="HydraClient" /> instances.
        /// </param>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory With(IHypermediaProcessor hypermediaProcessor)
        {
            _hypermediaProcessorFactories.Add(_ => hypermediaProcessor);
            return this;
        }

        /// <summary>Sets a <see cref="IIriTemplateExpansionStrategy" /> component.</summary>
        /// <param name="iriTemplateExpansionStrategy">
        /// IRI template expansion strategy to be used when an IRI template is encountered.
        /// </param>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory With(IIriTemplateExpansionStrategy iriTemplateExpansionStrategy)
        {
            _iriTemplateExpansionStrategy = iriTemplateExpansionStrategy;
            return this;
        }

        /// <summary>Adds an HTTP requests facility component.</summary>
        /// <param name="httpCall">HTTP call facility to be used for remote server calls.</param>
        /// <returns>This instance of the <see cref="HydraClientFactory" />.</returns>
        public HydraClientFactory With(HttpCallFacility httpCall)
        {
            _httpCall = httpCall;
            return this;
        }

        /// <summary>Creates a new instance of the <see cref="IHydraClient" />.</summary>
        /// <returns>New instance of the <see cref="IHydraClient" />.</returns>
        public IHydraClient AndCreate()
        {
            return new HydraClient(
                ResolveProcessors(),
                _iriTemplateExpansionStrategy,
                _linksPolicy,
                _httpCall);
        }

        private IEnumerable<IHypermediaProcessor> ResolveProcessors()
        {
            var result = new List<IHypermediaProcessor>();
            foreach (var factory in _hypermediaProcessorFactories)
            {
                if (!_hypermediaProcessorInstances.TryGetValue(factory, out IHypermediaProcessor instance))
                {
                    var childContext = new HydraClientFactory(
                        _hypermediaProcessorFactories.Where(_ => _ != factory).ToList(),
                        _iriTemplateExpansionStrategy,
                        _linksPolicy,
                        _httpCall);
                    _hypermediaProcessorInstances[factory] = instance = factory(childContext);
                }

                result.Add(instance);
            }

            return result;
        }
    }
}
