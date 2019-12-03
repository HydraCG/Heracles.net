using System.Collections.Generic;
using System.Linq;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.DataModel.Collections
{
    /// <summary>Provides a collection of <see cref="IDereferencableLink" /> that can be filtered with relevant criteria.</summary>
    public static class LinksCollectionExtensions
    {
        /// <summary>Obtains a collection of links of a given relation type.</summary>
        /// <param name="links">Collection to be filteded.</param>
        /// <param name="iri">Expected relation type.</param>
        /// <returns>Filtered collection.</returns>
        public static IEnumerable<IDereferencableLink> WithRelationOf(this IEnumerable<IDereferencableLink> links, Iri iri)
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
        public static IEnumerable<ITemplatedLink> WithTemplate(this IEnumerable<IDereferencableLink> links)
        {
            return links.OfType(hydra.TemplatedLink).Cast<ITemplatedLink>();
        }
    }
}