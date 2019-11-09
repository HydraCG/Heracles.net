using RDeF.Entities;

namespace Heracles.Rdf
{
    /// <summary>Provides an abstraction over ontology.</summary>
    public interface IOntologyProvider
    {
        /// <summary>Gets the domain for a given property.</summary>
        /// <param name="predicate">Iri of the predicate for which to obtain a domain.</param>
        /// <returns>Domain for a given <paramref name="predicate" /> if defined; otherwise <i>null</i>.</returns>
        Iri GetDomainFor(Iri predicate);

        /// <summary>Gets the range for a given property.</summary>
        /// <param name="predicate">Iri of the predicate for which to obtain a range.</param>
        /// <returns>Range for a given <paramref name="predicate" /> if defined; otherwise <i>null</i>.</returns>
        Iri GetRangeFor(Iri predicate);
    }
}
