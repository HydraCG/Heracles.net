using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Provides an abstraction layer over an HTTP responses.</summary>
    public interface IResponse
    {
        /// <summary>Gets the Url of the response.</summary>
        Uri Url { get; }

        /// <summary>Gets the response's headers.</summary>
        IHeaders Headers { get; }
        
        /// <summary>Gets the response status.</summary>
        int Status { get; }

        /// <summary>Gets the body of the response.</summary>
        /// <returns>Task of this operation.</returns>
        Task<Stream> GetBody();

        /// <summary>Gets the body of the response.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task of this operation.</returns>
        Task<Stream> GetBody(CancellationToken cancellationToken);
    }
}
