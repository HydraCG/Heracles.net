using System.Collections.Generic;

namespace Heracles.DataModel
{
    /// <summary>Provides an abstraction layer over hypermedia container.</summary>
    public interface IHypermediaContainer : IEnumerable<IResource>, IResponse, ICollection
    {
    }
}
