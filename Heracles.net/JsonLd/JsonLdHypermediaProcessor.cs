﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Namespaces;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RDeF.Entities;
using RDeF.Serialization;
using RollerCaster;
using RollerCaster.Reflection;

namespace Heracles.JsonLd
{
    /// <summary>Provides a JSON-LD based implementation of the {@link IHypermediaProcessor} interface.</summary>
    public partial class JsonLdHypermediaProcessor : IHypermediaProcessor
    {
        private const string JsonLdContext = "http://www.w3.org/ns/json-ld#context";
        
        private const string JsonLd = "application/ld+json";
        private const string Json = "application/json";
        
        internal static readonly HiddenPropertyInfo ClientPropertyInfo = new HiddenPropertyInfo("Client", typeof(IHydraClient), typeof(IResource));
        internal static readonly HiddenPropertyInfo IriTemplatePropertyInfo = new HiddenPropertyInfo("IriTemplate", typeof(IIriTemplate), typeof(ITemplatedLink));

        private static readonly string[] MediaTypes = { JsonLd, Json };
        
        private static readonly Iri Untyped = new Iri();

        private static readonly Regex LinkPattern = new Regex("\\<(?<Uri>[^>]+)\\>;(.*rel=\"?(?<Relation>[^ \";]+)\"?;.*type=\"?(?<Type>[^ \";]+)\"?.*|.*type=\"?(?<Type>[^ \";]+)\"?;.*rel=\"?(?<Relation>[^ \";]+)\"?.*)");
        
        private static readonly IDictionary<Iri, Func<ITypedEntity, IHydraClient, ProcessingState, IResource>> Initializers
            = new Dictionary<Iri, Func<ITypedEntity, IHydraClient, ProcessingState, IResource>>()
            {
                { Untyped, ResourceInitializer },
                { hydra.ApiDocumentation, ClientInitializer },
                { hydra.Collection, ClientInitializer },
                { hydra.Operation, PointingResourceInitializer },
                { hydra.Link, PointingResourceInitializer },
                { hydra.TemplatedLink, PointingResourceInitializer }
            };
        
    private static readonly Func<IHeaders, bool>[][] ExactMatchCases =
        {
            new Func<IHeaders, bool>[] { headers => headers["Content-Type"].Any(_ => _.Contains(JsonLd)) },
            new Func<IHeaders, bool>[]
            {
                headers => headers["Content-Type"].Any(_ => _.Contains(Json)),
                headers => { return headers["Link"].Any(_ => _.Contains(JsonLdContext) && _.Contains(JsonLd)); }
            }
        };

        private static readonly Lazy<IEntityContextFactory> EntityContextFactory =
            new Lazy<IEntityContextFactory>(CreateEntityContextFactory);

        private readonly HttpCallFacility _httpCall;
        private readonly IOntologyProvider _ontologyProvider;

        static JsonLdHypermediaProcessor()
        {
            MulticastObject
                .ImplementationOf<IApiDocumentation>()
                    .ForFunction(_ => _.GetEntryPoint())
                    .ImplementedBy(_ => ApiDocumentation.GetEntryPoint(_));
            MulticastObject
                .ImplementationOf<ICollection>()
                    .ForFunction(_ => _.GetIterator())
                    .ImplementedBy(_ => Collection.GetIterator(_));
            MulticastObject
                .ImplementationOf<ITemplatedLink>()
                    .ForFunction(_ => _.ExpandTarget((IDictionary<string, string>)null))
                    .ImplementedBy(_ => TemplatedLink.ExpandTarget(_, (IDictionary<string, string>)null));
            MulticastObject
                .ImplementationOf<ITemplatedLink>()
                    .ForFunction(_ => _.ExpandTarget((Action<MappingsBuilder>)null))
                    .ImplementedBy(_ => TemplatedLink.ExpandTarget(_, (Action<MappingsBuilder>)null));
            MulticastObject
                .ImplementationOf<ITemplatedOperation>()
                    .ForFunction(_ => _.ExpandTarget((IDictionary<string, string>)null))
                    .ImplementedBy(_ => TemplatedOperation.ExpandTarget(_, (IDictionary<string, string>)null));
            MulticastObject
                .ImplementationOf<ITemplatedOperation>()
                    .ForFunction(_ => _.ExpandTarget((Action<MappingsBuilder>)null))
                    .ImplementedBy(_ => TemplatedOperation.ExpandTarget(_, (Action<MappingsBuilder>)null));
            MulticastObject
                .ImplementationOf<IResource>()
                    .ForProperty(_ => _.DisplayName)
                    .ImplementedBy(_ => ResourceExtensions.GetDisplayName(_));
            MulticastObject
                .ImplementationOf<IResource>()
                    .ForProperty(_ => _.TextDescription)
                    .ImplementedBy(_ => ResourceExtensions.GetTextDescription(_));
        }

        public JsonLdHypermediaProcessor(IOntologyProvider ontologyProvider, HttpCallFacility httpCall)
        {
            _ontologyProvider = ontologyProvider ?? throw new ArgumentNullException(nameof(ontologyProvider)); 
            _httpCall = httpCall ?? throw new ArgumentNullException(nameof(httpCall));
        }

        /// <inheritdoc />
        public IEnumerable<string> SupportedMediaTypes
        {
            get { return MediaTypes; }
        }
        
        /// <inheritdoc />
        public Level Supports(IResponse response)
        {
            var result = Level.None;
            if (response != null)
            {
                foreach (var approach in ExactMatchCases)
                {
                    var currentMatch = approach.All(_ => _(response.Headers));
                    if (currentMatch)
                    {
                        result = Level.FullSupport;
                        break;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            IHypermediaProcessingOptions options = null)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (hydraClient == null)
            {
                throw new ArgumentNullException(nameof(hydraClient));
            }

            var responseIri = options?.OriginalUrl ?? response.Url;
            var context = EntityContextFactory.Value.Create();
            IResource resource;
            var hypermedia = new List<IResource>();
            using (var processingState = new ProcessingState(context, _ontologyProvider, responseIri, options?.LinksPolicy ?? LinksPolicy.Strict))
            {
                var jsonLdReader = new JsonLdReader(await CreateJsonLdOptionsFor(response));
                var serializableSource = (ISerializableEntitySource)context.EntitySource;
                using (processingState.StartGatheringStatementsFor(serializableSource))
                using (var reader = new StreamReader(await response.GetBody()))
                {
                    await serializableSource.Read(reader, jsonLdReader);
                }

                resource = ProcessResources(processingState, hydraClient, hypermedia);
            }

            return new HypermediaContainer(response, resource, hypermedia);
        }

        private IResource ProcessResources(ProcessingState processingState, IHydraClient hydraClient, List<IResource> hypermedia)
        {
            var resource = processingState.Context.Load<IResource>(processingState.BaseUrl);
            foreach (var entity in processingState.Context.AsQueryable<ITypedEntity>())
            {
                Func<ITypedEntity, IHydraClient, ProcessingState, IResource> initializer = Initializers[Untyped];
                foreach (var type in entity.Type.Where(item => item.ToString().StartsWith(hydra.Namespace)))
                {
                    if (!Initializers.TryGetValue(type, out initializer))
                    {
                        initializer = Initializers[Untyped];
                        continue;
                    }

                    break;
                }

                var currentResource = initializer(entity, hydraClient, processingState);
                if (currentResource != null)
                {
                    if (processingState.AllHypermedia.Contains(currentResource.Iri))
                    {
                        currentResource = EnsureTypeCastedFor(currentResource);
                        hypermedia.Add(currentResource);
                    }

                    if (currentResource.Iri == resource.Iri)
                    {
                        resource = currentResource;
                    }
                }
            }

            return resource;
        }

        private static IEntityContextFactory CreateEntityContextFactory()
        {
            return new DefaultEntityContextFactory()
                .WithQIri("hydra", hydra.Namespace)
                .WithQIri("rdf", rdf.Namespace)
                .WithQIri("rdfs", rdfs.Namespace)
                .WithMappings(_ => _.FromAssemblyOf<JsonLdHypermediaProcessor>());
        }

        private async Task<JsonLdOptions> CreateJsonLdOptionsFor(IResponse response)
        {
            JsonLdOptions result = new JsonLdOptions(response.Url.ToString());
            result.documentLoader = new JsonLdDocumentLoader(_httpCall);
            if (response.Headers["Content-Type"].Contains(Json))
            {
                var link = (
                    from item in response.Headers["Link"]
                    let pattern = LinkPattern.Match(item)
                    where pattern.Success && pattern.Groups["Relation"].Value == JsonLdContext
                    select new { Url = new Uri(response.Url, pattern.Groups["Uri"].Value), Type = pattern.Groups["Type"].Value }).First();
                var contextResponse = await _httpCall(link.Url, new HttpOptions() { Headers = { { "Accept", link.Type } } });
                using (var reader = new StreamReader(await contextResponse.GetBody()))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    result.SetExpandContext((JObject)JsonSerializer.CreateDefault().Deserialize(jsonReader));
                }
            }

            return result;
        }

        private IResource EnsureTypeCastedFor(IResource resource)
        {
            var result = resource;
            var typeToCast = typeof(IResource);
            foreach (var type in resource.Type)
            {
                var entityMapping = resource.Context.Mappings.FindEntityMappingFor(resource, type);
                if (entityMapping?.Type != null && entityMapping.Type != typeToCast && typeToCast.IsAssignableFrom(entityMapping.Type))
                {
                    typeToCast = entityMapping.Type;
                }
            }

            if (typeToCast != typeof(IResource))
            {
                result = (IResource)resource.ActLike(typeToCast);
            }

            return result;
        }
    }
}