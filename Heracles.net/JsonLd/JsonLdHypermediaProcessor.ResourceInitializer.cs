using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            hydra.operation,
        };

        private static readonly Iri[] HydraIndependentTypes = { hydra.Collection, hydra.PartialCollectionView };

        private static int _id;

        private static bool IsHydraDependent(ITypedEntity resource)
        {
            return resource.Type.Any(_ => _.ToString().StartsWith(hydra.Namespace) && !HydraIndependentTypes.Contains(_));
        }

        private static IResource ResourceInitializer(ITypedEntity resource, IHydraClient client, ProcessingState processingState)
        {
            IHydraResource hydraResource = resource as IHydraResource
                ?? (resource.Is(hydra.Resource) ? resource.ActLike<IHydraResource>() : null);

            var resourceStatementsCount = 0;
            foreach (var statement in processingState.StatementsOf(resource.Iri).Where(_ => _.Object != null && !IsStandaloneControl(_.Predicate)))
            {
                GatherLinks(resource, statement, processingState, ref hydraResource);
                resourceStatementsCount++;
            }

            if (hydraResource != null && resourceStatementsCount > 0)
            {
                var addToHypermedia = !processingState.ForbiddenHypermeda.Contains(hydraResource.Iri)
                    && (!hydraResource.Iri.IsBlank || IsHydraDependent(hydraResource));
                if (addToHypermedia)
                {
                    processingState.AllHypermedia.Add(hydraResource.Iri);
                }
            }

            return hydraResource;
        }

        private static void GatherLinks(ITypedEntity resource, Statement statement, ProcessingState processingState, ref IHydraResource hydraResource)
        {
            var relationResource = resource.Context.Load<IDerefencableLink>(statement.Predicate);
            var @object = resource.Context.Load<IResource>(statement.Object);
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