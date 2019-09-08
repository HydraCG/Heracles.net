using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>Provides a collection of types that can be filtered with relevant criteria.</summary>
    public static class TypesCollectionExtensions
    {
        /// <summary>
        /// Gets a value indicating that resource owning this type's collection has hydra:Collection type.
        /// </summary>
        /// <param name="types">Collection to be checked.</param>
        /// <returns>
        /// <i>true</i> in case hydra:Collection is within the given <paramref name="types" />;
        /// otherwise <i>false</i>.
        /// </returns>
        public static bool IsCollection(this IEnumerable<Iri> types)
        {
            var result = false;
            if (types != null)
            {
                result = types.Contains(hydra.Collection);
            }

            return result;
        }

        /// <summary>Excludes a given <paramref name="types" />.</summary>
        /// <param name="types">Collection to be filtered.</param>
        /// <param name="type">Type to be excluded.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<Iri> Except(this IEnumerable<Iri> types, Iri type)
        {
            var result = types;
            if (types != null && type != null)
            {
                result = result.Where(item => item != type);
            }

            return result;
        }
    }
}