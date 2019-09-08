using System;
using System.Collections.Generic;
using System.Reflection;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides useful <see cref="IResource" /> extensions.</summary>
    public static class ResourceExtensions
    {
        internal static readonly PropertyInfo BaseUrlPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.BaseUrl));
        internal static readonly PropertyInfo RelationPropertyInfo = typeof(IDerefencableLink).GetProperty(nameof(IDerefencableLink.Relation));
        internal static readonly PropertyInfo TargetPropertyInfo = typeof(IPointingResource).GetProperty(nameof(IPointingResource.Target));
        internal static readonly PropertyInfo MethodPropertyInfo = typeof(IOperation).GetProperty(nameof(IOperation.Method));

        private static readonly IDictionary<Iri, Func<IResource, string>> DisplayNameCandidates =
            new Dictionary<Iri, Func<IResource, string>>()
            {
                { hydra.ApiDocumentation, _ => _.ActLike<IApiDocumentation>().Title },
                { hydra.Class, _ => _.ActLike<IClass>().Title },
                { rdf.Property, _ => _.ActLike<IProperty>().Title },
                { new Iri(), _ => _.Label }
            };

        private static readonly IDictionary<Iri, Func<IResource, string>> TextDescriptionCandidates =
            new Dictionary<Iri, Func<IResource, string>>()
            {
                { hydra.ApiDocumentation, _ => _.ActLike<IApiDocumentation>().Description },
                { hydra.Class, _ => _.ActLike<IClass>().Description },
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

        public static string GetTextDescription(this IResource resource)
        {
            return resource.GetTextFor(TextDescriptionCandidates);
        }

        internal static IOperation Copy(this IOperation source, Iri iri)
        {
            var result = source.Context.Create<IOperation>(iri);
            var proxy = result.Unwrap();
            proxy.SetProperty(MethodPropertyInfo, source.Method);
            result.Expects.CopyFrom(source.Expects);
            result.Returns.CopyFrom(source.Returns);
            result.ExpectedHeaders.CopyFrom(source.ExpectedHeaders);
            result.ReturnedHeaders.CopyFrom(source.ReturnedHeaders);
            result.CopyFrom(source);
            return result;
        }

        internal static IDerefencableLink Copy(this IDerefencableLink source, Iri iri)
        {
            var result = source.Context.Create<IDerefencableLink>(iri);
            var proxy = result.Unwrap();
            proxy.SetProperty(RelationPropertyInfo, source.Iri.ToString());
            result.SupportedOperations.CopyFrom(source.SupportedOperations);
            result.CopyFrom(source);
            return result;
        }

        private static string GetTextFor(this IResource resource, IDictionary<Iri, Func<IResource, string>> candidates)
        {
            string result = null;
            foreach (var candidate in candidates)
            {
                if (candidate.Key.IsBlank || resource.Is(candidate.Key))
                {
                    result = candidate.Value(resource);
                    if (!String.IsNullOrEmpty(result))
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private static void CopyFrom(this IPointingResource target, IPointingResource source)
        {
            var proxy = target.Unwrap();
            proxy.SetProperty(BaseUrlPropertyInfo, source.BaseUrl);
            proxy.SetProperty(TargetPropertyInfo, source.Target);
            target.Collections.CopyFrom(source.Collections);
            target.Operations.CopyFrom(source.Operations);
            target.Links.CopyFrom(source.Links);
            target.Type.CopyFrom(source.Type);
        }

        private static void CopyFrom<T>(this ICollection<T> targetCollection, IEnumerable<T> sourceCollection)
        {
            foreach (var item in sourceCollection)
            {
                targetCollection.Add(item);
            }
        }
    }
}
