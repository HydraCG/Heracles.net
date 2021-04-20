using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract Hydra operation.</summary>
    [Class("hydra", "Operation")]
    public interface IOperation : IPointingResource
    {
        /// <summary>Gets the operation's display name.</summary>
        [Property("hydra", "title", DefaultValue = "")]
        string Title { get; }

        /// <summary>Gets the operation's description.</summary>
        [Property("hydra", "description", DefaultValue = "")]
        string Description { get; }
        
        /// <summary>Gets a method to be used for the call.</summary>
        [Property("hydra", "method", DefaultValue = "GET")]
        string Method { get; }

        /// <summary>Gets the expected classes.</summary>
        [Collection("hydra", "expects")]
        ISet<IClass> Expects { get; }
        
        /// <summary>Gets the returned classes.</summary>
        [Collection("hydra", "returns")]
        ISet<IClass> Returns { get; }

        /// <summary>Gets the expected headers.</summary>
        [Collection("hydra", "expectsHeader")]
        ISet<string> ExpectedHeaders { get; }

        /// <summary>Gets the returned headers.</summary>
        [Collection("hydra", "returnsHeader")]
        ISet<string> ReturnedHeaders { get; }

        /// <summary>Gets the media type of a resource that provided this operation.</summary>
        string OriginatingMediaType { get; }
    }
}