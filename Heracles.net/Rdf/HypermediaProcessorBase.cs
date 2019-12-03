using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;
using Heracles.Namespaces;
using RDeF.Entities;
using RDeF.Serialization;
using RollerCaster;
using RollerCaster.Reflection;

namespace Heracles.Rdf
{
    /// <summary>Provides a base functionality of the <see cref="IHypermediaProcessor" /> interface.</summary>
    public abstract partial class HypermediaProcessorBase : IHypermediaProcessor
    {
        internal static readonly HiddenPropertyInfo ClientPropertyInfo = new HiddenPropertyInfo("Client", typeof(IHydraClient), typeof(IResource));
        internal static readonly HiddenPropertyInfo IriTemplatePropertyInfo = new HiddenPropertyInfo("IriTemplate", typeof(IIriTemplate), typeof(ITemplatedLink));

        private static readonly Iri Untyped = new Iri();

        private static readonly IDictionary<Iri, Func<ITypedEntity, IHydraClient, ProcessingState, CancellationToken, Task<IResource>>> Initializers
            = new Dictionary<Iri, Func<ITypedEntity, IHydraClient, ProcessingState, CancellationToken, Task<IResource>>>()
            {
                { Untyped, ResourceInitializer },
                { hydra.ApiDocumentation, ClientInitializer },
                { hydra.Collection, ClientInitializer },
                { hydra.Operation, PointingResourceInitializer },
                { hydra.Link, PointingResourceInitializer },
                { hydra.TemplatedLink, PointingResourceInitializer }
            };

        private static readonly Lazy<IEntityContextFactory> EntityContextFactory =
            new Lazy<IEntityContextFactory>(CreateEntityContextFactory);

        private readonly IOntologyProvider _ontologyProvider;

        static HypermediaProcessorBase()
        {
            MulticastObject
                .ImplementationOf<IApiDocumentation>()
                    .ForFunction(_ => _.GetEntryPoint())
                    .ImplementedBy(_ => ApiDocumentation.GetEntryPoint(_));
            MulticastObject
                .ImplementationOf<IApiDocumentation>()
                    .ForFunction(_ => _.GetEntryPoint(CancellationToken.None))
                    .ImplementedBy(_ => ApiDocumentation.GetEntryPoint(_, CancellationToken.None));
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

        /// <summary>Initializes a new instance of the <see cref="HypermediaProcessorBase" /> class.</summary>
        /// <param name="ontologyProvider">Ontology provider.</param>
        /// <param name="httpCall">HTTP call facility.</param>
        protected HypermediaProcessorBase(IOntologyProvider ontologyProvider, HttpCallFacility httpCall)
        {
            _ontologyProvider = ontologyProvider ?? throw new ArgumentNullException(nameof(ontologyProvider)); 
            HttpCall = httpCall ?? throw new ArgumentNullException(nameof(httpCall));
        }

        /// <inheritdoc />
        public abstract IEnumerable<string> SupportedMediaTypes { get; }

        /// <summary>Gets an HTTP call facility.</summary>
        protected HttpCallFacility HttpCall { get; }

        /// <inheritdoc />
        public abstract Level Supports(IResponse response);

        /// <inheritdoc />
        public Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            IHypermediaProcessingOptions options = null)
        {
            return Process(response, hydraClient, options, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            CancellationToken cancellationToken)
        {
            return Process(response, hydraClient, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IHypermediaContainer> Process(
            IResponse response,
            IHydraClient hydraClient,
            IHypermediaProcessingOptions options,
            CancellationToken cancellationToken)
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
            using (var processingState = new ProcessingState(
                context,
                _ontologyProvider,
                responseIri,
                options?.LinksPolicy ?? LinksPolicy.Strict,
                response.Headers[HydraClient.ContentType].FirstOrDefault()))
            {
                var rdfReader = await CreateRdfReader(response, cancellationToken);
                var serializableSource = (ISerializableEntitySource)context.EntitySource;
                using (processingState.StartGatheringStatementsFor(serializableSource, _ => _.Object != null && !IsStandaloneControl(_.Predicate)))
                using (var reader = new StreamReader(await response.GetBody(cancellationToken)))
                {
                    await serializableSource.Read(reader, rdfReader, response.Url, cancellationToken);
                }

                resource = await ProcessResources(processingState, hydraClient, hypermedia, cancellationToken);
            }

            return new HypermediaContainer(response, resource, hypermedia);
        }

        /// <inheritdoc />
        public Task<Stream> Serialize(IResource body)
        {
            return Serialize(body, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<Stream> Serialize(IResource body, CancellationToken cancellationToken)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (body.Iri == null)
            {
                throw new ArgumentOutOfRangeException(nameof(body));
            }

            var statements =
                from statement in await body.Context.EntitySource.Load(body.Iri, cancellationToken)
                group statement by statement.Graph into graphs
                select new Graph(graphs.Key, graphs);
            var jsonLdWriter = CreateRdfWriter();
            var buffer = new MemoryStream();
            using (var writer = new StreamWriter(buffer, Encoding.UTF8, 4096, true))
            {
                await jsonLdWriter.Write(writer, statements, cancellationToken);
            }

            buffer.Seek(0, SeekOrigin.Begin);
            return buffer;
        }

        /// <summary>Creates a new <see cref="IRdfReader" /> instance.</summary>
        /// <param name="response">Original response obtained from the server.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Instance of the <see cref="IRdfReader" />.</returns>
        protected abstract Task<IRdfReader> CreateRdfReader(IResponse response, CancellationToken cancellationToken);

        /// <summary>Creates a new <see cref="IRdfWriter" /> instance.</summary>
        /// <returns>Instance of the <see cref="IRdfWriter" />.</returns>
        protected abstract IRdfWriter CreateRdfWriter();

        private static IEntityContextFactory CreateEntityContextFactory()
        {
            return RDeF.Entities.EntityContextFactory
                .FromConfiguration("heracles.net")
                .WithQIri("hydra", hydra.Namespace)
                .WithQIri("rdf", rdf.Namespace)
                .WithQIri("rdfs", rdfs.Namespace)
                .WithMappings(_ => _.FromAssemblyOf<HypermediaProcessorBase>());
        }

        private async Task<IResource> ProcessResources(
            ProcessingState processingState,
            IHydraClient hydraClient,
            List<IResource> hypermedia,
            CancellationToken cancellationToken)
        {
            var resource = await processingState.Context.Load<IResource>(processingState.BaseUrl, cancellationToken);
            foreach (var entity in processingState.Context.AsQueryable<ITypedEntity>())
            {
                Func<ITypedEntity, IHydraClient, ProcessingState, CancellationToken, Task<IResource>> initializer = Initializers[Untyped];
                foreach (var type in entity.Type.Where(item => item.ToString().StartsWith(hydra.Namespace)))
                {
                    if (!Initializers.TryGetValue(type, out initializer))
                    {
                        initializer = Initializers[Untyped];
                        continue;
                    }

                    break;
                }

                var currentResource = await initializer(entity, hydraClient, processingState, cancellationToken);
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
