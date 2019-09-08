using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Heracles.Net.Http
{
    internal class HttpRequestHttpCallFacility
    {
        private static readonly HttpClient Client = new HttpClient();

        private static readonly IDictionary<string, HttpMethod> HttpMethods = new Dictionary<string, HttpMethod>()
        {
            { "GET", HttpMethod.Get },
            { "PUT", HttpMethod.Put },
            { "POST", HttpMethod.Post },
            { "DELETE", HttpMethod.Delete },
            { "HEAD", HttpMethod.Head },
            { "OPTIONS", HttpMethod.Options },
            { "TRACE", HttpMethod.Trace },
            { String.Empty, HttpMethod.Get } 
        };

        public async Task<IResponse> Call(Uri url, IHttpOptions options = null)
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethods.TryGetValue(options?.Method ?? String.Empty, out HttpMethod method) ? method : new HttpMethod(options.Method),
                url);
            if (options != null)
            {
                foreach (var header in options.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            var httpResponse = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead);
            return new HttpResponse(url, httpResponse);
        }
    }
}
