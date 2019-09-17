using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RDeF.Entities;

namespace Heracles.DataModel
{
    /// <summary>
    /// Describes an abstract view of a partial collection.
    /// This is an iterator-like pattern that once obtained from it's owning <see cref="ICollection"/>
    /// should maintain it's state between consecutive next/previous page calls.
    /// </summary>
    public interface IPartialCollectionIterator
    {
        /// <summary>Gets the IRI of current part.</summary>
        Iri CurrentPartIri { get; }

        /// <summary>Gets the IRI to the first part.</summary>
        Iri FirstPartIri { get; }

        /// <summary>Gets the IRI to the next part.</summary>
        Iri NextPartIri { get; }

        /// <summary>Gets the IRI to the previous part.</summary>
        Iri PreviousPartIri { get; }

        /// <summary>Gets the IRI to the last part.</summary>
        Iri LastPartIri { get; }

        /// <summary>Gets a value indicating whether the view has a next part available.</summary>
        bool HasNextPart { get; }

        /// <summary>Gets a value indicating whether the view has a previous part available.</summary>
        bool HasPreviousPart { get; }

        /// <summary>Retrieves a first part of the partial collection view.</summary>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetFirstPart();

        /// <summary>Retrieves a first part of the partial collection view.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetFirstPart(CancellationToken cancellationToken);

        /// <summary>Retrieves a next part of the partial collection view.</summary>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetNextPart();

        /// <summary>Retrieves a next part of the partial collection view.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetNextPart(CancellationToken cancellationToken);

        /// <summary>Retrieves a previous part of the partial collection view.</summary>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetPreviousPart();

        /// <summary>Retrieves a previous part of the partial collection view.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetPreviousPart(CancellationToken cancellationToken);

        /// <summary>Retrieves a last part of the partial collection view.</summary>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetLastPart();

        /// <summary>Retrieves a last part of the partial collection view.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of resources.</returns>
        Task<IEnumerable<IResource>> GetLastPart(CancellationToken cancellationToken);
    }
}