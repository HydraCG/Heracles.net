using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Heracles.Net.Http
{
    internal class HttpRequestHttpCallFacility
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<IResponse> Call(Uri url, IHttpOptions options, CancellationToken cancellationTokenn)
        {
            var httpRequest = new HttpRequestMessage(new HttpMethod(options?.Method ?? "GET"), url);
            if (options != null)
            {
                foreach (var header in options.Headers)
                {
                    if (!httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        httpRequest.EnsureContentFrom(options.Body).Headers.Add(header.Key, header.Value);
                    }
                }
            }

            var httpResponse = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationTokenn);
            return new HttpResponse(url, httpResponse);
        }

    }
}
