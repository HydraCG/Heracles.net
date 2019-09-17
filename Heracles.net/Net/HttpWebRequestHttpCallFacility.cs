using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Heracles.Net
{
    internal class HttpWebRequestHttpCallFacility
    {
        public async Task<IResponse> Call(Uri url, IHttpOptions options, CancellationToken cancellationToken)
        {
            var request = WebRequest.Create(url);
            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null && !String.IsNullOrEmpty(options?.Method))
            {
                httpRequest.Method = options.Method;
                foreach (var header in options.Headers)
                {
                    httpRequest.Headers[header.Key] = header.Value;
                }
            }

            IDisposable registration = null;
            registration = cancellationToken.Register(() =>
            {
                httpRequest.Abort();
                registration?.Dispose();
            });

            try
            {
                var response = await httpRequest.GetResponseAsync();
                if (response is HttpWebResponse httpResponse)
                {
                    return new HttpResponse(response.ResponseUri, httpResponse);
                }

                return null;
            }
            catch (WebException error)
            {
                if (error.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                throw;
            }
        }
    }
}
