using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Collections.Generic;
using Heracles.DataModel;
using Heracles.Entities;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.JsonLd
{
    /// <summary>Contains links initializer.</summary>
    public partial class JsonLdHypermediaProcessor
    {
        private static readonly ISet<Iri> StandaloneControls = new HashSet<Iri>()
        {
            hydra.view,
            hydra.collection,
            hydra.supportedClass,
            hydra.supportedOperation,
            hydra.supportedProperty,
            hydra.entrypoint,
            hydra.manages,
            hydra.member,
            hydra.first,
            hydra.last,
            hydra.next,
            hydra.previous,
            hydra.mapping,
            hydra.variableRepresentation,
            hydra.expects,
            hydra.returns,
            hydra.subject,
            hydra.property,
            hydra.@object,
            hydra.operation
        };

        private static readonly IDictionary<Iri, Func<IHydraResource, IEnumerable<IOperation>>> OperationContainers =
            new Dictionary<Iri, Func<IHydraResource, IEnumerable<IOperation>>>()
            {
                { hydra.Class, _ => _.ActLike<IClass>().SupportedOperations },
                { hydra.Link, _ => _.ActLike<ILink>().SupportedOperations },
                { hydra.TemplatedLink, _ => _.ActLike<ITemplatedLink>().SupportedOperations }
            };

        private static readonly Iri[] HydraIndependentTypes = { hydra.Collection, hydra.PartialCollectionView };

        private static int _id;

        private static bool IsHydraDependent(ITypedEntity resource)
        {
            return resource.Type.Any(_ => _.ToString().StartsWith(hydra.Namespace) && !HydraIndependentTypes.Contains(_));
        }

        private static async Task<IResource> ResourceInitializer(
            ITypedEntity resource,
            IHydraClient client,
            ProcessingState processingState,
            CancellationToken cancellationToken)
        {
            IHydraResource hydraResource = resource as IHydraResource
                ?? (resource.Is(hydra.Resource) ? resource.ActLike<IHydraResource>() : null);

            bool hasView = false;
            foreach (var statement in processingState.StatementsOf(resource.Iri))
            {
                hydraResource = await GatherLinks(resource, statement, processingState, hydraResource, cancellationToken);
                if (statement.Predicate == hydra.view)
                {
                    hasView = true;
                }
            }

            if (hydraResource != null && processingState.NumberOfStatementsOf(resource.Iri) > 0)
            {
                GatherOperationTargets(hydraResource);
                if (hasView)
                {
                    hydraResource = hydraResource.ActLike<IResourceView>();
                }

                var addToHypermedia = !processingState.ForbiddenHypermeda.Contains(hydraResource.Iri)
                    && (!hydraResource.Iri.IsBlank || IsHydraDependent(hydraResource));
                if (addToHypermedia)
                {
                    processingState.AllHypermedia.Add(hydraResource.Iri);
                }
            }

            return hydraResource;
        }

        private static void GatherOperationTargets(IHydraResource resource)
        {
            if (resource != null)
            {
                IEnumerable<IOperation> operations = resource.Operations;
                var supportedOperationsContainer = OperationContainers.Where(_ => resource.Type.Contains(_.Key)).Select(_ => _.Value).FirstOrDefault();
                if (supportedOperationsContainer != null)
                {
                    operations = operations.Concat(supportedOperationsContainer(resource));
                }

                foreach (var operation in operations)
                {
                    operation.Unwrap().SetProperty(ResourceExtensions.TargetPropertyInfo, resource);
                }
            }
        }

        private static async Task<IHydraResource> GatherLinks(
            ITypedEntity resource,
            Statement statement,
            ProcessingState processingState,
            IHydraResource hydraResource,
            CancellationToken cancellationToken)
        {
            var relationResource = await resource.Context.Load<IDerefencableLink>(statement.Predicate, cancellationToken);
            var @object = await resource.Context.Load<IResource>(statement.Object, cancellationToken);
            var linkType = relationResource.GetLinkType() ?? @object.GetLinkType(processingState.LinksPolicy, processingState.Root);
            if (linkType != null)
            {
                hydraResource = hydraResource ?? resource.ActLike<IHydraResource>();
                var owner = hydraResource;
                processingState.MarkAsOwned(relationResource.Iri);
                processingState.MarkAsOwned(@object.Iri);
                processingState.ProcessingCompleted +=
                    (sender, e) => CreateRelationHandler((ProcessingState)sender, owner, relationResource, @object, linkType);
            }

            return hydraResource;
        }

        private static Iri GetNextIri(string type)
        {
            return new Iri($"_:blank{type}{++_id}");
        }

        private static bool IsStandaloneControl(Iri iri)
        {
            return StandaloneControls.Contains(iri);
        }

        private static void CreateRelationHandler(
            ProcessingState processingState,
            IHydraResource owner,
            IDerefencableLink relationResource,
            IResource @object,
            Iri type)
        {
            var handlers = relationResource.SupportedOperations.Any() && @object != null
                ? CreateTemplatedOperation(owner, relationResource, @object, hydra.Operation)
                : (IEnumerable)CreateLink(owner, relationResource, @object, type);
            foreach (IResource handler in handlers)
            {
                var proxy = handler.Unwrap();
                proxy.SetProperty(ResourceExtensions.BaseUrlPropertyInfo, (Uri)processingState.BaseUrl);
                processingState.ForbiddenHypermeda.Add(handler.Iri);
            }
        }

        private static IEnumerable<IDerefencableLink> CreateLink(
            IHydraResource owner,
            IDerefencableLink relationResource,
            IResource @object,
            Iri type)
        {
            var result = relationResource.Copy(GetNextIri(type.ToString().Replace(hydra.Namespace, String.Empty)));
            var proxy = result.Unwrap();
            proxy.SetProperty(ResourceExtensions.TargetPropertyInfo, @object);
            result.Type.AddIfNotExist(type);
            result.Type.Remove(type == hydra.TemplatedLink ? hydra.Link : hydra.TemplatedLink);
            if (type == hydra.TemplatedLink && @object != null)
            {
                var templatedLink = result.ActLike<ITemplatedLink>();
                proxy.SetProperty(IriTemplatePropertyInfo, @object.ActLike<IIriTemplate>());
                result = templatedLink;
            }

            owner.Links.Add(result);
            return new[] { result };
        }

        private static IEnumerable<IOperation> CreateTemplatedOperation(
            IHydraResource owner,
            IDerefencableLink relationResource,
            IEntity @object,
            Iri type)
        {
            foreach (var operation in relationResource.SupportedOperations)
            {
                var result = operation.Copy(GetNextIri("Operation"));
                result.Type.AddIfNotExist(type);
                result.Type.AddIfNotExist(hydra.IriTemplate);
                var templatedOperation = result.ActLike<ITemplatedOperation>();
                var proxy = templatedOperation.Unwrap();
                proxy.SetProperty(IriTemplatePropertyInfo, @object.ActLike<IIriTemplate>());
                result = templatedOperation;
                owner.Operations.Add(result);
                yield return result;
            }
        }
    }
}