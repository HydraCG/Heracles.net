using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Collections.Generic;
using Heracles.Entities;
using Heracles.Namespaces;
using RDeF.Entities;
using RollerCaster;

namespace Heracles.DataModel
{
    /// <summary>Provides a default implementation of the <see cref="IHypermediaProcessor" />.</summary>
    public sealed class HypermediaContainer : IHypermediaContainer
    {
        private readonly IResponse _response;
        private readonly IResource _rootResource;
        private readonly IEnumerable<IResource> _hypermedia;
        private readonly Func<IPartialCollectionIterator> _getIterator;

        /// <summary>Initializes a new instance of the <see cref="HypermediaContainer" /> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="rootResource">Main resource associated with the requested Url.</param>
        /// <param name="hypermedia">All hypermedia controls discovered with the payload of the resource owning this container.</param>
        public HypermediaContainer(
            IResponse response,
            IResource rootResource,
            IEnumerable<IResource> hypermedia)
        {
            _rootResource = rootResource ?? throw new ArgumentNullException(nameof(rootResource));
            _hypermedia = hypermedia ?? throw new ArgumentNullException(nameof(hypermedia));
            _response = response ?? throw new ArgumentNullException(nameof(response));
            Collections = hypermedia.DiscoverCollections();
            Operations = new HashSet<IOperation>();
            Links = new HashSet<IDereferencableLink>();
            var hydraResource = rootResource.As<IHydraResource>(hydra.Resource);
            if (hydraResource != null)
            {
                Operations = hydraResource.Operations;
                Links = hydraResource.Links;
            }

            if (rootResource is IResourceView resourceView)
            {
                View = resourceView.View;
            }

            var collection = rootResource.As<ICollection>(hydra.Collection);
            if (collection != null)
            {
                Members = collection.Members;
                TotalItems = collection.TotalItems;
                Manages = collection.Manages;
                _getIterator = collection.GetIterator;
            }
        }

        /// <inheritdoc />
        public IHeaders Headers
        {
            get { return _response.Headers; }
        }

        /// <inheritdoc />
        public int Status
        {
            get { return _response.Status; }
        }

        /// <inheritdoc />
        public Uri Url
        {
            get { return _response.Url; }
        }

        /// <inheritdoc />
        public Iri Iri
        {
            get { return _rootResource.Iri; }
        }

        /// <inheritdoc />
        public ISet<Iri> Type
        {
            get { return _rootResource.Type; }
        }

        /// <inheritdoc />
        public string DisplayName
        {
            get { return _rootResource.GetDisplayName(); }
        }

        /// <inheritdoc />
        public string TextDescription
        {
            get { return _rootResource.GetTextDescription(); }
        }

        /// <inheritdoc />
        public string Label
        {
            get { return _rootResource.Label; }
        }

        /// <inheritdoc />
        public string Comment
        {
            get { return _rootResource.Comment; }
        }

        /// <inheritdoc />
        public IEnumerable<Relation> UnmappedRelations
        {
            get { return _rootResource.UnmappedRelations; }
        }

        /// <inheritdoc />
        public int TotalItems { get; }

        /// <inheritdoc />
        public IPartialCollectionView View { get; }

        /// <inheritdoc />
        public ISet<IResource> Members { get; }

        /// <inheritdoc />
        public ISet<IStatement> Manages { get; }

        /// <inheritdoc />
        public ISet<ICollection> Collections { get; }

        /// <inheritdoc />
        public ISet<IOperation> Operations { get; }
        
        /// <inheritdoc />
        public ISet<IDereferencableLink> Links { get; }

        IEntityContext IEntity.Context
        {
            get { return _rootResource.Context; }
        }

        /// <inheritdoc />
        public Task<Stream> GetBody()
        {
            return GetBody(CancellationToken.None);
        }
        
        /// <inheritdoc />
        public Task<Stream> GetBody(CancellationToken cancellationToken)
        {
            return _response.GetBody(cancellationToken);
        }

        /// <inheritdoc />
        public IPartialCollectionIterator GetIterator()
        {
            return _getIterator?.Invoke();
        }

        /// <inheritdoc />
        public IEnumerator<IResource> GetEnumerator()
        {
            return _hypermedia.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
