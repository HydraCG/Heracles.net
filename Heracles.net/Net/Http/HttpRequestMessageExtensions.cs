using System.IO;
using System.Net.Http;

namespace Heracles.Net.Http
{
    internal static class HttpRequestMessageExtensions
    {
        internal static HttpContent EnsureContentFrom(this HttpRequestMessage request, Stream body)
        {
            if (request.Content == null)
            {
                request.Content = new StreamContent(body ?? new MemoryStream());
            }

            return request.Content;
        }
    }
}
