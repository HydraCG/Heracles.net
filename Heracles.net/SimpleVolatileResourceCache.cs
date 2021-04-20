using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Heracles.DataModel;

namespace Heracles
{
    /// <summary>
    /// Provides a simple, <see cref="ConcurrentDictionary{TKey,TValue}" /> based implementation
    /// of the <see cref="IResourceCache" /> interface.
    /// </summary>
    public class SimpleVolatileResourceCache : IResourceCache
    {
        private readonly ConcurrentDictionary<Uri, IResource> _cache = new ConcurrentDictionary<Uri, IResource>();
        
        /// <inheritdoc />
        public IResource this[Uri uri]
        {
            get { return _cache.TryGetValue(uri, out IResource result) ? result : null; }
            set { _cache[uri] = value; }
        }

        /// <inheritdoc />
        public IEnumerable<T> All<T>() where T : IResource
        {
            return _cache.Values.OfType<T>();
        }
    }
}