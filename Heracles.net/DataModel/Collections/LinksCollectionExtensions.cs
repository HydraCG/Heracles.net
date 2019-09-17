using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>Provides a collection of <see cref="IDerefencableLink" /> that can be filtered with relevant criteria.</summary>
    public static class LinksCollectionExtensions
    {
        /// <summary>Obtains a collection of links of a given relation type.</summary>
        /// <param name="links">Collection to be filteded.</param>
        /// <param name="iri">Expected relation type.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IDerefencableLink> WithRelationOf(this IEnumerable<IDerefencableLink> links, Iri iri)
        {
            var result = links;
            if (links != null && iri != null)
            {
                result = result.Where(item => item.Relation == iri);
            }

            return result;
        }

        /// <summary>Obtains a collection of operations being an Hydra TemplatedLink.</summary>
        /// <param name="links">Collection to be filteded.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<ITemplatedLink> WithTemplate(this IEnumerable<IDerefencableLink> links)
        {
            return links.OfType(hydra.TemplatedLink).Cast<ITemplatedLink>();
        }
    }
}