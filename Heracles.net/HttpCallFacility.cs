using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heracles
{
    /// <summary>Allows to obtain an HTTP resource.</summary>
    /// <param name="url">Url to be called.</param>
    /// <param name="options">Additional call options, if any.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Received response.</returns>
    public delegate Task<IResponse> HttpCallFacility(Uri url, IHttpOptions options, CancellationToken cancellationToken);
}