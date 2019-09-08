using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Heracles.DataModel;

namespace Heracles.Net
{
    /// <summary>Provides a default implementation of the <see cref="IResponse" />.</summary>
    public class HttpResponse : IResponse
    {
        private readonly HttpWebResponse _response;

        /// <summary>Initializes a new instance of the <see cref="HttpResponse" /> class.</summary>
        /// <param name="url">Url of the response.</param>
        /// <param name="response">HTTP response.</param>
        public HttpResponse(Uri url, HttpWebResponse response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Headers = new ResponseHeaders(_response.Headers);
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
            return Task.FromResult(_response.GetResponseStream());
        }

        private class ResponseHeaders : IHeaders
        {
            private readonly WebHeaderCollection _headers;

            internal ResponseHeaders(WebHeaderCollection headers)
            {
                _headers = headers;
            }

            /// <inheritdoc />
            public IEnumerable<string> this[string name]
            {
                get { return _headers.GetValues(name); }
            }

            /// <inheritdoc />
            public bool Has(string name)
            {
                return _headers.Keys.Cast<string>().Contains(name);
            }
        }
    }
}
