using System;
using System.Collections.Generic;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>Provides an abstraction over a cache that stores resources.</summary>
    public interface IResourceCache
    {
        /// <summary>Gets or sets a resource within the cache.</summary>
        /// <param name="uri">Uri of the resource.</param>
        IResource this[Uri uri] { get; set; }

        /// <summary>Gets all cached resources of type <typeparamref name="T" />.</summary>
        /// <typeparam name="T">Type of resources to obtain from cache.</typeparam>
        /// <returns>Collection of cached resources.</returns>
        IEnumerable<T> All<T>() where T : IResource;
    }
}