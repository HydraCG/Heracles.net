using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Heracles.DataModel;

namespace Heracles.Net.Http
{
    /// <summary>Provides a default implementation of the <see cref="IResponse" />.</summary>
    public class HttpResponse : IResponse
    {
        private readonly HttpResponseMessage _response;

        /// <summary>Initializes a new instance of the <see cref="HttpResponse" /> class.</summary>
        /// <param name="url">Url of the response.</param>
        /// <param name="response">HTTP response.</param>
        public HttpResponse(Uri url, HttpResponseMessage response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Headers = new ResponseHeaders(_response.Headers, _response.Content.Headers);
        }

        /// <inheritdoc />
        public Uri Url { get; }

        /// <inheritdoc />
        public IHeaders Headers { get; }

        /// <inheritdoc />
        public int Status
        {
            get { return (int)_response.StatusCode; }
        }

        /// <inheritdoc />
        public Task<Stream> GetBody()
        {
            return _response.Content.ReadAsStreamAsync();
        }

        private class ResponseHeaders : IHeaders
        {
            private readonly HttpResponseHeaders _headers;
            private readonly HttpContentHeaders _contentHeaders;

            internal ResponseHeaders(HttpResponseHeaders headers, HttpContentHeaders contentHeaders)
            {
                _headers = headers;
                _contentHeaders = contentHeaders;
            }

            /// <inheritdoc />
            public IEnumerable<string> this[string name]
            {
                get
                {
                    var result = _contentHeaders.Where(_ => _.Key == name).Select(_ => _.Value);
                    if (!result.Any())
                    {
                        result = _headers.Where(_ => _.Key == name).Select(_ => _.Value);
                    }

                    return result.FirstOrDefault() ?? Array.Empty<string>();
                }
            }

            /// <inheritdoc />
            public bool Has(string name)
            {
                return _headers.Contains(name);
            }
        }
    }
}
