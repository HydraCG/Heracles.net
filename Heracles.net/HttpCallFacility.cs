using System;
using System.Threading.Tasks;

namespace Heracles
{
    /// <summary>Allows to obtain an HTTP resource.</summary>
    /// <param name="url">Url to be called.</param>
    /// <param name="options">Call options.</param>
    public delegate Task<IResponse> HttpCallFacility(Uri url, IHttpOptions options = null);
}