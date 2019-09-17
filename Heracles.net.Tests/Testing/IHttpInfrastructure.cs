using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heracles.Testing
{
    public interface IHttpInfrastructure
    {
        Task<IResponse> HttpCall(Uri url, IHttpOptions options, CancellationToken cancellationToken);
    }
}
