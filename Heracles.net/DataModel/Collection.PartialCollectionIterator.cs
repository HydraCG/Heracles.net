using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RDeF.Entities;

namespace Heracles.DataModel
{
    internal class PartialCollectionIterator : IPartialCollectionIterator
    {
        private readonly IHydraClient _hydraClient;
        private readonly Iri _collectionIri;

        internal PartialCollectionIterator(ICollection collection, IHydraClient hydraClient)
        {
            _hydraClient = hydraClient;
            Update(_collectionIri = collection.Iri, collection.View);
        }

        /// <inheritdoc />
        public Iri CurrentPartIri { get; private set; }

        /// <inheritdoc />
        public Iri FirstPartIri { get; private set; }

        /// <inheritdoc />
        public Iri NextPartIri { get; private set; }

        /// <inheritdoc />
        public Iri PreviousPartIri { get; private set; }

        /// <inheritdoc />
        public Iri LastPartIri { get; private set; }

        /// <inheritdoc />
        public bool HasNextPart
        {
            get { return NextPartIri != null; }
        }

        /// <inheritdoc />
        public bool HasPreviousPart
        {
            get { return PreviousPartIri != null; }
        }

        public Task<IEnumerable<IResource>> GetFirstPart()
        {
            return GetFirstPart(CancellationToken.None);
        }

        public Task<IEnumerable<IResource>> GetFirstPart(CancellationToken cancellationToken)
        {
            return GetPart(FirstPartIri, cancellationToken);
        }

        public Task<IEnumerable<IResource>> GetNextPart()
        {
            return GetNextPart(CancellationToken.None);
        }

        public Task<IEnumerable<IResource>> GetNextPart(CancellationToken cancellationToken)
        {
            return GetPart(NextPartIri, cancellationToken);
        }

        public Task<IEnumerable<IResource>> GetPreviousPart()
        {
            return GetPreviousPart(CancellationToken.None);
        }

        public Task<IEnumerable<IResource>> GetPreviousPart(CancellationToken cancellationToken)
        {
            return GetPart(PreviousPartIri, cancellationToken);
        }

        public Task<IEnumerable<IResource>> GetLastPart()
        {
            return GetLastPart(CancellationToken.None);
        }

        public Task<IEnumerable<IResource>> GetLastPart(CancellationToken cancellationToken)
        {
            return GetPart(LastPartIri, cancellationToken);
        }

        private async Task<IEnumerable<IResource>> GetPart(Iri link, CancellationToken cancellationToken)
        {
            var collectionPart = await _hydraClient.GetResource(link, cancellationToken);
            ICollection page = collectionPart;
            if (page.View == null)
            {
                page = collectionPart.OfType<ICollection>().FirstOrDefault(_ => _.Iri == _collectionIri && _.View != null);
            }

            Update(link, page.View);
            return page.Members;
        }

        private void Update(Iri current, IPartialCollectionView view)
        {
            CurrentPartIri = current;
            FirstPartIri = view.First?.Iri;
            NextPartIri = view.Next?.Iri;
            PreviousPartIri = view.Previous?.Iri;
            LastPartIri = view.Last?.Iri;
        }
    }
}