using System;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Provides an abstraction layer over <see cref="HydraClient" />.</summary>
    public interface IHydraClient
    {
        /// <summary>Gets a hypermedia provider suitable for a given response.</summary>
        /// <param name="response">Raw response to find hypermedia processor for.</param>
        /// <returns>
        /// Hypermedia processor capable of processing given <paramref name="response" /> or <b>null</b>.
        /// </returns>
        IHypermediaProcessor GetHypermediaProcessor(IResponse response);

        /// <summary>Obtains an API documentation.</summary>
        /// <param name="resource">Resource from which to obtain an API documentation.</param>
        /// <returns>API documentation obtained.</returns>
        Task<IApiDocumentation> GetApiDocumentation(IResource resource);

        /// <summary>Obtains an API documentation.</summary>
        /// <param name="resource">Resource from which to obtain an API documentation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>API documentation obtained.</returns>
        Task<IApiDocumentation> GetApiDocumentation(IResource resource, CancellationToken cancellationToken);

        /// <summary>Obtains an API documentation.</summary>
        /// <param name="url">URL from which to obtain an API documentation.</param>
        /// <returns>API documentation obtained.</returns>
        Task<IApiDocumentation> GetApiDocumentation(Uri url);

        /// <summary>Obtains an API documentation.</summary>
        /// <param name="url">URL from which to obtain an API documentation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>API documentation obtained.</returns>
        Task<IApiDocumentation> GetApiDocumentation(Uri url, CancellationToken cancellationToken);

        /// <summary>Obtains a representation of a resource.</summary>
        /// <param name="resource">Resource carrying an IRI of the resource to be obtained.</param>
        Task<IHypermediaContainer> GetResource(IResource resource);

        /// <summary>Obtains a representation of a resource.</summary>
        /// <param name="resource">Resource carrying an IRI of the resource to be obtained.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IHypermediaContainer> GetResource(IResource resource, CancellationToken cancellationToken);

        /// <summary>Obtains a representation of a resource.</summary>
        /// <param name="url">URL carrying an IRI of the resource to be obtained.</param>
        Task<IHypermediaContainer> GetResource(Uri url);

        /// <summary>Obtains a representation of a resource.</summary>
        /// <param name="url">URL carrying an IRI of the resource to be obtained.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IHypermediaContainer> GetResource(Uri url, CancellationToken cancellationToken);
        
        ///<summary>Invokes a given operation.</summary>
        ///<param name="operation">Operation descriptor to be invoked.</param>
        ///<param name="body">Optional resource to be used as a body of the operation.</param>
        ///<param name="parameters">Optional auxiliary parameters.</param>
        ///<returns>Response with the obtained resource</returns>
        Task<IResponse> Invoke(IOperation operation, IResource body = null, IResource parameters = null);
        
        ///<summary>Invokes a given operation.</summary>
        ///<param name="operation">Operation descriptor to be invoked.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        ///<returns>Response with the obtained resource</returns>
        Task<IResponse> Invoke(IOperation operation, CancellationToken cancellationToken);
        
        ///<summary>Invokes a given operation.</summary>
        ///<param name="operation">Operation descriptor to be invoked.</param>
        ///<param name="body">Optional resource to be used as a body of the operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        ///<returns>Response with the obtained resource</returns>
        Task<IResponse> Invoke(IOperation operation, IResource body, CancellationToken cancellationToken);
        
        ///<summary>Invokes a given operation.</summary>
        ///<param name="operation">Operation descriptor to be invoked.</param>
        ///<param name="body">Optional resource to be used as a body of the operation.</param>
        ///<param name="parameters">Optional auxiliary parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        ///<returns>Response with the obtained resource</returns>
        Task<IResponse> Invoke(IOperation operation, IResource body, IResource parameters, CancellationToken cancellationToken);
    }
}