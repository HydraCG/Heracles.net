using System.Collections.Generic;
using System.IO;

namespace Heracles
{
    /// <summary>Provides a default implementation of the <see cref="IHttpOptions" />.</summary>
    public class HttpOptions : IHttpOptions
    {
        private const string HttpGetMethod = "GET";
        
        /// <summary>Initializes a new instance of the <see cref="HttpOptions" /> class.</summary>
        /// <param name="method">Request method.</param>
        /// <param name="body">Request body.</param>
        /// <param name="headers">Request headers.</param>
        public HttpOptions(string method = HttpGetMethod, Stream body = null, IDictionary<string, string> headers = null)
        {
            Body = body;
            Method = method ?? HttpGetMethod;
            Headers = headers ?? new Dictionary<string, string>();
        }
        
        /// <inheritdoc />
        public IDictionary<string, string> Headers { get; }
        
        /// <inheritdoc />
        public Stream Body { get; }
        
        /// <inheritdoc />
        public string Method { get; }
    }
}