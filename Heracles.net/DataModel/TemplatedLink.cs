using System;
using System.Collections.Generic;
using System.Reflection;
using Heracles.Collections.Generic;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="ITemplatedLink"/> interface.</summary>
    public static class TemplatedLink
    {
        private static readonly PropertyInfo RelationPropertyInfo = typeof(IDereferencableLink).GetProperty(nameof(IDereferencableLink.Relation));

        private static int _id;

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="templatedLink">Templated link.</param>
        /// <param name="mappedVariables">Template variables with value.</param>
        /// <returns>Expanded templated resource.</returns>
        public static ILink ExpandTarget(ITemplatedLink templatedLink, IDictionary<string, string> mappedVariables)
        {
            return TemplatedResource<ILink>.ExpandTarget(templatedLink, mappedVariables, GetNextIri())
                .CreateInstanceFrom(templatedLink);
        }

        /// <summary>Expands an URI template with given variables.</summary>
        /// <param name="templatedLink">Templated link.</param>
        /// <param name="mappedVariables">Template variables mapping builder.</param>
        /// <returns>Expanded templated resource.</returns>
        public static ILink ExpandTarget(ITemplatedLink templatedLink, Action<MappingsBuilder> mappedVariables)
        {
            return TemplatedResource<ILink>.ExpandTarget(templatedLink, mappedVariables, GetNextIri())
                .CreateInstanceFrom(templatedLink);
        }

        private static ILink CreateInstanceFrom(this ILink result, ITemplatedLink templatedLink)
        {
            var proxy = result.Unwrap();
            proxy.SetProperty(RelationPropertyInfo, templatedLink.Relation);
            result.SupportedOperations.AddRange(templatedLink.SupportedOperations);
            result.Type.Remove(hydra.TemplatedLink);
            result.Type.Add(hydra.Link);
            return result;
        }

        private static Iri GetNextIri()
        {
            return new Iri($"_:blankLink{++_id}");
        }
    }
}