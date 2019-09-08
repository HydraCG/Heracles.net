using System.Collections.Generic;
using System.Threading.Tasks;
using RDeF.Entities;

namespace Heracles.DataModel
{
    internal class PartialCollectionIterator : IPartialCollectionIterator
    {
        private readonly IHydraClient _hydraClient;

        internal PartialCollectionIterator(ICollection collection, IHydraClient hydraClient)
        {
            _hydraClient = hydraClient;
            Update(collection.Iri, collection.View);
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
            return GetPart(FirstPartIri);
        }

        public Task<IEnumerable<IResource>> GetNextPart()
        {
            return GetPart(NextPartIri);
        }

        public Task<IEnumerable<IResource>> GetPreviousPart()
        {
            return GetPart(PreviousPartIri);
        }

        public Task<IEnumerable<IResource>> GetLastPart()
        {
            return GetPart(LastPartIri);
        }

        private async Task<IEnumerable<IResource>> GetPart(Iri link)
        {
            var collectionPart = await _hydraClient.GetResource(link);
            var page = collectionPart;
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