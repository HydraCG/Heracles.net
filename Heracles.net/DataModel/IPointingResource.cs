using System;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstract description of a resource that points to another one.</summary>
    public interface IPointingResource : IHydraResource
    {
        /// <summary>Gets a base URL that can be used to resolve target in case it is relative.</summary>
        Uri BaseUrl { get; }

        /// <summary>Gets a target URL to be called.</summary>
        IResource Target { get; }
    }
}