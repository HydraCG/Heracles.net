using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;
using RollerCaster.Reflection;

namespace Heracles.DataModel
{
    /// <summary>Provides useful <see cref="IResource" /> extensions.</summary>
    public static class ResourceExtensions
    {
        internal static readonly PropertyInfo BaseUrlPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.BaseUrl));
        internal static readonly PropertyInfo RelationPropertyInfo = typeof(IDereferencableLink).GetProperty(nameof(IDereferencableLink.Relation));
        internal static readonly PropertyInfo TargetPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.Target));
        internal static readonly PropertyInfo MethodPropertyInfo = typeof(IOperation).GetProperty(nameof(IOperation.Method));
        internal static readonly PropertyInfo OriginatingMediaTypeProperty = typeof(IOperation).GetProperty(nameof(IOperation.OriginatingMediaType));
        internal static readonly PropertyInfo LabelPropertyInfo = typeof(IResource).GetProperty(nameof(IResource.Label));
        internal static readonly PropertyInfo CommentPropertyInfo = typeof(IResource).GetProperty(nameof(IResource.Comment));
        internal static readonly PropertyInfo TitlePropertyInfo = typeof(IOperation).GetProperty(nameof(IOperation.Title));
        internal static readonly PropertyInfo DescriptionPropertyInfo = typeof(IOperation).GetProperty(nameof(IOperation.Description));
        internal static readonly HiddenPropertyInfo IriTemplatePropertyInfo = new HiddenPropertyInfo("IriTemplate", typeof(IIriTemplate), typeof(ITemplatedLink));

        private static readonly IDictionary<Iri, Func<IResource, string>> DisplayNameCandidates =
            new Dictionary<Iri, Func<IResource, string>>()
            {
                { hydra.ApiDocumentation, _ => _.ActLike<IApiDocumentation>().Title },
                { hydra.Operation, _ => _.ActLike<IOperation>().Title },
                { hydra.Class, _ => _.ActLike<IClass>().Title },
                { hydra.SupportedProperty, _ => _.ActLike<ISupportedProperty>().Title },
                { hydra.Link, _ => _.ActLike<ILink>().Title },
                { rdf.Property, _ => _.ActLike<IProperty>().Title },
                { new Iri(), _ => _.Label }
            };

        private static readonly IDictionary<Iri, Func<IResource, string>> TextDescriptionCandidates =
            new Dictionary<Iri, Func<IResource, string>>()
            {
                { hydra.ApiDocumentation, _ => _.ActLike<IApiDocumentation>().Description },
                { hydra.Operation, _ => _.ActLike<IOperation>().Description },
                { hydra.Class, _ => _.ActLike<IClass>().Description },
                { hydra.SupportedProperty, _ => _.ActLike<ISupportedProperty>().Description },
                { hydra.Link, _ => _.ActLike<ILink>().Description },
                { rdf.Property, _ => _.ActLike<IProperty>().Description },
                { new Iri(), _ => _.Comment }
            };

        /// <summary>Gets a display name of the given <paramref name="resource" />.</summary>
        /// <returns>This methods searches for <see cref="hydra.title" /> or <see cref="rdfs.label" /> occurances.</returns>
        /// <param name="resource">Resource for which to obtain a display name.</param>
        /// <returns>Display name of the given <paramref name="resource" /> or <i>null</i>.</returns>
        public static string GetDisplayName(this IResource resource)
        {
            return resource.GetTextFor(DisplayNameCandidates);
        }

        /// <summary>Gets a description of the given <paramref name="resource" />.</summary>
        /// <param name="resource">Resource for which to obtain a description.</param>
        /// <returns>Description of the given <paramref name="resource" /> or <i>null</i>.</returns>
        public static string GetTextDescription(this IResource resource)
        {
            return resource.GetTextFor(TextDescriptionCandidates);
        }

        internal static IOperation Copy(this IOperation source, Iri iri, Uri baseUrl = null, IResource target = null)
        {
            var result = source.Context.Create<IOperation>(iri);
            var proxy = result.Unwrap();
            result.Expects.CopyFrom(source.Expects);
            result.Returns.CopyFrom(source.Returns);
            result.ExpectedHeaders.CopyFrom(source.ExpectedHeaders);
            result.ReturnedHeaders.CopyFrom(source.ReturnedHeaders);
            result.CopyFrom(source, baseUrl);
            proxy.CopyFrom(TargetPropertyInfo, target ?? source.Target);
            proxy.CopyFrom(MethodPropertyInfo, source.Method);
            proxy.CopyFrom(TitlePropertyInfo, source.Title);
            proxy.CopyFrom(DescriptionPropertyInfo, source.Description);
            return result;
        }

        internal static IDereferencableLink Copy(this IDereferencableLink source, Iri iri)
        {
            var result = source.Context.Create<IDereferencableLink>(iri);
            var proxy = result.Unwrap();
            proxy.CopyFrom(RelationPropertyInfo, source.Iri.ToString());
            result.SupportedOperations.CopyFrom(source.SupportedOperations);
            result.CopyFrom(source);
            return result;
        }

        private static string GetTextFor(this IResource resource, IDictionary<Iri, Func<IResource, string>> candidates)
        {
            string result = null;
            foreach (var candidate in candidates.Where(_ => _.Key.IsBlank || resource.Is(_.Key)))
            {
                var candidateText = candidate.Value(resource);
                if ((candidateText != null) && (result == null || candidateText.Length > result.Length))
                {
                    result = candidateText;
                }
            }

            return result;
        }

        private static void CopyFrom(this IPointingResource target, IPointingResource source, Uri baseUrl = null)
        {
            var proxy = target.Unwrap();
            proxy.CopyFrom(BaseUrlPropertyInfo, baseUrl ?? source.BaseUrl);
            proxy.CopyFrom(TargetPropertyInfo, source.Target);
            proxy.CopyFrom(LabelPropertyInfo, source.Label);
            proxy.CopyFrom(CommentPropertyInfo, source.Comment);
            target.Collections.CopyFrom(source.Collections);
            target.Operations.CopyFrom(source.Operations);
            target.Links.CopyFrom(source.Links);
            target.Type.CopyFrom(source.Type);
        }

        private static void CopyFrom<T>(this ICollection<T> targetCollection, IEnumerable<T> sourceCollection)
        {
            if (sourceCollection != null)
            {
                foreach (var item in sourceCollection)
                {
                    targetCollection.Add(item);
                }
            }
        }

        private static void CopyFrom<T>(this MulticastObject proxy, PropertyInfo propertyInfo, T source)
        {
            if (source != null)
            {
                proxy.SetProperty(propertyInfo, source);
            }
        }
    }
}
