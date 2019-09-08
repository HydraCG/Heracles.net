using System;
using System.Net;
using System.Threading.Tasks;

namespace Heracles.Net
{
    internal class HttpWebRequestHttpCallFacility
    {
        public async Task<IResponse> Call(Uri url, IHttpOptions options = null)
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

            var response = await httpRequest.GetResponseAsync();
            if (response is HttpWebResponse httpResponse)
            {
                return new HttpResponse(response.ResponseUri, httpResponse);
            }

            return null;
        }
    }
}
