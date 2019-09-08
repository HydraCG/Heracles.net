using System.Collections.Generic;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstraction layer over HTTP headers.</summary>
    public interface IHeaders
    {
        /// <summary>Gets the values of a header <paramref name="name" />.</summary>
        /// <param name="name">Name of the header to obtain values of.</param>
        /// <returns>Collection of values.</returns>
        IEnumerable<string> this[string name] { get; }
        
        /// <summary>Checks whether the collection has a header of a given name.</summary>
        /// <param name="name">Name of the header to check for existence.</param>
        /// <returns>
        /// <b>true</b> in case there is a header of the given <paramref name="name" />; otherwise <b>false</b>.
        /// </returns>
        bool Has(string name);
    }
}