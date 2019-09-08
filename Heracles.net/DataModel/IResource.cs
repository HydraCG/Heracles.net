using RDeF.Entities;
using RDeF.Mapping.Attributes;

namespace Heracles.DataModel
{
    /// <summary>Describes an abstract RDF resource.</summary>
    [Class("rdfs", "Resource")]
    public interface IResource : ITypedEntity
    {
        /// <summary>Gets the display name of the resource.</summary>
        string DisplayName { get; }

        /// <summary>Gets the description of the resource.</summary>
        string TextDescription { get; }

        /// <summary>Gets the resource's label.</summary>
        [Property("rdfs", "label")]
        string Label { get; }

        /// <summary>Gets the resource's description.</summary>
        [Property("rdfs", "comment")]
        string Comment { get; }
    }
}
