using System.Collections.Generic;
using System.IO;

namespace Heracles
{
    /// <summary>Defines possible <see cref="HttpCallFacility" /> options.</summary>
    public interface IHttpOptions
    {
        /// <summary>Gets request headers.</summary>
        IDictionary<string, string> Headers { get; }
        
        /// <summary>Gets request body.</summary>
        Stream Body { get; }
        
        /// <summary>Gets request method.</summary>
        string Method { get; }
    }
}