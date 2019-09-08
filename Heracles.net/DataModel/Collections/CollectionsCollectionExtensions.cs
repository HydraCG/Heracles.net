using System.Collections.Generic;
using System.Linq;
using RDeF.Entities;
using RDeF.Vocabularies;

namespace Heracles.DataModel.Collections
{
    /// <summary>Provides a convenient collection for <see cref="IEnumerable{ICollection}" />s.</summary>
    public static class CollectionsCollectionExtensions
    {
        /// <summary>
        /// Obtains those collections that defines a hydra:manages block matching a given property and object.
        /// </summary>
        /// <param name="collections">Collection to be filtered.</param>
        /// <param name="subject">Subject to match in the hydra:manages block.</param>
        /// <param name="property">Property to match in the hydra:manages block.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<ICollection> WithMembersInRelationWith(
            this IEnumerable<ICollection> collections,
            Iri subject,
            Iri property)
        {
            var result = collections;
            if (collections != null && property != null && subject != null)
            {
                result = result.Where(item =>
                    item.Manages.Any(_ => _.Property?.Iri == property && _.Subject?.Iri == subject));
            }

            return result;
        }

        /// <summary>
        /// Obtains those collections that defines a hydra:manages block matching a given property and object.
        /// </summary>
        /// <param name="collections">Collection to be filtered.</param>
        /// <param name="property">Property to match in the hydra:manages block.</param>
        /// <param name="object">Object to match in the hydra:manages block.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<ICollection> WithMembersMatching(
            this IEnumerable<ICollection> collections,
            Iri property,
            Iri @object)
        {
            var result = collections;
            if (collections != null && property != null && @object != null)
            {
                result = result.Where(item =>
                    item.Manages.Any(_ => _.Property?.Iri == property && _.Object?.Iri == @object));
            }

            return result;
        }

        /// <summary>
        /// Obtains those collections that defines a hydra:manages block matching a given type and type IRI.
        /// </summary>
        /// <param name="collections">Collection to be filtered.</param>
        /// <param name="type">Type to match in the hydra:manages block.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<ICollection> WithMembersOfType(this IEnumerable<ICollection> collections, Iri type)
        {
            return collections.WithMembersMatching(rdf.type, type);
        }
    }
}