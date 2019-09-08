using System.Collections.Generic;
using System.Linq;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>
    /// Provides extensions methods of <see cref="IEnumerable{IResource}" />
    /// that can be filtered with relevant criteria.
    /// </summary>
    public static class ResourceCollectionExtensions
    {
        /// <summary>Obtains a collection of resources of a given type.</summary>
        /// <typeparam name="T">Type of the resource.</typeparam>
        /// <param name="resources">Collection to be filtered.</param>
        /// <param name="iri">Expected type.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<T> OfType<T>(this IEnumerable<T> resources, Iri iri) where T : IResource
        {
            var result = resources;
            if (resources != null && iri != null)
            {
                result = result.Where(item => item.Type.Contains(iri));
            }

            return result;
        }
        
        /// <summary>Obtains a collection of resources of a given Iri.</summary>
        /// <typeparam name="T">Type of the resource.</typeparam>
        /// <param name="resources">Collection to be filtered.</param>
        /// <param name="iri"> Iri of the resources.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<T> OfIri<T>(this IEnumerable<T> resources, Iri iri) where T : IResource
        {
            var result = resources;
            if (resources != null && iri != null)
            {
                result = result.Where(item => item.Iri == iri);
            }

            return result;
        }

        /// <summary>Obtains a collection of resources being non blank nodes.</summary>
        /// <typeparam name="T">Type of the resource.</typeparam>
        /// <param name="resources">Collection to be filtered.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<T> NonBlank<T>(this IEnumerable<T> resources) where T : IResource
        {
            var result = resources;
            if (resources != null)
            {
                result = result.Where(item => !item.Iri.IsBlank);
            }

            return result;
        }
    }
}
